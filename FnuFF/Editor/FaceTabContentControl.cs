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
		private string _texturePath;
		private string _textureName;

		public string TextureName => _textureName;

		public string TexturePath
		{
			get { return _texturePath; }
		}

		public FaceTabContentControl()
		{
			InitializeComponent();
		}

		private void btn_browse_Click( object sender, EventArgs e )
		{
			openFileDialog.Title = "Load texture";
			openFileDialog.DefaultExt = ".tga";
			openFileDialog.Filter = "Targa Files|*.tga|All Files|*.*";

			if( openFileDialog.ShowDialog() == DialogResult.OK )
			{
				string filename = openFileDialog.FileName;

				_texturePath = filename;
				LoadPreview();
			}
		}

		private void LoadPreview()
		{
			if( File.Exists( _texturePath ) )
			{
				/*var name = TextureMap.LoadTexture( _texturePath );
				if( !string.IsNullOrEmpty( name ) )
				{
					TextureMap.SetCurrent( name );
					pb_texture.Image = TextureMap.GetTarga( name ).ToImage();
				}*/
			}
		}
	}
}
