using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using Editor.UndoRedo;
using Editor.Entities;

namespace Editor
{
    public class View2DControl : Control
    {
		private const int HANDLE_SIZE = 8;
		private const int DIMENSION_TEXT_OFFSET = 12;

		public delegate void GlobalInvalidationHandler();
		public event GlobalInvalidationHandler OnGlobalInvalidation;

        private static Keys[] CMD_KEYS =
        {
            Keys.Left,
            Keys.Right,
            Keys.Up,
            Keys.Down,
            Keys.Control,
            Keys.Delete,
            Keys.Space,
            Keys.Escape,
            Keys.Alt,
			Keys.ShiftKey | Keys.Shift,
        };

		private static Cursor[] HANDLE_CURSORS =
		{
			Cursors.SizeNWSE,	Cursors.SizeNS,		Cursors.SizeNESW,
			Cursors.SizeWE,							Cursors.SizeWE,
			Cursors.SizeNESW,	Cursors.SizeNS,		Cursors.SizeNWSE
		};

        private Camera2D _camera;

        private Brush _backgroundBrush;

        private Pen _solidPen;
        private PointF _startPosition;
        private PointF _endPosition;
        private bool _lmbDown;
		private bool _mmbDown;
		private bool _spaceDown;
		private bool _shiftDown;
		private bool _controlDown;
        private int _directionType;
		private PointF _hoverPosition;
		private PointF _clipStart;
		private PointF _clipEnd;
		private bool _clipping;

		private PointF _entityPosition;
		private PointF _handlePosition;
		private PointF _movePosition;
		private PointF _moveOffset;

		private PointF _previousMousePosition;
		
		private int _handleIndex;
		private bool _movingSelection;
		private bool _duplactingSelection;
		private bool _movingVertex;
		private List<Tuple<int, int, int>> _selectedVertices;

		private Level _level;
		private Level.ChangeHandler _invalidateAction;
		
		private CommandStack _commandStack;
		private List<CommandSolidChanged> _commandSolidsChanged;
		private List<CommandEntityChanged> _commandEntitiesChanged;

        public int Direction
        {
            get { return _directionType; }
            set
            {
                _directionType = value;
                if( _directionType < 0 )
                    _directionType = 0;
                else if( _directionType > 2 )
                    _directionType = 2;

                if( _directionType == 0 )
                    _camera.Direction = new Triple( 1, 0, 0 );
                else if( _directionType == 1 )
                    _camera.Direction = new Triple( 0, 1, 0 );
                else if( _directionType == 2 )
                    _camera.Direction = new Triple( 0, 0, 1 );
            }
        }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Level Level
		{
			get { return _level; }
			set
			{
				if( _level != null )
				{
					_level.OnSolidChange -= _invalidateAction;
					_level.OnEntityChange -= _invalidateAction;
				}

				_level = value;
				_level.OnSolidChange += _invalidateAction;
				_level.OnEntityChange += _invalidateAction;

				if( _directionType == 0 )
				{
					var solid = new Solid();
					// TRIANGLE
					/*solid.Faces.AddRange
					(
						new[]
						{
							new Face(new Triple(-1, 1, 0), 1),
							new Face(new Triple(1,1,0), 1),
							new Face(new Triple(0,0,1), 1),
							new Face(new Triple(0,0,-1), 1),
							new Face(new Triple(0,-1,0), 1)
						}
					);*/

					// PENTAGON
					solid.Faces.AddRange
					(
						new[]
						{
						new Face(new Triple(0,0,1), 1),
						new Face(new Triple(0,0,-1), 1),

						new Face(new Triple(-2,-1,0),1),
						new Face(new Triple(-1,1,0), 1),
						new Face(new Triple(1,1,0), 1),
						new Face(new Triple(2,-1,0), 1),
						new Face(new Triple(0,-1,0), 1)
						}
					);

					// ROTATED CUBE
					/*solid.Faces.AddRange
					(
						new[]
						{
							new Face(new Triple(0,0,1),1),
							new Face(new Triple(0,0,-1),1),

							new Face(new Triple(-1,-1,0), 1),
							new Face(new Triple(-1,1,0), 1),
							new Face(new Triple(1,1,0), 1 ),
							new Face(new Triple(1,-1,0), 1 )
						}
					);*/

					//_level.AddSolid( solid );
				}

				Invalidate();
			}
		}

		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public CommandStack CommandStack
		{
			get { return _commandStack; }
			set
			{
				_commandStack = value;
				_commandStack.OnDo += ( command ) => Invalidate();
				_commandStack.OnUndo += ( command ) => Invalidate();
				_commandStack.OnRedo += ( command ) => Invalidate();
			}
		}

		public View2DControl()
        {
            DoubleBuffered = true;

            _camera = new Camera2D();
			_camera.Zoom = 5;

            _backgroundBrush = new SolidBrush( EditorColors.BACKGROUND_LOW );
			
            _solidPen = new Pen( Color.White );
			_solidPen.DashPattern = EditorColors.DASH_PATTERN;

			_invalidateAction = new Level.ChangeHandler( () => Invalidate() );

			_handleIndex = -1;
			_clipping = false;
			
			_movingSelection = false;
			_duplactingSelection = false;
			_movingVertex = false;
			_selectedVertices = new List<Tuple<int, int, int>>();

			_commandSolidsChanged = new List<CommandSolidChanged>();
			_commandEntitiesChanged = new List<CommandEntityChanged>();
        }

