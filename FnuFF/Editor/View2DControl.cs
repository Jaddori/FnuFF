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
using Editor.UndoRedo;
using Editor.Entities;

namespace Editor
{
    public class View2DControl : Control
    {
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
        };

		/*private static Cursor[] HANDLE_CURSORS =
		{
			Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW,
			Cursors.SizeWE, Cursors.SizeAll, Cursors.SizeWE,
			Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE
		};*/

		private static Cursor[] HANDLE_CURSORS =
	{
			Cursors.SizeNWSE,	Cursors.SizeNS,		Cursors.SizeNESW,
			Cursors.SizeWE,							Cursors.SizeWE,
			Cursors.SizeNESW,	Cursors.SizeNS,		Cursors.SizeNWSE
		};

		private static int[] GRID_SIZES = new int[] { 1, 2, 3, 4, 5, 10 };
		private const int GRID_MAX_LINES = 100;
		private const int GRID_GAP_BASE = 64;

        private Camera2D _camera;

        private Brush _backgroundBrush;
        private Pen _gridPen;
		private Pen _gridHighlightPen;
        private int _gridSize;
        private int _gridGap;
        private int _gridSizeIndex;

        private Pen _solidPen;
        private PointF _startPosition;
        private PointF _endPosition;
        private bool _snapToGrid;
        private bool _lmbDown;
		private bool _mmbDown;
		private bool _spaceDown;
        private int _directionType;
		private PointF _hoverPosition;
		private PointF _clipStart;
		private PointF _clipEnd;
		private bool _clipping;

		private PointF _entityPosition;
		private PointF _solidPosition;
		private PointF _solidOffset;
		private PointF _handlePosition;

		private PointF _previousMousePosition;
		
		private int _handleIndex;
		private bool _movingEntity;
		private bool _movingSolid;

		private Level _level;
		private Level.ChangeHandler _invalidateAction;
		
