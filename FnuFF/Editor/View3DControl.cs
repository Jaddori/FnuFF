using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.ComponentModel;

namespace Editor
{
    public class View3DControl : Control
    {
		private Level _level;
		private int _frame;

		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Level Level
		{
			get { return _level; }
			set
			{
				_level = value;
				_level.OnSolidChange += () => Invalidate();
			}
		}

        public View3DControl()
        {
			//DoubleBuffered = true;
        }

		protected override void OnCreateControl()
		{
			if(!DesignMode)
				GL.CreateContext( Handle, Size.Width, Size.Height );

			_frame = 0;
			Log.AddFunctor( Name, () => "Frame: " + _frame.ToString() );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			//base.OnPaint( e );

			if( !DesignMode )
			{
				GL.ClearColor( 0.0f, 0.0f, 0.0f, 0.0f );

				foreach( var solid in _level.Solids )
				{
					solid.Paint3D();
				}

				GL.SwapBuffers( Handle );
				_frame++;
			}
		}
	}
}