		private PointF SnapToGrid( PointF point )
		{
			var result = _camera.ToGlobal( point );
			result = _camera.Snap( Grid.Gap, result );
			result = _camera.ToLocal( result );

			return result;
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			if(_directionType == 1 )
				_camera.Position = new PointF( (int)(Size.Width * -0.25f), (int)(Size.Height * -0.25f) );
			else
				_camera.Position = new PointF( (int)( Size.Width * -0.25f ), (int)( Size.Height * -0.75f ) );

			EditorTool.SelectedSolids.CollectionChanged += ( sender, args ) => Invalidate();
			EditorTool.SelectedEntities.CollectionChanged += ( sender, args ) => Invalidate();

			Log.AddFunctor( Name, () => "Camera: " + _camera.Position.ToString() );
			Log.AddFunctor( Name, () => "Zoom: " + _camera.Zoom.ToString() );
			Log.AddFunctor( Name, () => "Hover: " + _hoverPosition.ToString() );
			Log.AddFunctor( Name, () => "Grid size: " + Grid.Size.ToString() );
			Log.AddFunctor( Name, () => "Grid gap: " + Grid.Gap.ToString() );
			Log.AddFunctor( Name, () => "Clip end: " + _clipEnd.ToString() );
		}

		protected override void OnPaint( PaintEventArgs e )
        {
            var g = e.Graphics;
            var rect = new RectangleF( 0, 0, Size.Width, Size.Height );

            // paint background
            g.FillRectangle( _backgroundBrush, rect );

			if( DesignMode )
				return;

			// paint grid
			Grid.Paint2D( g, _camera, Size );

			// paint axis gizmo
			var gizmoExtent = Grid.Gap * 3;
			var origo = _camera.ToLocal( _camera.Project( new Triple(0,0,0) ) );
			var xproj = _camera.ToLocal( _camera.Project( new Triple( gizmoExtent, 0, 0 ) ) );
			var yproj = _camera.ToLocal( _camera.Project( new Triple( 0, gizmoExtent, 0 ) ) );
			var zproj = _camera.ToLocal( _camera.Project( new Triple( 0, 0, gizmoExtent ) ) );

			g.DrawLine( EditorColors.PEN_FADED_RED, origo, xproj );
			g.DrawLine( EditorColors.PEN_FADED_GREEN, origo, yproj );
			g.DrawLine( EditorColors.PEN_FADED_BLUE, origo, zproj );

            // paint outline of new solid
            if( EditorTool.Current == EditorTools.Solid && _lmbDown )
            {
                var minPoint = new PointF( Math.Min( _startPosition.X, _endPosition.X ), Math.Min( _startPosition.Y, _endPosition.Y ) );
                var maxPoint = new PointF( Math.Max( _startPosition.X, _endPosition.X ), Math.Max( _startPosition.Y, _endPosition.Y ) );
                var solidRect = new RectangleF( minPoint, new SizeF( maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y ) );
				g.DrawRectangle( EditorColors.PEN_WHITE, solidRect.X, solidRect.Y, solidRect.Width, solidRect.Height );

				// paint dimensions text
				var globalMin = _camera.ToGlobal( minPoint ).Deflate( Grid.Gap ).Inflate( Grid.Size );
				var globalMax = _camera.ToGlobal( maxPoint ).Deflate( Grid.Gap ).Inflate( Grid.Size );
				var size = new PointF( globalMax.X - globalMin.X, globalMax.Y - globalMin.Y );

				var numberFormat = new NumberFormatInfo() { NumberDecimalSeparator = "." };
				var leftText = size.Y.ToString( "0.0", numberFormat );
				var topText = size.X.ToString( "0.0", numberFormat );

				var leftTextSize = g.MeasureString( leftText, EditorColors.SOLID_DIMENSIONS_FONT );
				var topTextSize = g.MeasureString( topText, EditorColors.SOLID_DIMENSIONS_FONT );

				var center = solidRect.GetCenter();
				var leftPosition = new PointF( solidRect.Left - leftTextSize.Width - DIMENSION_TEXT_OFFSET, center.Y - leftTextSize.Height/2 );
				var topPosition = new PointF( center.X - topTextSize.Width / 2, solidRect.Top - topTextSize.Height - DIMENSION_TEXT_OFFSET );

				g.DrawString( leftText, EditorColors.SOLID_DIMENSIONS_FONT, EditorColors.BRUSH_WHITE, leftPosition );
				g.DrawString( topText, EditorColors.SOLID_DIMENSIONS_FONT, EditorColors.BRUSH_WHITE, topPosition );
            }

			// paint solids
			foreach( var solid in _level.Solids )
			{
				solid.Paint2D( g, _camera );
			}

			// paint selection bounds
			var selectionMinPoint = new PointF( 9999.0f, 9999.0f );
			var selectionMaxPoint = new PointF( -9999.0f, -9999.0f );
			var facePoints = new List<PointF>();
			var selectedSolids = EditorTool.SelectedSolids;
			if( !selectedSolids.Empty() )
			{
				var faces = selectedSolids.SelectMany( solid => solid.Faces.Where( x => x.Plane.Normal.Dot( _camera.Direction ) > 0 ) ).ToArray();
				foreach( var face in faces )
				{
					var projectedPoints = face.Vertices.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( Grid.Gap ).Deflate( Grid.Size ) ) ).Distinct().ToArray();

					facePoints.AddRange( projectedPoints );

					foreach( var p in face.Vertices.Select( x => _camera.Project( x ) ) )
					{
						if( p.X < selectionMinPoint.X )
							selectionMinPoint.X = p.X;
						if( p.Y < selectionMinPoint.Y )
							selectionMinPoint.Y = p.Y;
						if( p.X > selectionMaxPoint.X )
							selectionMaxPoint.X = p.X;
						if( p.Y > selectionMaxPoint.Y )
							selectionMaxPoint.Y = p.Y;
					}
				}
			}

