using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Editor
{
	public partial class FaceTabContentControl : UserControl
	{
		public delegate void MetricChangedHandler();
		public event MetricChangedHandler OnFaceMetricsChanged;

		private Face _face;
		private bool _silent;

		public FaceTabContentControl()
		{
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			EditorTool.OnSelectedFaceChanged += ( prev, cur ) => SetFace( cur );
			SetFace( null );
		}

		private void btn_browse_Click( object sender, EventArgs e )
		{
			var tbf = new TextureBrowserForm();
			tbf.ShowDialog();

			txt_textureName.Text = TextureMap.CurrentPackName + "/" + TextureMap.CurrentTextureName;

			var targa = TextureMap.GetCurrentTarga();
			if( targa != null )
				pb_texture.Image = targa.ToImage();
		}

		private void FaceTabContentControl_Resize( object sender, EventArgs e )
		{
			var width = Size.Width;

			var numWidth = ( width - 3 * 6 ) / 2;
			var numHeight = num_offsetx.Size.Height;

			num_offsetx.Size = new Size( numWidth, numHeight );
			num_offsety.Size = new Size( numWidth, numHeight );
			num_scalex.Size = new Size( numWidth, numHeight );
			num_scaley.Size = new Size( numWidth, numHeight );

			num_offsety.Location = new Point( num_offsetx.Location.X + numWidth + 8, num_offsetx.Location.Y );
			num_scaley.Location = new Point( num_scalex.Location.X + numWidth + 8, num_scalex.Location.Y );
		}

		public void SetFace( Face face )
		{
			_face = face;
			if( _face != null )
			{
				_silent = true;
				num_offsetx.Value = (decimal)_face.Offset.X;
				num_offsety.Value = (decimal)_face.Offset.Y;
				num_scalex.Value = (decimal)_face.Scale.X;
				num_scaley.Value = (decimal)_face.Scale.Y;
				num_rotation.Value = (decimal)_face.Rotation;

				num_offsetx.Enabled = true;
				num_offsety.Enabled = true;
				num_scalex.Enabled = true;
				num_scaley.Enabled = true;
				num_rotation.Enabled = true;
				_silent = false;
			}
			else
			{
				num_offsetx.Value = 0;
				num_offsety.Value = 0;
				num_scalex.Value = 0;
				num_scaley.Value = 0;
				num_rotation.Value = 0;

				num_offsetx.Enabled = false;
				num_offsety.Enabled = false;
				num_scalex.Enabled = false;
				num_scaley.Enabled = false;
				num_rotation.Enabled = false;
			}
		}

		public PointF GetOffset() { return new PointF( (float)num_offsetx.Value, (float)num_offsety.Value ); }
		public PointF GetScale() { return new PointF( (float)num_scalex.Value, (float)num_scaley.Value ); }
		public float GetRotation() { return (float)num_rotation.Value; }

		private void num_offsetx_ValueChanged( object sender, EventArgs e )
		{
			if( _silent )
				return;

			if( _face != null )
			{
				_face.Offset = GetOffset();
				OnFaceMetricsChanged?.Invoke();
			}
		}

		private void num_offsety_ValueChanged( object sender, EventArgs e )
		{
			if( _silent )
				return;

			if( _face != null )
			{
				_face.Offset = GetOffset();
				OnFaceMetricsChanged?.Invoke();
			}
		}

		private void num_scalex_ValueChanged( object sender, EventArgs e )
		{
			if( _silent )
				return;

			if( _face != null )
			{
				_face.Scale = GetScale();
				OnFaceMetricsChanged?.Invoke();
			}
		}

		private void num_scaley_ValueChanged( object sender, EventArgs e )
		{
			if( _silent )
				return;

			if( _face != null )
			{
				_face.Scale = GetScale();
				OnFaceMetricsChanged?.Invoke();
			}
		}

		private void num_rotation_ValueChanged( object sender, EventArgs e )
		{
			if( _silent )
				return;

			if( num_rotation.Value < 0 )
				num_rotation.Value = 360;
			else if( num_rotation.Value > 360 )
				num_rotation.Value = 0;

			if( _face != null )
			{
				_face.Rotation = GetRotation();
				OnFaceMetricsChanged?.Invoke();
			}
		}

		public void SetDefaultTexture()
		{
			txt_textureName.Text = TextureMap.CurrentPackName + "/" + TextureMap.CurrentTextureName;

			var targa = TextureMap.GetCurrentTarga();
			if( targa != null )
				pb_texture.Image = targa.ToImage();
		}
	}
}
