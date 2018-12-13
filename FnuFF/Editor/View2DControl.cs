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

namespace Editor
{
    public class View2DControl : Control
    {
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

        private static float[] GRID_SIZES = new float[]{ 0.1f, 0.5f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 8.0f, 10.0f, 15.0f, 20.0f };
		private const int GRID_MAX_LINES = 100;

        private Camera2D _camera;

        private Brush _backgroundBrush;
        private Pen _gridPen;
		private Pen _gridHighlightPen;
        private float _gridSize;
        private int _gridGap;
        private int _gridSizeIndex;

        private Pen _solidPen;
        private Point _startPosition;
        private Point _endPosition;
        private bool _snapToGrid;
        private bool _lmbDown;
		private bool _spaceDown;
        private int _directionType;
		private Point _hoverPosition;

		private Point _previousMousePosition;

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

        public View2DControl()
        {
            DoubleBuffered = true;

            _camera = new Camera2D();

            _backgroundBrush = new SolidBrush( EditorColors.BACKGROUND_LOW );
            _gridPen = new Pen( EditorColors.GRID );
			_gridHighlightPen = new Pen( EditorColors.GRID_HIGHLIGHT );
            _gridSizeIndex = 2;
            _gridSize = GRID_SIZES[_gridSizeIndex];
            _gridGap = 64;

            _solidPen = new Pen( Color.White );

            _snapToGrid = true;

            Geometry.OnSolidChange += () => Invalidate();
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

			Log.AddFunctor( Name, () => "Camera: " + _camera.Position.ToString() );
			Log.AddFunctor( Name, () => "Hover: " + _hoverPosition.ToString() );
		}

		protected override void OnPaint( PaintEventArgs e )
        {
            var g = e.Graphics;
            var rect = new Rectangle( 0, 0, Size.Width, Size.Height );

            // paint background
            g.FillRectangle( _backgroundBrush, rect );

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

			// paint origo
			var origo = _camera.ToLocal( Point.Empty );
			var origoExtent = _gridGap * 3;
			g.DrawLine( _gridHighlightPen, _camera.ToLocal(new Point(0, -origoExtent)), _camera.ToLocal(new Point(0, origoExtent)) );
			g.DrawLine( _gridHighlightPen, _camera.ToLocal(new Point(-origoExtent, 0)), _camera.ToLocal(new Point(origoExtent, 0)) );

            // paint outline of new solid
            if( _lmbDown )
            {
                var minPoint = new Point( Math.Min( _startPosition.X, _endPosition.X ), Math.Min( _startPosition.Y, _endPosition.Y ) );
                var maxPoint = new Point( Math.Max( _startPosition.X, _endPosition.X ), Math.Max( _startPosition.Y, _endPosition.Y ) );
                var solidRect = new Rectangle( minPoint, new Size( maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y ) );
                g.DrawRectangle( _solidPen, solidRect );
            }

            // paint solids
            foreach( var solid in Geometry.Solids )
            {
				var min = _camera.ToLocal(_camera.Project( solid.Min ));
				var max = _camera.ToLocal(_camera.Project( solid.Max ));

				var srect = new Rectangle( min.X, min.Y, max.X - min.X, max.Y - min.Y );

                g.DrawRectangle( _solidPen, srect );
            }

			// (DEBUG) paint hover position
			using( var pen = new Pen( Color.Blue ) )
			{
				g.DrawLine( pen, new Point( _hoverPosition.X - 4, _hoverPosition.Y ), new Point( _hoverPosition.X + 4, _hoverPosition.Y ) );
				g.DrawLine( pen, new Point( _hoverPosition.X, _hoverPosition.Y - 4 ), new Point( _hoverPosition.X, _hoverPosition.Y + 4 ) );
			}

			// (DEBUG) paint log
			Log.Paint( Name, g );
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
            }
        }

        protected override void OnMouseUp( MouseEventArgs e )
        {
            base.OnMouseUp( e );

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

				var minTriple = _camera.Unproject( _camera.ToGlobal( min ), -32 );
				var maxTriple = _camera.Unproject( _camera.ToGlobal( max ), 32 );

                var solid = new GeometrySolid( minTriple, maxTriple );
                Geometry.AddSolid( solid );
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

			if( _lmbDown )
			{
				_endPosition = e.Location;

				if( _snapToGrid )
				{
					_endPosition = SnapToGrid( _endPosition );
				}

				Invalidate();
			}
			else if( _spaceDown )
			{
				var mouseDelta = new Point( e.X - _previousMousePosition.X, e.Y - _previousMousePosition.Y );
				_camera.Move( mouseDelta.X, mouseDelta.Y );
			}

			_hoverPosition = e.Location;

			if( _snapToGrid )
			{
				_hoverPosition = _camera.ToGlobal( _hoverPosition );
				_hoverPosition = _camera.Snap( _gridGap, _hoverPosition );
				_hoverPosition = _camera.ToLocal( _hoverPosition );
			}

			Invalidate();

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

                Invalidate();
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