			var selectedEntities = EditorTool.SelectedEntities;
			if( !selectedEntities.Empty() )
			{
				foreach( var entity in selectedEntities )
				{
					var center = _camera.ToLocal( _camera.Project( entity.Position ).Inflate( Grid.Gap ).Deflate( Grid.Size ) );
					var corners = center.GetCorners( Entity.ICON_SIZE );

					facePoints.AddRange( corners );

					foreach( var p in _camera.Project( entity.Position ).GetCorners( Entity.ICON_SIZE ) )
					{
						if( p.X < selectionMinPoint.X )
							selectionMinPoint.X = p.X;
						if( p.Y < selectionMinPoint.Y )
							selectionMinPoint.Y = p.Y;
						if( p.X > selectionMaxPoint.X )
							selectionMaxPoint.X = p.X;
						if( p.Y > selectionMaxPoint.Y )
							selectionMaxPoint.Y = p.Y;
					}
				}
			}

			if( !facePoints.Empty() )
			{
				var topleft = new PointF( facePoints.Min( x => x.X ), facePoints.Min( x => x.Y ) );
				var bottomright = new PointF( facePoints.Max( x => x.X ), facePoints.Max( x => x.Y ) );
				var bounds = new RectangleF( topleft.X, topleft.Y, bottomright.X - topleft.X, bottomright.Y - topleft.Y );

				var handles = Extensions.GetHandles( bounds, 8 );
				var drawBounds = bounds.Downcast();

				if( EditorTool.Current == EditorTools.Select )
				{
					// draw handle outline
					g.DrawRectangle( EditorColors.PEN_DASH_FADED_HANDLE_OUTLINE, drawBounds );

					// draw handles
					foreach( var handle in handles )
						g.FillRectangle( EditorColors.BRUSH_HANDLE, handle );
				}

				// draw dimensions text
				var size = new PointF( selectionMaxPoint.X - selectionMinPoint.X, selectionMaxPoint.Y - selectionMinPoint.Y );
				var numberFormat = new NumberFormatInfo() { NumberDecimalSeparator = "." };
				var leftText = size.X.ToString( "0.0", numberFormat );
				var topText = size.Y.ToString( "0.0", numberFormat );

				var leftTextSize = g.MeasureString( leftText, EditorColors.SOLID_DIMENSIONS_FONT );
				var topTextSize = g.MeasureString( topText, EditorColors.SOLID_DIMENSIONS_FONT );

				var center = bounds.GetCenter();
				var leftPosition = new PointF( bounds.Left - leftTextSize.Width - DIMENSION_TEXT_OFFSET, center.Y - leftTextSize.Height / 2 );
				var topPosition = new PointF( center.X - topTextSize.Width / 2, bounds.Top - DIMENSION_TEXT_OFFSET - topTextSize.Height );

				g.DrawString( leftText, EditorColors.SOLID_DIMENSIONS_FONT, EditorColors.BRUSH_WHITE, leftPosition );
				g.DrawString( topText, EditorColors.SOLID_DIMENSIONS_FONT, EditorColors.BRUSH_WHITE, topPosition );
			}

			// paint clipping handle
			if( _clipping )
			{
				var start = Extensions.FromPoint( _clipStart, 8 );
				var end = Extensions.FromPoint( _clipEnd, 8 );

				var center = Extensions.PointLerp( _clipStart, _clipEnd, 0.5f );
				var dir = Extensions.Normalize( new PointF( _clipEnd.X - _clipStart.X, _clipEnd.Y - _clipStart.Y ) );
				var complimentDir = Extensions.Normalize( new PointF( dir.Y, -dir.X ) );
				var compliment = new PointF( center.X - complimentDir.X * 32, center.Y - complimentDir.Y * 32 );

				g.FillRectangle( EditorColors.BRUSH_HANDLE, start );
				g.FillRectangle( EditorColors.BRUSH_HANDLE, end );
				g.DrawLine( EditorColors.PEN_DASH_FADED_CLIP_LINE, _clipStart, _clipEnd );
				g.DrawLine( EditorColors.PEN_DASH_FADED_CLIP_LINE, center, compliment );
			}

			// draw entities
			foreach( var entity in _level.Entities )
			{
				entity.Paint2D( g, _camera );
			}

