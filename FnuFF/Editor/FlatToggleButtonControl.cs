using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Editor
{
	public class FlatToggleButtonControl : Button
	{
		private bool _pressed;
		private bool _hovered;
		private bool _selected;

		private Brush _pressedBrush;
		private Brush _hoveredBrush;

		private Image _selectedImage;

		public bool Pressed => _pressed;
		public bool Hovered => _hovered;
		public bool Selected
		{
			get { return _selected; }
			set
			{
				_selected = value;
				Invalidate();
			}
		}

		public Image SelectedImage
		{
			get { return _selectedImage; }
			set
			{
				_selectedImage = value;
				if( _selected )
					Invalidate();
			}
		}

		public FlatToggleButtonControl()
		{
			_pressed = false;
			_hovered = false;
			_selected = false;

			_pressedBrush = new SolidBrush( Color.FromArgb( 64, EditorColors.PRESSED ) );
			_hoveredBrush = new SolidBrush( Color.FromArgb( 64, EditorColors.HOVERED ) );
		}

		protected override void OnPaint( PaintEventArgs pevent )
		{
			var g = pevent.Graphics;

			var rect = new Rectangle( 0, 0, Size.Width, Size.Height );

			using( var backgroundBrush = new SolidBrush( BackColor ) )
				g.FillRectangle( backgroundBrush, rect );

			// draw image
			if( _selected )
			{
				if( _selectedImage != null )
				{
					var offset = new Size( ( Size.Width - _selectedImage.Width ) / 2, ( Size.Height - _selectedImage.Height ) / 2 );
					var imageRect = new Rectangle( rect.X + offset.Width, rect.Y + offset.Height, _selectedImage.Width, _selectedImage.Height );

					g.DrawImage( _selectedImage, imageRect );
				}
				else
				{
					g.FillRectangle( Brushes.White, rect );
				}
			}
			else
			{
				if( Image != null )
				{
					var offset = new Size( ( Size.Width - Image.Width ) / 2, ( Size.Height - Image.Height ) / 2 );
					var imageRect = new Rectangle( rect.X + offset.Width, rect.Y + offset.Height, Image.Width, Image.Height );

					g.DrawImage( Image, imageRect );
				}
				else
				{
					g.FillRectangle( Brushes.White, rect );
				}
			}

			// draw press/hover indicator
			if( _pressed || _hovered )
			{
				var brush = _hoveredBrush;
				if( _pressed )
				{
					brush = _pressedBrush;
				}

				g.FillRectangle( brush, rect );
			}

			if( DesignMode )
			{
				using( var pen = new Pen( Color.DarkGray ) )
				{
					pen.DashPattern = new float[] { 3.0f, 3.0f };
					g.DrawRectangle( pen, new Rectangle( 0, 0, Size.Width - 1, Size.Height - 1 ) );
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
			_hovered = false;

			base.OnMouseLeave( e );
			
			Invalidate();
		}

		protected override void OnClick( EventArgs e )
		{
			_selected = !_selected;

			base.OnClick( e );

			Invalidate();
		}

		protected override void OnMouseDown( MouseEventArgs mevent )
		{
			_pressed = true;

			base.OnMouseDown( mevent );
			
			Invalidate();
		}

		protected override void OnMouseUp( MouseEventArgs mevent )
		{
			_pressed = false;

			base.OnMouseUp( mevent );
			
			Invalidate();
		}
	}
}
