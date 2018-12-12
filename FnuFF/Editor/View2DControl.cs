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

        private Camera2D _camera;

        private Brush _backgroundBrush;
        private Pen _gridPen;
        private float _gridSize;
        private int _gridGap;
        private int _gridSizeIndex;

        private Pen _solidPen;
        private Point _startPosition;
        private Point _endPosition;
        private bool _snapToGrid;
        private bool _lmbDown;
        private int _directionType;

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
            _gridSizeIndex = 2;
            _gridSize = GRID_SIZES[_gridSizeIndex];
            _gridGap = 64;

            _solidPen = new Pen( Color.White );

            _snapToGrid = true;

            Geometry.OnSolidChange += () => Invalidate();
        }

        private int SnapToGrid( int value )
        {
            var gap = _gridGap / _camera.Zoom;
            return (int)( Math.Round( value / gap ) * gap );
        }

        private float SnapToGrid( float value )
        {
            var gap = _gridGap / _camera.Zoom;
            return (float)Math.Round( value / gap ) * gap;
        }

        private Point SnapToGrid( Point value )
        {
            return new Point( SnapToGrid( value.X ), SnapToGrid( value.Y ) );
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            var g = e.Graphics;
            var rect = new Rectangle( 0, 0, Size.Width, Size.Height );

            // paint background
            g.FillRectangle( _backgroundBrush, rect );

            // paint grid
            var factor = _gridGap / _camera.Zoom;

            float x = (-_camera.Position.X / _camera.Zoom) % factor;
            while( x < Size.Width )
            {
                g.DrawLine( _gridPen, x, 0, x, Size.Height );
                x += factor;
            }

            float y = (-_camera.Position.Y / _camera.Zoom) % factor;
            while( y < Size.Height )
            {
                g.DrawLine( _gridPen, 0, y, Size.Width, y);
                y += factor;
            }

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
                var min = solid.Min;
                var max = solid.Max;

                var minxproj = ( min.Z / Math.Max( min.X, 1 ) ) * _camera.Direction.X + ( 1 - _camera.Direction.X );
                var minyproj = ( min.Z / Math.Max( min.Y, 1 ) ) * _camera.Direction.Y + ( 1 - _camera.Direction.Y );

                var maxxproj = ( max.Z / Math.Max( max.X, 1 ) ) * _camera.Direction.X + ( 1 - _camera.Direction.X );
                var maxyproj = ( max.Z / Math.Max( max.Y, 1 ) ) * _camera.Direction.Y + ( 1 - _camera.Direction.Y );

                var rectx = (int)( minxproj * min.X );
                var recty = (int)( minyproj * min.Y );
                var rectw = (int)( maxxproj * max.X ) - rectx;
                var recth = (int)( maxyproj * max.Y ) - recty;
                var srect = new Rectangle( rectx, recty, rectw, recth );

                g.DrawRectangle( _solidPen, srect );
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
                    _endPosition = SnapToGrid( _endPosition );

                _lmbDown = false;
                Invalidate();

                var min = new Point( Math.Min( _startPosition.X, _endPosition.X ), Math.Min( _startPosition.Y, _endPosition.Y ) );
                var max = new Point( Math.Max( _startPosition.X, _endPosition.X ), Math.Max( _startPosition.Y, _endPosition.Y ) );

                var minxproj = min.X - _camera.Direction.X * min.X;// - _camera.Direction.X * _gridGap;
                var minyproj = min.Y - _camera.Direction.Y * min.Y;// - _camera.Direction.Y * _gridGap;
                var minzproj = _camera.Direction.X * min.X + _camera.Direction.Y * min.Y;// - _camera.Direction.Z * _gridGap;

                var maxxproj = max.X - _camera.Direction.X * max.X + _camera.Direction.X * _gridGap;
                var maxyproj = max.Y - _camera.Direction.Y * max.Y + _camera.Direction.Y * _gridGap;
                var maxzproj = _camera.Direction.X * max.X + _camera.Direction.Y * max.Y + _camera.Direction.Z * _gridGap;

                var minTriple = new Triple( minxproj, minyproj, minzproj );
                var maxTriple = new Triple( maxxproj, maxyproj, maxzproj );

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

            if( _lmbDown )
            {
                _endPosition = e.Location;

                if( _snapToGrid )
                {
                    _endPosition = SnapToGrid( _endPosition );
                }

                Invalidate();
            }
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
        }
    }
}