			if( !DesignMode )
			{
				// (DEBUG) paint hover position
				using( var pen = new Pen( Color.Blue ) )
				{
					g.DrawLine( pen, new PointF( _hoverPosition.X - 4, _hoverPosition.Y ), new PointF( _hoverPosition.X + 4, _hoverPosition.Y ) );
					g.DrawLine( pen, new PointF( _hoverPosition.X, _hoverPosition.Y - 4 ), new PointF( _hoverPosition.X, _hoverPosition.Y + 4 ) );
				}

				// (DEBUG) paint log
				Log.Paint( Name, g );
			}
        }

        protected override void OnMouseEnter( EventArgs e )
        {
            base.OnMouseEnter( e );

            Focus();
        }

        protected override void OnMouseLeave( EventArgs e )
        {
            base.OnMouseLeave( e );

			_lmbDown = false;
			_mmbDown = false;
			_shiftDown = false;
        }

        protected override void OnMouseDown( MouseEventArgs e )
        {
            base.OnMouseDown( e );

			if( e.Button == MouseButtons.Middle )
				_mmbDown = true;

			if( EditorTool.Current == EditorTools.Solid )
			{
				if( e.Button == MouseButtons.Left )
				{
					_startPosition = e.Location;

					if( EditorFlags.SnapToGrid )
					{
						_startPosition = SnapToGrid( _startPosition );
					}

					_endPosition = _startPosition;
					_lmbDown = true;
					Invalidate();

					OnGlobalInvalidation?.Invoke();
				}
			}
			else if( EditorTool.Current == EditorTools.Select )
			{
				if( e.Button == MouseButtons.Left && !_controlDown )
				{
					_handleIndex = -1;

					var selectedSolids = EditorTool.SelectedSolids;
					var selectedEntities = EditorTool.SelectedEntities;

					var selectionPoints = new List<PointF>();

					if( !selectedSolids.Empty() )
					{
						var faces = selectedSolids.SelectMany( solid => solid.Faces.Where( x => x.Plane.Normal.Dot( _camera.Direction ) > 0 ) ).ToArray();
						foreach( var face in faces )
						{
							var projectedPoints = face.Vertices.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( Grid.Gap ).Deflate( Grid.Size ) ) ).ToArray();

							selectionPoints.AddRange( projectedPoints );
						}
					}

					if( !selectedEntities.Empty() )
					{
						foreach( var entity in selectedEntities )
						{
							var center = _camera.ToLocal( _camera.Project( entity.Position ).Inflate( Grid.Gap ).Deflate( Grid.Size ) );
							var corners = center.GetCorners( Entity.ICON_SIZE );

							selectionPoints.AddRange( corners );
						}
					}

					if( !selectionPoints.Empty() )
					{
						var topleft = new PointF( selectionPoints.Min( x => x.X ), selectionPoints.Min( x => x.Y ) );
						var bottomright = new PointF( selectionPoints.Max( x => x.X ), selectionPoints.Max( x => x.Y ) );
						var bounds = new RectangleF( topleft.X, topleft.Y, bottomright.X - topleft.X, bottomright.Y - topleft.Y );

						var handles = Extensions.GetHandles( bounds, 8 );
						var drawBounds = bounds.Downcast();

						// check interation handles
						for( int i=0; i<handles.Length && _handleIndex < 0; i++ )
						{
							if( handles[i].Contains( e.Location ) )
							{
								_handleIndex = i;
								_handlePosition = SnapToGrid( e.Location ).Inflate( _camera.Zoom );
							}
						}

						// check if we're trying to move the selection
						if( _handleIndex < 0 )
						{
							if( bounds.Contains( e.Location ) )
							{
								_movingSelection = true;

								_movePosition = _camera.ToGlobal( bounds.TopLeft() );
								_moveOffset = new PointF( e.X - bounds.Left, e.Y - bounds.Top );

								if( _shiftDown ) // check if we're duplicating solid
								{
									_duplactingSelection = true;

									var duplicateSolids = new List<Solid>();
									var duplicateEntities = new List<Entity>();

									foreach( var solid in selectedSolids )
									{
										var duplicate = solid.Copy();
										duplicateSolids.Add( duplicate );
										_level.AddSolid( duplicate );
									}

									foreach( var entity in selectedEntities )
									{
										var duplicate = entity.Copy();
										duplicateEntities.Add( duplicate );
										_level.AddEntity( duplicate );
									}

									selectedSolids.Clear();
									selectedEntities.Clear();

									foreach( var solid in duplicateSolids )
										selectedSolids.Add( solid );

									foreach( var entity in duplicateEntities )
										selectedEntities.Add( entity );
								}
								else
								{
									// TODO: Create CommandMultipleSolidChanged or add functionality in current command for multiple solids
									foreach( var solid in selectedSolids )
									{
										var command = new CommandSolidChanged( solid );
										command.Begin();

										_commandSolidsChanged.Add( command );
									}

									foreach( var entity in selectedEntities )
									{
										var command = new CommandEntityChanged( entity );
										command.Begin();

										_commandEntitiesChanged.Add( command );
									}
								}
							}
						}
					}
				}
			}
			else if( EditorTool.Current == EditorTools.Clip )
			{
				if( e.Button == MouseButtons.Left )
				{
					_clipStart = e.Location;
					if( EditorFlags.SnapToGrid )
						_clipStart = SnapToGrid( _clipStart );

					_clipping = true;
				}
			}
			else if( EditorTool.Current == EditorTools.Vertex )
			{
				if( e.Button == MouseButtons.Left )
				{
					_selectedVertices.Clear();

					var selectedSolids = EditorTool.SelectedSolids;
					if( !selectedSolids.Empty() )
					{
						for( int curSolid = 0; curSolid < selectedSolids.Count; curSolid++ )
						{
							var solid = selectedSolids[curSolid];
							for( int curFace = 0; curFace < solid.Faces.Count; curFace++ )
							{
								var face = solid.Faces[curFace];
								var projectedVertices = face.Vertices.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( Grid.Gap ).Deflate( Grid.Size ) ) ).ToArray();

								for( int curVertex = 0; curVertex < projectedVertices.Length; curVertex++ )
								{
									var bounds = Extensions.FromPoint( projectedVertices[curVertex], 8 );

									if( bounds.Contains( e.Location ) )
										_selectedVertices.Add( new Tuple<int, int, int>( curSolid, curFace, curVertex ) );
								}
							}

						}

						_movingVertex = ( _selectedVertices.Count > 0 );
					}
				}
			}
			else if( EditorTool.Current == EditorTools.Entity )
			{
				if( e.Button == MouseButtons.Left )
				{
					_entityPosition = e.Location;
					if( EditorFlags.SnapToGrid )
						_entityPosition = SnapToGrid( _entityPosition );
				}
			}
        }

        protected override void OnMouseUp( MouseEventArgs e )
        {
            base.OnMouseUp( e );

			if( e.Button == MouseButtons.Middle )
				_mmbDown = false;

			if( EditorTool.Current == EditorTools.Solid )
			{
				if( e.Button == MouseButtons.Left )
				{
					_endPosition = e.Location;

					if( EditorFlags.SnapToGrid )
					{
						_endPosition = SnapToGrid( _endPosition );
					}

					_lmbDown = false;
					Invalidate();

					var min = new PointF( Math.Min( _startPosition.X, _endPosition.X ), Math.Min( _startPosition.Y, _endPosition.Y ) );
					var max = new PointF( Math.Max( _startPosition.X, _endPosition.X ), Math.Max( _startPosition.Y, _endPosition.Y ) );

					if( max.X - min.X > Extensions.EPSILON && max.Y - min.Y > Extensions.EPSILON )
					{
						var gmin = _camera.ToGlobal( min );
						var gmax = _camera.ToGlobal( max );

						var minTriple = _camera.Unproject( gmin, 0 );
						var maxTriple = _camera.Unproject( gmax, Grid.Gap );

						Extensions.MinMax( ref minTriple, ref maxTriple );

						minTriple /= Grid.Gap;
						minTriple *= Grid.Size;

						maxTriple /= Grid.Gap;
						maxTriple *= Grid.Size;

						var solid = new Solid( minTriple, maxTriple );
						_level.AddSolid( solid );

						// add command to stack
						var newSolidCommand = new CommandSolidCreated( _level.Solids, solid );
						_commandStack.Do( newSolidCommand );

						OnGlobalInvalidation?.Invoke();
					}
				}
			}
			else if( EditorTool.Current == EditorTools.Select )
			{
				if( e.Button == MouseButtons.Left )
				{
					if( _handleIndex >= 0 )
					{
						_handleIndex = -1;

						foreach( var command in _commandSolidsChanged )
						{
							command.End();

							if( command.HasChanges )
								_commandStack.Do( command );
						}

						_commandSolidsChanged.Clear();

						Invalidate();
						OnGlobalInvalidation?.Invoke();
					}
					else if( _duplactingSelection )
					{
						_movingSelection = false;
						_duplactingSelection = false;

						// TODO: Create command for creating multiple solids/entities
						foreach( var solid in EditorTool.SelectedSolids )
						{
							var command = new CommandSolidCreated( _level.Solids, solid );
							_commandStack.Do( command );
						}

						foreach( var entity in EditorTool.SelectedEntities )
						{
							var command = new CommandEntityCreated( _level.Entities, entity );
							_commandStack.Do( command );
						}

						Invalidate();
						OnGlobalInvalidation?.Invoke();
					}
					else if( _movingSelection )
					{
						_movingSelection = false;

						// TODO: Create CommandMoveSelection
						var changesMade = false;
						foreach( var command in _commandSolidsChanged )
						{
							command.End();

							if( command.HasChanges )
							{
								changesMade = true;
								_commandStack.Do( command );
							}
						}
						_commandSolidsChanged.Clear();

						foreach( var command in _commandEntitiesChanged )
						{
							command.End();

							if( command.HasChanges )
							{
								changesMade = true;
								_commandStack.Do( command );
							}
						}
						_commandEntitiesChanged.Clear();

						if( changesMade )
						{
							Invalidate();
							OnGlobalInvalidation?.Invoke();
						}
					}
					else
					{
						var mpos = new Triple( e.X, e.Y, 0 );

						var minDepth = 99999.0f;
						Solid minSolid = null;
						foreach( var solid in _level.Solids )
						{
							var faces = solid.Faces.Where( x => x.Plane.Normal.Dot( _camera.Direction ) > 0 ).ToArray();

							for( int i = 0; i < faces.Length; i++ )
							{
								var face = faces[i];
								var projectedPoints = face.Vertices.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( Grid.Gap ).Deflate( Grid.Size ) ) ).ToArray();

								var windingPoints = Extensions.WindingSort2D( projectedPoints.ToArray() );

								var localMinDepth = 9999.0f;
								foreach( var v in face.Vertices )
								{
									var depth = v.Dot( _camera.Direction );
									if( depth < localMinDepth )
										localMinDepth = depth;
								}

								if( localMinDepth < minDepth )
								{
									// check against lines
									var lineIndices = new Point[windingPoints.Length];
									if( lineIndices.Length > 0 )
									{
										for( int j = 0; j < windingPoints.Length - 1; j++ )
											lineIndices[j] = new Point( j, j + 1 );
										lineIndices[lineIndices.Length - 1] = new Point( lineIndices.Length - 1, 0 );
									}

									var hovered = false;
									for( int j = 0; j < lineIndices.Length && !hovered; j++ )
									{
										var i0 = lineIndices[j].X;
										var i1 = lineIndices[j].Y;

										var p0 = windingPoints[i0].ToTriple();
										var p1 = windingPoints[i1].ToTriple();
										var dir = p1 - p0;
										dir.Normalize();

										var dif = mpos - p0;
										var proj = dir * dif.Dot( dir );

										var u = dif - proj;
										var distance = u.Length();

										if( distance < 8.0f )
										{
											// check if point is within segment
											var d0 = ( mpos - p0 ).Dot( dir );
											var d1 = ( mpos - p1 ).Dot( dir );
											if( ( d0 < 0 && d1 > 0 ) || ( d0 > 0 && d1 < 0 ) )
											{
												hovered = true;
												minDepth = localMinDepth;
												minSolid = solid;
											}
										}
									}

									// check against center
									var centerBounds = Extensions.FromPoints( windingPoints );
									var center = centerBounds.GetCenter();
									centerBounds = Extensions.FromPoint( center, 8 );

									if( centerBounds.Contains( e.X, e.Y ) )
									{
										minDepth = localMinDepth;
										minSolid = solid;
									}
								}
							}
						}

						Entity minEntity = null;
						foreach( var entity in _level.Entities )
						{
							if( entity.Contains2D( e.Location, _camera ) )
							{
								var depth = entity.Position.Dot( _camera.Direction );
								if( depth < minDepth )
								{
									minEntity = entity;
									minDepth = depth;
									minSolid = null;
								}
							}
						}

						if( _controlDown )
						{
							if( minSolid != null )
							{
								if( EditorTool.SelectedSolids.Contains( minSolid ) )
								{
									EditorTool.SelectedSolids.Remove( minSolid );
								}
								else
								{
									EditorTool.SelectedSolids.Add( minSolid );
								}
							}
							else if( minEntity != null )
							{
								if( EditorTool.SelectedEntities.Contains( minEntity ) )
								{
									EditorTool.SelectedEntities.Remove( minEntity );
								}
								else
								{
									EditorTool.SelectedEntities.Add( minEntity );
								}
							}
						}
						else
						{
							EditorTool.SelectedSolids.Clear();
							EditorTool.SelectedEntities.Clear();

							if( minSolid != null )
							{
								EditorTool.SelectedSolids.Add( minSolid );
							}
							else if( minEntity != null )
							{
								EditorTool.SelectedEntities.Add( minEntity );
							}
						}
					}
				}
			}
			else if( EditorTool.Current == EditorTools.Clip )
			{
				if( e.Button == MouseButtons.Left )
				{
					_clipping = false;
				}
			}
			else if( EditorTool.Current == EditorTools.Vertex )
			{
				if( e.Button == MouseButtons.Left )
				{
					_movingVertex = false;
					_selectedVertices.Clear();
				}
			}
			else if( EditorTool.Current == EditorTools.Entity )
			{
				if( e.Button == MouseButtons.Left )
				{
					_entityPosition = e.Location;
					if( EditorFlags.SnapToGrid )
						_entityPosition = SnapToGrid( _entityPosition );

					var position = _camera.Unproject( _camera.ToGlobal( _entityPosition ).Deflate( Grid.Gap ).Inflate( Grid.Size ) );
					var entity = new Entity( position );
					//entity.Data = new PlayerSpawn();
					entity.Data = new PointLight() { Intensity = 0.8f, Color = Color.Blue };
					_level.AddEntity( entity );

					var command = new CommandEntityCreated( _level.Entities, entity );
					_commandStack.Do( command );

					Invalidate();
					OnGlobalInvalidation?.Invoke();
				}
			}
        }

        protected override void OnMouseWheel( MouseEventArgs e )
        {
            base.OnMouseWheel( e );

            if( !_lmbDown ) // don't want to deal with zooming and creating solids at the same time
            {
                var prevZoom = _camera.Zoom;

                var delta = e.Delta > 0 ? 1 : -1;
                _camera.Zoom += delta;

                if( prevZoom != _camera.Zoom )
                    Invalidate();
            }
        }

        protected override void OnMouseMove( MouseEventArgs e )
        {
            base.OnMouseMove( e );

			if( e.Location == _previousMousePosition )
				return;

			if( _handleIndex >= 0 )
			{
				var localSnap = SnapToGrid( e.Location ).Inflate( _camera.Zoom );
				_hoverPosition = localSnap.Deflate( _camera.Zoom );

				if( localSnap != _handlePosition )
				{
					var dif = new PointF( localSnap.X - _handlePosition.X, localSnap.Y - _handlePosition.Y );
					dif = dif.Deflate( Grid.Gap ).Inflate( Grid.Size );

					if( !dif.IsEmpty )
					{
						var localDirection = new PointF( 0, 0 );

						var index = _handleIndex;
						if( index > 3 )
							index++;

						if( index % 3 == 0 )
							localDirection.X = -1.0f;
						else if( ( index + 1 ) % 3 == 0 )
							localDirection.X = 1.0f;

						if( index / 3 == 0 )
							localDirection.Y = -1.0f;
						else if( index / 3 == 2 )
							localDirection.Y = 1.0f;

						var unprojectedDirection = _camera.Unproject( localDirection );
						var unprojectedDif = _camera.Unproject( dif ) * unprojectedDirection.Absolute();

						if( !unprojectedDif.IsEmpty )
						{
							foreach( var solid in EditorTool.SelectedSolids )
							{
								var affectedFaces = solid.Faces.Where( x => x.Plane.Normal.Dot( unprojectedDirection ) > 0.0f ).ToArray();

								foreach( var face in affectedFaces )
								{
									var scale = face.Plane.Normal.Dot( unprojectedDirection );

									for( int i = 0; i < face.Vertices.Count; i++ )
										face.Vertices[i] += unprojectedDif * scale;

									face.BuildPlane();
								}

								foreach( var face in solid.Faces )
								{
									face.BuildVertices( solid );
								}
							}

							_handlePosition = localSnap;
							Invalidate();
						}
					}
				}
			}
			else if( _movingSelection )
			{
				var movePosition = new PointF( e.X - _moveOffset.X, e.Y - _moveOffset.Y );
				var localSnap = SnapToGrid( movePosition );
				var globalSnap = _camera.ToGlobal( localSnap );

				if( globalSnap != _movePosition )
				{
					var localMovement = new PointF( globalSnap.X - _movePosition.X, globalSnap.Y - _movePosition.Y );
					var unprojectedMovement = _camera.Unproject( localMovement.Deflate( Grid.Gap ).Inflate( Grid.Size ) );

					foreach( var solid in EditorTool.SelectedSolids )
						solid.Move( unprojectedMovement );

					foreach( var entity in EditorTool.SelectedEntities )
						entity.Move( unprojectedMovement );

					_movePosition = globalSnap;
					Invalidate();
				}
			}
			else if( _movingVertex )
			{
				var localSnap = SnapToGrid( e.Location );
				_hoverPosition = localSnap;

				var movedVertices = true;
				var selectedSolids = EditorTool.SelectedSolids;
				foreach( var pair in _selectedVertices )
				{
					var solid = pair.Item1;
					var face = pair.Item2;
					var vertex = pair.Item3;

					var globalSnap = _camera.Unproject( _camera.ToGlobal( localSnap ).Deflate( Grid.Gap ).Inflate( Grid.Size ), selectedSolids[solid].Faces[face].Vertices[vertex].Dot( _camera.Direction ) );

					if( globalSnap != selectedSolids[solid].Faces[face].Vertices[vertex] )
					{
						selectedSolids[solid].Faces[face].Vertices[vertex] = globalSnap;
					}
					else
					{
						movedVertices = false;
						break;
					}
				}

				if( movedVertices )
				{
					foreach( var solid in selectedSolids )
					{
						foreach( var face in solid.Faces )
						{
							face.BuildPlane();

							if( !EditorFlags.TextureLock )
								face.UpdateUVs();
						}
					}

					Invalidate();
					OnGlobalInvalidation?.Invoke();
				}
			}
			else if( _spaceDown || _mmbDown )
			{
				var mouseDelta = new PointF( e.X - _previousMousePosition.X, e.Y - _previousMousePosition.Y );
				_camera.Move( -mouseDelta.X, -mouseDelta.Y );

				Invalidate();
			}
			else
			{
				if( EditorTool.Current == EditorTools.Solid )
				{
					if( _lmbDown )
					{
						_endPosition = e.Location;

						if( EditorFlags.SnapToGrid )
						{
							_endPosition = SnapToGrid( _endPosition );
						}

						Invalidate();
					}

					_hoverPosition = e.Location;

					if( EditorFlags.SnapToGrid )
					{
						_hoverPosition = SnapToGrid( _hoverPosition );
					}

					Invalidate();
				}
				else if( EditorTool.Current == EditorTools.Select )
				{
					var mpos = new Triple( e.X, e.Y, 0 );

					var minDepth = 99999.0f;
					Solid minSolid = null;
					foreach( var solid in _level.Solids )
					{
						var faces = solid.Faces.Where( x => x.Plane.Normal.Dot( _camera.Direction ) > 0 ).ToArray();

						for( int i = 0; i < faces.Length; i++ )
						{
							var face = faces[i];
							var projectedPoints = face.Vertices.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( Grid.Gap ).Deflate( Grid.Size ) ) ).ToArray();

							var windingPoints = Extensions.WindingSort2D( projectedPoints.ToArray() );

							var localMinDepth = 9999.0f;
							foreach( var v in face.Vertices )
							{
								var depth = v.Dot( _camera.Direction );
								if( depth < localMinDepth )
									localMinDepth = depth;
							}

							if( localMinDepth < minDepth )
							{
								// check against lines
								var lineIndices = new Point[windingPoints.Length];
								if( lineIndices.Length > 0 )
								{
									for( int j = 0; j < windingPoints.Length - 1; j++ )
										lineIndices[j] = new Point( j, j + 1 );
									lineIndices[lineIndices.Length - 1] = new Point( lineIndices.Length - 1, 0 );
								}

								var hovered = false;
								for( int j = 0; j < lineIndices.Length && !hovered; j++ )
								{
									var i0 = lineIndices[j].X;
									var i1 = lineIndices[j].Y;

									var p0 = windingPoints[i0].ToTriple();
									var p1 = windingPoints[i1].ToTriple();
									var dir = p1 - p0;
									dir.Normalize();

									var dif = mpos - p0;
									var proj = dir * dif.Dot( dir );

									var u = dif - proj;
									var distance = u.Length();

									if( distance < 8.0f )
									{
										// check if point is within segment
										var d0 = ( mpos - p0 ).Dot( dir );
										var d1 = ( mpos - p1 ).Dot( dir );
										if( ( d0 < 0 && d1 > 0 ) || ( d0 > 0 && d1 < 0 ) )
										{
											hovered = true;
											minDepth = localMinDepth;
											minSolid = solid;
										}
									}
								}

								// check against center
								var centerBounds = Extensions.FromPoints( windingPoints );
								var center = centerBounds.GetCenter();
								centerBounds = Extensions.FromPoint( center, 8 );

								if( centerBounds.Contains( e.X, e.Y ) )
								{
									minDepth = localMinDepth;
									minSolid = solid;
								}
							}
						}
					}

					// check interaction with handles
					var selectedSolids = EditorTool.SelectedSolids;
					if( !selectedSolids.Empty() )
					{
						var facePoints = new List<PointF>();
						var faces = selectedSolids.SelectMany( solid => solid.Faces.Where( x => x.Plane.Normal.Dot( _camera.Direction ) > 0 ) ).ToArray();
						foreach( var face in faces )
						{
							var projectedPoints = face.Vertices.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( Grid.Gap ).Deflate( Grid.Size ) ) ).ToArray();

							facePoints.AddRange( projectedPoints );
						}

						var bounds = Extensions.FromPoints( facePoints.ToArray() );
						var handles = Extensions.GetHandles( bounds, 8 );

						var hoverHandleIndex = -1;
						for( int i = 0; i < handles.Length && hoverHandleIndex < 0; i++ )
						{
							if( handles[i].Contains( e.Location ) )
							{
								hoverHandleIndex = i;
							}
						}

						if( hoverHandleIndex >= 0 )
						{
							Cursor.Current = HANDLE_CURSORS[hoverHandleIndex];
						}
					}
				}
				else if( EditorTool.Current == EditorTools.Clip )
				{
					if( _clipping )
					{
						_clipEnd = e.Location;
						if( EditorFlags.SnapToGrid )
							_clipEnd = SnapToGrid( _clipEnd );

						Invalidate();
					}
				}
				else if( EditorTool.Current == EditorTools.Entity )
				{
					_hoverPosition = e.Location;
					if( EditorFlags.SnapToGrid )
						_hoverPosition = SnapToGrid( _hoverPosition );

					if( _lmbDown )
					{
						_entityPosition = e.Location;
						if( EditorFlags.SnapToGrid )
							_entityPosition = SnapToGrid( _entityPosition );
					}

					Invalidate();
				}
			}

			_previousMousePosition = e.Location;
		}

        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if( CMD_KEYS.Contains(keyData) )
                OnKeyDown( new KeyEventArgs( keyData ) );
            return base.ProcessCmdKey( ref msg, keyData );
        }

        protected override void OnKeyDown( KeyEventArgs e )
        {
            base.OnKeyDown( e );

			if( e.KeyCode == Keys.Space )
				_spaceDown = true;
			else if( e.KeyCode == Keys.ShiftKey )
				_shiftDown = true;
			else if( e.KeyCode == Keys.ControlKey )
				_controlDown = true;

			// camera input
			var movement = new PointF();
            if( e.KeyCode == Keys.Left || e.KeyCode == Keys.A )
                movement.X = -1;
            else if( e.KeyCode == Keys.Right || e.KeyCode == Keys.D )
                movement.X = 1;
            else if( e.KeyCode == Keys.Up || e.KeyCode == Keys.W )
                movement.Y = -1;
            else if( e.KeyCode == Keys.Down || e.KeyCode == Keys.S )
                movement.Y = 1;

            if( movement.X != 0 || movement.Y != 0 )
            {
                _camera.Move( movement.X, movement.Y );
                Invalidate();
            }

            // grid manipulation
            if( e.Alt )
            {
				if( e.KeyCode == Keys.D8 )
				{
					Grid.Decrease();
					_camera.Zoom = 5;
				}
				else if( e.KeyCode == Keys.D9 )
				{
					Grid.Increase();
					_camera.Zoom = 5;
				}

                Invalidate();

				OnGlobalInvalidation?.Invoke();
			}

			// clipping
			if( e.KeyCode == Keys.Enter )
			{
				if( EditorTool.Current == EditorTools.Clip )
				{
					_clipping = false;

					var selectedSolids = EditorTool.SelectedSolids;
					if( !selectedSolids.Empty() )
					{
						var globalStart = _camera.Unproject( _camera.ToGlobal( _clipStart ).Deflate( Grid.Gap ).Inflate( Grid.Size ) );
						var globalEnd = _camera.Unproject( _camera.ToGlobal( _clipEnd ).Deflate( Grid.Gap ).Inflate( Grid.Size ) );

						var compliment = globalEnd - _camera.Direction;

						var v1v0 = compliment - globalStart;
						var v2v0 = compliment - globalEnd;

						var normal = v2v0.Cross( v1v0 );
						var clipPlane = new Plane( normal, globalStart );

						// TODO: Create command for clipping multiple solids
						foreach( var solid in selectedSolids )
						{
							var command = new CommandSolidChanged( solid );
							command.Begin();

							if( solid.Clip( clipPlane ) )
							{
								command.End();
								_commandStack.Do( command );
							}
						}

						OnGlobalInvalidation?.Invoke();
					}

					Invalidate();
				}
			}
		}

		protected override void OnKeyUp( KeyEventArgs e )
		{
			base.OnKeyUp( e );

			if( e.KeyCode == Keys.Space )
				_spaceDown = false;
			else if( e.KeyCode == Keys.ShiftKey )
				_shiftDown = false;
			else if( e.KeyCode == Keys.ControlKey )
				_controlDown = false;
        }
    }
}
