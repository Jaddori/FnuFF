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
            Keys.Alt
        };

		private static Cursor[] HANDLE_CURSORS =
		{
			Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW,
			Cursors.SizeWE, Cursors.SizeAll, Cursors.SizeWE,
			Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE
		};

		//private static float[] GRID_SIZES = new float[]{ 0.1f, 0.5f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 8.0f, 10.0f, 15.0f, 20.0f };
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
        private Point _startPosition;
        private Point _endPosition;
        private bool _snapToGrid;
        private bool _lmbDown;
		private bool _mmbDown;
		private bool _spaceDown;
        private int _directionType;
		private Point _hoverPosition;

		private Point _previousMousePosition;

		//private SizeHandle[] _sizeHandles;
		private int _handleIndex;
		private GeometrySolid _selectedSolid;

		private Level _level;
		private Level.ChangeHandler _invalidateAction;

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
					_level.OnSolidChange -= _invalidateAction;

				_level = value;
				_level.OnSolidChange += _invalidateAction;

				/*var solid = new GeometrySolid();
				solid.Points.AddRange
				(
					new[]
					{
					new Triple(0,0,0),
					new Triple(2,0,1),
					new Triple(2,0,3),
					new Triple(0,0,2),

					new Triple(2,2,3),
					new Triple(0,2,2),
					new Triple(0,2,0),
					new Triple(2,2,1),
					}
				);
				solid.GenerateFaces();
				//_level.AddSolid( solid );*/

				Invalidate();
			}
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
			_solidPen.DashPattern = new[] { 2.0f, 2.0f };

            _snapToGrid = true;

			_invalidateAction = new Level.ChangeHandler( () => Invalidate() );

			var handleCursors = new[]
			{
				Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW,
				Cursors.SizeWE, Cursors.SizeAll, Cursors.SizeWE,
				Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE
			};

			_handleIndex = -1;
        }

		private Point SnapToGrid( Point point )
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
				_camera.Position = new Point( (int)(Size.Width * -0.25f), (int)(Size.Height * -0.25f) );
			else
				_camera.Position = new Point( (int)( Size.Width * -0.25f ), (int)( Size.Height * -0.75f ) );

			Log.AddFunctor( Name, () => "Camera: " + _camera.Position.ToString() );
			Log.AddFunctor( Name, () => "Hover: " + _hoverPosition.ToString() );
			Log.AddFunctor( Name, () => "Grid size: " + _gridSize.ToString() );
			Log.AddFunctor( Name, () => "Grid gap: " + _gridGap.ToString() );
		}

		protected override void OnPaint( PaintEventArgs e )
        {
            var g = e.Graphics;
            var rect = new Rectangle( 0, 0, Size.Width, Size.Height );

            // paint background
            g.FillRectangle( _backgroundBrush, rect );

			if( DesignMode )
				return;

			// paint grid
			for( int x = -GRID_MAX_LINES; x <= GRID_MAX_LINES; x++ )
			{
				var p1 = new Point( x * _gridGap, -GRID_MAX_LINES*_gridGap );
				var p2 = new Point( p1.X, GRID_MAX_LINES * _gridGap );

				var g1 = _camera.ToLocal( p1 );
				var g2 = _camera.ToLocal( p2 );

				if( g1.X > 0 && g1.X < Size.Width )
					g.DrawLine( _gridPen, g1, g2 );
			}
			
			for( int y = -GRID_MAX_LINES; y <= GRID_MAX_LINES; y++ )
			{
				var p1 = new Point( -GRID_MAX_LINES * _gridGap, y*_gridGap );
				var p2 = new Point( GRID_MAX_LINES * _gridGap, p1.Y );

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

			if( Direction == 1 )
			{
				var p1 = _camera.Project( new Triple( gizmoExtent, 0, 0 ) );
				var p2 = _camera.Project( new Triple( 0, gizmoExtent, 0 ) );
				var p3 = _camera.Project( new Triple( 0, 0, gizmoExtent ) );
			}

			g.DrawLine( EditorColors.PEN_FADED_RED, origo, xproj );
			g.DrawLine( EditorColors.PEN_FADED_GREEN, origo, yproj );
			g.DrawLine( EditorColors.PEN_FADED_BLUE, origo, zproj );

            // paint outline of new solid
            if( _lmbDown )
            {
                var minPoint = new Point( Math.Min( _startPosition.X, _endPosition.X ), Math.Min( _startPosition.Y, _endPosition.Y ) );
                var maxPoint = new Point( Math.Max( _startPosition.X, _endPosition.X ), Math.Max( _startPosition.Y, _endPosition.Y ) );
                var solidRect = new Rectangle( minPoint, new Size( maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y ) );
                g.DrawRectangle( EditorColors.PEN_WHITE, solidRect );
            }

			// paint solids
			foreach( var solid in _level.Solids )
			{
				var color = Color.FromArgb( EditorColors.FADE, solid.Color );
				if( solid.Selected )
					color = Color.White;
				else if( solid.Hovered )
					color = solid.Color;

				if( solid.Selected || solid.Hovered )
					_solidPen.DashStyle = DashStyle.Solid;
				else
					_solidPen.DashStyle = DashStyle.Dash;

				_solidPen.Color = color;

				var faces = solid.Faces.Where( x => x.Plane.Normal.Dot( _camera.Direction ) < 0 ).ToArray();
				foreach( var face in faces )
				{
					var otherPlanes = solid.Faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();
					var points = Extensions.IntersectPlanes( face.Plane, otherPlanes );
					var projectedPoints = points.Select( x => _camera.ToLocal( _camera.Project( x ).Inflate( _gridGap ).Deflate( _gridSize ) ) ).ToArray();

					var windingPoints = Extensions.WindingSort2D( projectedPoints.ToArray() );

					for( int i = 0; i < windingPoints.Length - 1; i++ )
					{
						g.DrawLine( _solidPen, windingPoints[i], windingPoints[i + 1] );
					}
					g.DrawLine( _solidPen, windingPoints[windingPoints.Length - 1], windingPoints[0] );
				}
			}

			if( !DesignMode )
			{
				// (DEBUG) paint hover position
				using( var pen = new Pen( Color.Blue ) )
				{
					g.DrawLine( pen, new Point( _hoverPosition.X - 4, _hoverPosition.Y ), new Point( _hoverPosition.X + 4, _hoverPosition.Y ) );
					g.DrawLine( pen, new Point( _hoverPosition.X, _hoverPosition.Y - 4 ), new Point( _hoverPosition.X, _hoverPosition.Y + 4 ) );
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

					if( _selectedSolid != null )
					{
						/*var bounds = _selectedSolid.Project( _camera, _gridGap, _gridSize );
						var handles = Extensions.GetHandles( bounds, 8 );
						for( int i = 0; i < handles.Length && _handleIndex < 0; i++ )
						{
							if( handles[i].Contains( e.Location ) )
								_handleIndex = i;
						}

						if( _handleIndex >= 0 )
							Cursor.Current = HANDLE_CURSORS[_handleIndex];*/
					}
				}
			}
			else if( EditorTool.Current == EditorTools.Vertex )
			{
				if( e.Button == MouseButtons.Left )
				{
					
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

					var min = new Point( Math.Min( _startPosition.X, _endPosition.X ), Math.Min( _startPosition.Y, _endPosition.Y ) );
					var max = new Point( Math.Max( _startPosition.X, _endPosition.X ), Math.Max( _startPosition.Y, _endPosition.Y ) );

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
					}
					else
					{
						var hadSelection = false;
						var minDepth = 99999;
						_selectedSolid = null;
						var minBounds = Rectangle.Empty;
						foreach( var solid in _level.Solids )
						{
							if( solid.Selected )
								hadSelection = true;

							solid.Selected = false;

							/*var bounds = solid.Project( _camera, _gridGap, _gridSize );

							var depth = (int)Math.Min( _camera.Direction.Dot( solid.Min ), _camera.Direction.Dot( solid.Max ) );
							if( depth < minDepth || _selectedSolid == null )
							{
								if( bounds.Contains( e.Location ) )
								{
									minDepth = depth;
									_selectedSolid = solid;
									minBounds = bounds;
								}
							}*/
						}

						if( _selectedSolid != null )
						{
							_selectedSolid.Selected = true;

							Invalidate();
							OnGlobalInvalidation?.Invoke();
						}
						else if( hadSelection )
						{
							Invalidate();
							OnGlobalInvalidation?.Invoke();
						}
					}
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
				/*var bounds = _selectedSolid.Project( _camera, _gridGap, _gridSize );
				var snapPosition = SnapToGrid( e.Location );
				
				_hoverPosition = snapPosition;
				
				if( snapPosition.X != bounds.X || snapPosition.Y != bounds.Y )
				{
					var min = new Point( bounds.Left, bounds.Top );
					var max = new Point( bounds.Right, bounds.Bottom );

					var newBounds = bounds;

					if( _handleIndex == 4 ) // center move tool
					{
						var halfWidth = newBounds.Width / 2;
						var halfHeight = newBounds.Height / 2;

						snapPosition = SnapToGrid( new Point( e.Location.X - halfWidth, e.Location.Y - halfHeight ) );
						newBounds = new Rectangle( snapPosition.X, snapPosition.Y, newBounds.Width, newBounds.Height );
					}
					else
					{
						if( _handleIndex % 3 == 0 ) // left column
						{
							var right = newBounds.Right;
							if( snapPosition.X < right )
							{
								newBounds.X = snapPosition.X;
								newBounds.Width = right - newBounds.X;
							}
						}
						else if( ( _handleIndex + 1 ) % 3 == 0 ) // right column
						{
							if( snapPosition.X > newBounds.X )
								newBounds.Width = snapPosition.X - newBounds.X;
						}

						if( _handleIndex / 3 == 0 ) // top row
						{
							var bottom = newBounds.Bottom;
							if( snapPosition.Y < bottom )
							{
								newBounds.Y = snapPosition.Y;
								newBounds.Height = bottom - newBounds.Y;
							}
						}
						else if( _handleIndex / 3 == 2 ) // bottom row
						{
							if( snapPosition.Y > newBounds.Y )
								newBounds.Height = snapPosition.Y - newBounds.Y;
						}
					}

					if(!newBounds.Equals(bounds))
						_selectedSolid.Unproject( _camera, newBounds, _gridGap, _gridSize );

					Invalidate();
				}

				Cursor.Current = HANDLE_CURSORS[_handleIndex];*/
			}
			else if( _spaceDown || _mmbDown )
			{
				var mouseDelta = new Point( e.X - _previousMousePosition.X, e.Y - _previousMousePosition.Y );
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
					var hadHover = false;
					var minDepth = 99999;
					GeometrySolid minSolid = null;
					GeometrySolid prevHover = null;
					foreach( var solid in _level.Solids )
					{
						if( solid.Hovered )
						{
							hadHover = true;
							prevHover = solid;
						}

						solid.Hovered = false;

						/*var bounds = solid.Project( _camera, _gridGap, _gridSize );
						
						var depth = (int)Math.Min( _camera.Direction.Dot( solid.Min ), _camera.Direction.Dot( solid.Max ) );
						if( depth < minDepth || minSolid == null )
						{
							if( bounds.Contains( e.Location ) )
							{
								minDepth = depth;
								minSolid = solid;
							}
						}*/
					}

					if( minSolid != null )
					{
						minSolid.Hovered = true;

						if( minSolid != prevHover )
						{
							Invalidate();
							OnGlobalInvalidation?.Invoke();
						}
					}
					else if( hadHover )
					{
						Invalidate();
						OnGlobalInvalidation?.Invoke();
					}

					// check interaction with handles
					if( _selectedSolid != null )
					{
						/*var bounds = _selectedSolid.Project( _camera, _gridGap, _gridSize );
						var handles = Extensions.GetHandles( bounds, 8 );

						var cursorChanged = false;
						for( int i = 0; i < handles.Length && !cursorChanged; i++ )
						{
							if( handles[i].Contains( e.Location ) )
							{
								Cursor.Current = HANDLE_CURSORS[i];
								cursorChanged = true;
							}
						}*/
					}
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

            // camera input
            var movement = new Point();
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

			if( e.KeyCode == Keys.Space )
				_spaceDown = true;

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
        }

        protected override void OnKeyUp( KeyEventArgs e )
        {
            base.OnKeyUp( e );

			if( e.KeyCode == Keys.Space )
				_spaceDown = false;
        }
    }
}
