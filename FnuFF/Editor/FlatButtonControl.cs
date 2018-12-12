using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Editor
{
    public class FlatButtonControl : Button
    {
        private const int INDICATOR_WIDTH = 8;

        private bool _pressed;
        private bool _hovered;
        private bool _selected;

        private Brush _pressedBrush;
        private Brush _hoveredBrush;
        private Brush _selectedBrush;
        private Brush _selectedPressedBrush;
        private Brush _selectedHoveredBrush;

        public bool Pressed { get { return _pressed; } set { _pressed = value; } }
        public bool Hovered { get { return _hovered; } set { _hovered = value; } }
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                Invalidate();
            }
        }

        public FlatButtonControl()
        {
            _pressedBrush = new SolidBrush( EditorColors.PRESSED );
            _hoveredBrush = new SolidBrush( EditorColors.HOVERED );
            _selectedBrush = new SolidBrush( EditorColors.SELECTED );
            _selectedPressedBrush = new SolidBrush( EditorColors.SELECTED_PRESSED );
            _selectedHoveredBrush = new SolidBrush( EditorColors.SELECTED_HOVERED );
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            var rect = new Rectangle( 0, 0, Size.Width, Size.Height );

            using( var backgroundBrush = new SolidBrush( BackColor ) )
                e.Graphics.FillRectangle( backgroundBrush, rect );

            // draw background image
            if( BackgroundImage != null )
                e.Graphics.DrawImage( BackgroundImage, rect );

            // draw image
            if( Image != null )
            {
                var offset = new Size( ( Size.Width - Image.Width ) / 2, ( Size.Height - Image.Height ) / 2 );
                var imageRect = new Rectangle( rect.X + offset.Width, rect.Y + offset.Height, Image.Width, Image.Height );

                e.Graphics.DrawImage( Image, imageRect );
            }
            else
                e.Graphics.FillRectangle( Brushes.White, rect );

            // draw selection/hover indicator
            if( _pressed || _hovered || _selected )
            {
                var brush = _selectedBrush;
                if( _selected )
                {
                    if( _pressed )
                        brush = _selectedPressedBrush;
                    else if( _hovered )
                        brush = _selectedHoveredBrush;
                }
                else
                {
                    if( _pressed )
                        brush = _pressedBrush;
                    else if( _hovered )
                        brush = _hoveredBrush;
                }

                e.Graphics.FillRectangle( brush, new Rectangle( 0, 0, INDICATOR_WIDTH, Size.Height ) );
            }

            if( DesignMode )
            {
                using( var pen = new Pen( Color.DarkGray ) )
                {
                    pen.DashPattern = new float[] { 3.0f, 3.0f };
                    e.Graphics.DrawRectangle( pen, new Rectangle( 0, 0, Size.Width - 1, Size.Height - 1 ) );
                }
            }
        }

        protected override void OnMouseEnter( EventArgs e )
        {
            base.OnMouseEnter( e );

            _hovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave( EventArgs e )
        {
            base.OnMouseLeave( e );

            _hovered = false;
            Invalidate();
        }

        protected override void OnClick( EventArgs e )
        {
            base.OnClick( e );

            _selected = true;
            Invalidate();
        }

        protected override void OnMouseDown( MouseEventArgs mevent )
        {
            base.OnMouseDown( mevent );

            _pressed = true;
            Invalidate();
        }

        protected override void OnMouseUp( MouseEventArgs mevent )
        {
            base.OnMouseUp( mevent );

            _pressed = false;
            Invalidate();
        }
    }
}