		private CommandStack _commandStack;
		private CommandSolidChanged _commandSolidChanged;
		private CommandEntityChanged _commandEntityChanged;

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
					var solid = new GeometrySolid();
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
			set { _commandStack = value; }
		}

		public View2DControl()
        {
            DoubleBuffered = true;

            _camera = new Camera2D();

            _backgroundBrush = new SolidBrush( EditorColors.BACKGROUND_LOW );
            _gridPen = new Pen( EditorColors.GRID );
			_gridHighlightPen = new Pen( EditorColors.GRID_HIGHLIGHT );
            _gridSizeIndex = 0;
            _gridSize = GRID_SIZES[_gridSizeIndex];
            _gridGap = 64;
			
            _solidPen = new Pen( Color.White );
			_solidPen.DashPattern = EditorColors.DASH_PATTERN;

            _snapToGrid = true;

			_invalidateAction = new Level.ChangeHandler( () => Invalidate() );

			var handleCursors = new[]
			{
				Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW,
				Cursors.SizeWE, Cursors.SizeAll, Cursors.SizeWE,
				Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE
			};

			_handleIndex = -1;
			_clipping = false;
        }

		private PointF SnapToGrid( PointF point )
		{
			var result = _camera.ToGlobal( point );
			result = _camera.Snap( _gridGap, result );
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

			EditorTool.OnHoveredSolidChanged += (prev, cur) => Invalidate();
			EditorTool.OnSelectedSolidChanged += ( prev, cur ) => Invalidate();
			EditorTool.OnSelectedEntityChanged += ( prev, cur ) => Invalidate();

			Log.AddFunctor( Name, () => "Camera: " + _camera.Position.ToString() );
			Log.AddFunctor( Name, () => "Hover: " + _hoverPosition.ToString() );
			Log.AddFunctor( Name, () => "Grid size: " + _gridSize.ToString() );
			Log.AddFunctor( Name, () => "Grid gap: " + _gridGap.ToString() );
			Log.AddFunctor( Name, () => "Clip end: " + _clipEnd.ToString() );
			Log.AddFunctor( Name, () =>
				{
					var sb = new StringBuilder();
					sb.Append( "Command stack:\r\n" );
					foreach( var command in _commandStack.Commands )
						sb.Append( "\t" + command.GetDescription() + "\r\n" );

					return sb.ToString();
				}
			);
			Log.AddFunctor( Name, () => "Command index: " + _commandStack.Index.ToString() );
			Log.AddFunctor( Name, () =>
				{
					var result = "No solid selected.";
					if( EditorTool.SelectedSolid != null )
					{
						result = "Selected solid faces: " + EditorTool.SelectedSolid.Faces.Count.ToString();
					}

					return result;
				}
			);
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
			for( int x = -GRID_MAX_LINES; x <= GRID_MAX_LINES; x++ )
			{
				var p1 = new PointF( x * _gridGap, -GRID_MAX_LINES*_gridGap );
				var p2 = new PointF( p1.X, GRID_MAX_LINES * _gridGap );

				var g1 = _camera.ToLocal( p1 );
				var g2 = _camera.ToLocal( p2 );

				if( g1.X > 0 && g1.X < Size.Width )
					g.DrawLine( _gridPen, g1, g2 );
			}
			
			for( int y = -GRID_MAX_LINES; y <= GRID_MAX_LINES; y++ )
			{
				var p1 = new PointF( -GRID_MAX_LINES * _gridGap, y*_gridGap );
				var p2 = new PointF( GRID_MAX_LINES * _gridGap, p1.Y );

				var g1 = _camera.ToLocal( p1 );
				var g2 = _camera.ToLocal( p2 );

				if( g1.Y > 0 && g1.Y < Size.Height )
					g.DrawLine( _gridPen, g1, g2 );
			}

			// paint axis gizmo
			var gizmoExtent = _gridGap * 3;
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
            }

			// paint solids
			foreach( var solid in _level.Solids )
			{
				solid.Paint2D( g, _camera, _gridGap, _gridSize );
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
				entity.Paint2D( g, _camera, _gridSize, _gridGap );
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

					if( _snapToGrid )
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
				if( e.Button == MouseButtons.Left )
				{
					_handleIndex = -1;

					var selectedSolid = EditorTool.SelectedSolid;

					//if( _selectedSolid != null )
					if( selectedSolid != null )
					{
						var facePoints = new List<PointF>();
						var faces = selectedSolid.Faces.Where( x => x.Plane.Normal.Dot( _camera.Direction ) > 0 ).ToArray();
						foreach( var face in faces )
						{
							var otherPlanes = selectedSolid.Faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();
							var points = Extensions.IntersectPlanes( face.Plane, otherPlanes );
							var projectedPoints = points.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( _gridGap ).Deflate( _gridSize ) ) ).ToArray();

							facePoints.AddRange( projectedPoints );
						}

						var bounds = Extensions.FromPoints( facePoints.ToArray() );
						var handles = Extensions.GetHandles( bounds, 8 );

						for( int i = 0; i < handles.Length && _handleIndex < 0; i++ )
						{
							if( handles[i].Contains( e.Location ) )
							{
								_handleIndex = i;

								_handlePosition = SnapToGrid( e.Location );
							}
						}

						// check if we're trying to move the selected solid
						if( _handleIndex < 0 )
						{
							if( bounds.Contains( e.X, e.Y ) )
							{
								_movingSolid = true;
								
								_solidPosition = _camera.ToGlobal( bounds.TopLeft() );
								_solidOffset = new PointF( e.X - bounds.Left, e.Y - bounds.Top );

								_commandSolidChanged = new CommandSolidChanged( selectedSolid );
								_commandSolidChanged.Begin();
							}
						}
						else // interaction with handles
						{
							Cursor.Current = HANDLE_CURSORS[_handleIndex];

							_commandSolidChanged = new CommandSolidChanged( selectedSolid );
							_commandSolidChanged.Begin();
						}
					}

					if( _handleIndex < 0 )
					{
						// check if we're trying to move the selected entity
						var selectedEntity = EditorTool.SelectedEntity;
						if( selectedEntity != null )
						{
							if( selectedEntity.Contains2D( e.Location, _camera, _gridSize, _gridGap ) )
							{
								_movingEntity = true;

								_commandEntityChanged = new CommandEntityChanged( selectedEntity );
								_commandEntityChanged.Begin();
							}
						}

						// check if we're trying to select an entity
						if( !_movingEntity )
						{
							var potentialEntities = _level.Entities.Where( x => x.Contains2D( e.Location, _camera, _gridSize, _gridGap ) ).OrderBy( x => x.Position.Dot( _camera.Direction ) ).ToArray();
							if( potentialEntities.Length > 0 )
							{
								EditorTool.SelectedEntity = potentialEntities[0];
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
					if( _snapToGrid )
						_clipStart = SnapToGrid( _clipStart );

					_clipping = true;
				}
			}
			else if( EditorTool.Current == EditorTools.Vertex )
			{
				if( e.Button == MouseButtons.Left )
				{
				}
			}
			else if( EditorTool.Current == EditorTools.Entity )
			{
				if( e.Button == MouseButtons.Left )
				{
					_entityPosition = e.Location;
					if( _snapToGrid )
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

					if( _snapToGrid )
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
						var maxTriple = _camera.Unproject( gmax, _gridGap );

						Extensions.MinMax( ref minTriple, ref maxTriple );

						minTriple /= _gridGap;
						minTriple *= _gridSize;

						maxTriple /= _gridGap;
						maxTriple *= _gridSize;

						var solid = new GeometrySolid( minTriple, maxTriple );
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

						_commandSolidChanged.End();
						_commandStack.Do( _commandSolidChanged );

						Invalidate();
						OnGlobalInvalidation?.Invoke();
					}
					else if( _movingSolid )
					{
						_movingSolid = false;

						_commandSolidChanged.End();
						if( _commandSolidChanged.HasChanges )
							_commandStack.Do( _commandSolidChanged );

						Invalidate();
						OnGlobalInvalidation?.Invoke();
					}
					else if( _movingEntity )
					{
						_movingEntity = false;

						_commandEntityChanged.End();

						if( _commandEntityChanged.HasChanges )
							_commandStack.Do( _commandEntityChanged );

						_commandEntityChanged = null;

						Invalidate();
						OnGlobalInvalidation?.Invoke();
					}
					else
					{
						var hoveredSolid = _level.Solids.FirstOrDefault( x => x.Hovered );
						if( hoveredSolid != null )
							hoveredSolid.Selected = true;

						EditorTool.SelectedSolid = hoveredSolid;
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
			else if( EditorTool.Current == EditorTools.Entity )
			{
				if( e.Button == MouseButtons.Left )
				{
					_entityPosition = e.Location;
					if( _snapToGrid )
						_entityPosition = SnapToGrid( _entityPosition );

					var position = _camera.Unproject( _camera.ToGlobal( _entityPosition ).Deflate( _gridGap ).Inflate( _gridSize ) );
					var entity = new Entity( position );
					entity.Data = new PlayerSpawn();
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
				var localSnap = SnapToGrid( e.Location );
				_hoverPosition = localSnap;

				if( localSnap != _handlePosition )
				{
					var dif = new PointF( localSnap.X - _handlePosition.X, localSnap.Y - _handlePosition.Y );
					dif = dif.Deflate( _gridGap ).Inflate( _gridSize );

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
							var selectedSolid = EditorTool.SelectedSolid;
							var affectedFaces = selectedSolid.Faces.Where( x => x.Plane.Normal.Dot( unprojectedDirection ) > 0.0f ).ToArray();

							foreach( var face in affectedFaces )
							{
								var scale = face.Plane.Normal.Dot( unprojectedDirection );

								for( int i = 0; i < face.Vertices.Count; i++ )
									face.Vertices[i] += unprojectedDif * scale;

								face.BuildPlane();
							}

							var allFaces = selectedSolid.Faces;
							foreach( var face in allFaces )
							{
								face.BuildVertices( selectedSolid );
							}

							_handlePosition = localSnap;

							Invalidate();
						}
					}
				}
			}
			else if( _movingSolid )
			{
				var movePosition = new PointF( e.X - _solidOffset.X, e.Y - _solidOffset.Y );
				var localSnap = SnapToGrid( movePosition );
				var globalSnap = _camera.ToGlobal( localSnap );

				if( globalSnap != _solidPosition )
				{
					var localMovement = new PointF( globalSnap.X - _solidPosition.X, globalSnap.Y - _solidPosition.Y );
					var unprojectedMovement = _camera.Unproject( localMovement.Deflate( _gridGap ).Inflate( _gridSize ) );

					var selectedSolid = EditorTool.SelectedSolid;
					selectedSolid.Move( unprojectedMovement );

					_solidPosition = globalSnap;
					Invalidate();
				}
			}
			else if( _movingEntity )
			{
				var localSnap = SnapToGrid( e.Location );
				_hoverPosition = localSnap;

				var selectedEntity = EditorTool.SelectedEntity;
				var globalSnap = _camera.Unproject( _camera.ToGlobal( localSnap ).Deflate( _gridGap ).Inflate( _gridSize ), selectedEntity.Position.Dot( _camera.Direction ) );

				if( globalSnap != selectedEntity.Position )
				{
					selectedEntity.Position = globalSnap;

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

						if( _snapToGrid )
						{
							_endPosition = SnapToGrid( _endPosition );
						}

						Invalidate();
					}

					_hoverPosition = e.Location;

					if( _snapToGrid )
					{
						_hoverPosition = _camera.ToGlobal( _hoverPosition );
						_hoverPosition = _camera.Snap( _gridGap, _hoverPosition );
						_hoverPosition = _camera.ToLocal( _hoverPosition );
					}

					Invalidate();
				}
				else if( EditorTool.Current == EditorTools.Select )
				{
					var mpos = new Triple( e.X, e.Y, 0 );

					var minDepth = 99999.0f;
					GeometrySolid minSolid = null;
					foreach( var solid in _level.Solids )
					{
						var faces = solid.Faces.Where( x => x.Plane.Normal.Dot( _camera.Direction ) > 0 ).ToArray();

						for( int i = 0; i < faces.Length; i++ )
						{
							var face = faces[i];
							var otherPlanes = solid.Faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();
							var points = Extensions.IntersectPlanes( face.Plane, otherPlanes );
							var projectedPoints = points.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( _gridGap ).Deflate( _gridSize ) ) ).ToArray();

							var windingPoints = Extensions.WindingSort2D( projectedPoints.ToArray() );

							var localMinDepth = 9999.0f;
							foreach( var p in points )
							{
								var depth = p.Dot( _camera.Direction );
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

					EditorTool.HoveredSolid = minSolid;

					// check interaction with handles
					var selectedSolid = EditorTool.SelectedSolid;
					if( selectedSolid != null )
					{
						var facePoints = new List<PointF>();
						var faces = selectedSolid.Faces.Where( x => x.Plane.Normal.Dot( _camera.Direction ) > 0 ).ToArray();
						foreach( var face in faces )
						{
							var otherPlanes = selectedSolid.Faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();
							var points = Extensions.IntersectPlanes( face.Plane, otherPlanes );
							var projectedPoints = points.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( _gridGap ).Deflate( _gridSize ) ) ).ToArray();

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

					// check hover on entities
					var hoveredSelectedEntity = false;
					var selectedEntity = EditorTool.SelectedEntity;
					if( selectedEntity != null )
					{
						if( selectedEntity.Contains2D( e.Location, _camera, _gridSize, _gridGap ) )
						{
							Cursor.Current = Cursors.SizeAll;
							hoveredSelectedEntity = true;
						}
					}

					if( !hoveredSelectedEntity )
					{
						var hasPrevHover = false;

						foreach( var entity in _level.Entities )
						{
							if( entity.Hovered )
								hasPrevHover = true;

							entity.Hovered = false;
						}

						var potentialEntites = _level.Entities.Where( x => x.Contains2D( e.Location, _camera, _gridSize, _gridGap ) ).OrderBy( x => x.Position.Dot( _camera.Direction ) ).ToArray();
						if( potentialEntites.Length > 0 )
						{
							potentialEntites[0].Hovered = true;
							Invalidate();
							OnGlobalInvalidation?.Invoke();
						}
						else if( hasPrevHover )
						{
							Invalidate();
							OnGlobalInvalidation?.Invoke();
						}
					}
				}
				else if( EditorTool.Current == EditorTools.Clip )
				{
					if( _clipping )
					{
						_clipEnd = e.Location;
						if( _snapToGrid )
							_clipEnd = SnapToGrid( _clipEnd );

						Invalidate();
					}
				}
				else if( EditorTool.Current == EditorTools.Entity )
				{
					_hoverPosition = e.Location;
					if( _snapToGrid )
						_hoverPosition = SnapToGrid( _hoverPosition );

					if( _lmbDown )
					{
						_entityPosition = e.Location;
						if( _snapToGrid )
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
                    _gridSizeIndex--;
                else if( e.KeyCode == Keys.D9 )
                    _gridSizeIndex++;

                if( _gridSizeIndex < 0 )
                    _gridSizeIndex = 0;
                else if( _gridSizeIndex >= GRID_SIZES.Length )
                    _gridSizeIndex = GRID_SIZES.Length-1;

                _gridSize = GRID_SIZES[_gridSizeIndex];
				_gridGap = (int)(GRID_GAP_BASE * _gridSize);

                Invalidate();

				OnGlobalInvalidation?.Invoke();
			}

			// command stack manipulation
			if( e.Control )
			{
				if( e.KeyCode == Keys.Z )
				{
					_commandStack.Undo();
					Invalidate();
					OnGlobalInvalidation?.Invoke();
				}
				else if( e.KeyCode == Keys.Y )
				{
					_commandStack.Redo();
					Invalidate();
					OnGlobalInvalidation?.Invoke();
				}
			}

			if( e.KeyCode == Keys.Enter )
			{
				if( EditorTool.Current == EditorTools.Clip )
				{
					_clipping = false;

					var selectedSolid = EditorTool.SelectedSolid;
					if( selectedSolid != null )
					{
						var globalStart = _camera.Unproject( _camera.ToGlobal( _clipStart ).Deflate( _gridGap ).Inflate( _gridSize ) );
						var globalEnd = _camera.Unproject( _camera.ToGlobal( _clipEnd ).Deflate( _gridGap ).Inflate( _gridSize ) );

						var compliment = globalEnd - _camera.Direction;

						var v1v0 = compliment - globalStart;
						var v2v0 = compliment - globalEnd;

						var normal = v2v0.Cross( v1v0 );
						var clipPlane = new Plane( normal, globalStart );

						var command = new CommandSolidChanged( selectedSolid );
						command.Begin();

						if( selectedSolid.Clip( clipPlane ) )
						{
							command.End();
							_commandStack.Do( command );
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

			// solid manipulation
			if( e.KeyCode == Keys.Delete )
			{
				var selectedSolid = EditorTool.SelectedSolid;
				var selectedEntity = EditorTool.SelectedEntity;

				if( selectedSolid != null )
				{
					_level.RemoveSolid( selectedSolid );

					var deleteCommand = new CommandSolidCreated( _level.Solids, selectedSolid, true );
					_commandStack.Do( deleteCommand );

					EditorTool.SelectedSolid = null;
				}
				else if( selectedEntity != null )
				{
					_level.RemoveEntity( selectedEntity );

					var deleteCommand = new CommandEntityCreated( _level.Entities, selectedEntity, true );
					_commandStack.Do( deleteCommand );

					EditorTool.SelectedEntity = null;
				}
			}
        }
    }
}
