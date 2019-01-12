using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Editor
{
	public class FlatTabButtonControl : Button
	{
		private const int INDICATOR_HEIGHT = 4;

		private bool _pressed;
		private bool _hovered;
		private bool _selected;

		private Brush _pressedBrush;
		private Brush _hoveredBrush;
		private Brush _selectedBrush;
		private Brush _selectedPressedBrush;
		private Brush _selectedHoveredBrush;
		private Brush _textBrush;

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

		public FlatTabButtonControl()
		{
			_pressedBrush = new SolidBrush( EditorColors.PRESSED );
			_hoveredBrush = new SolidBrush( EditorColors.HOVERED );
			_selectedBrush = new SolidBrush( EditorColors.SELECTED );
			_selectedPressedBrush = new SolidBrush( EditorColors.SELECTED_PRESSED );
			_selectedHoveredBrush = new SolidBrush( EditorColors.SELECTED_HOVERED );
			_textBrush = new SolidBrush( Color.White );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			var rect = new Rectangle( 0, 0, Size.Width, Size.Height );

			// draw background color
			using( var backgroundBrush = new SolidBrush( BackColor ) )
				e.Graphics.FillRectangle( backgroundBrush, rect );

			// draw text
			var format = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			e.Graphics.DrawString( Text, Font, _textBrush, rect, format );

			// draw selection/hover indicator
			if( _pressed || _hovered  )
			{
				var brush = _selectedBrush;
				/*if( _selected )
				{
					if( _pressed )
						brush = _selectedPressedBrush;
					else if( _hovered )
						brush = _selectedHoveredBrush;
				}
				else*/
				{
					if( _pressed )
						brush = _pressedBrush;
					else if( _hovered )
						brush = _hoveredBrush;
				}

				e.Graphics.FillRectangle( brush, new Rectangle( 0, Size.Height-INDICATOR_HEIGHT, Size.Width, INDICATOR_HEIGHT ) );
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
