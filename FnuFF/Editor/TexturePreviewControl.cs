using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Editor
{
	public class TexturePreviewControl : PictureBox
	{
		private string _name;
		private Font _font;

		public string Name { get { return _name; } set { _name = value; } }
		public Font Font { get { return _font; } set { _font = value; } }

		public TexturePreviewControl()
		{
			_name = string.Empty;
		}

		protected override void OnPaint( PaintEventArgs pe )
		{
			base.OnPaint( pe );

			var g = pe.Graphics;
			var textSize = g.MeasureString( _name, _font );
			var height = (int)( textSize.Height + 0.5f );

			var bounds = new Rectangle( 0, Size.Height - height-3, Size.Width, height+3 );
			var format = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			g.DrawString( _name, _font, Brushes.Black, bounds, format );

			bounds.Offset( 1, 1 );
			g.DrawString( _name, _font, Brushes.White, bounds, format );
		}
	}
}
