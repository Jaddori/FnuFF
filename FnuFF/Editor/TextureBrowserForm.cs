using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor
{
	public partial class TextureBrowserForm : Form
	{
		private const int PREVIEW_BOX_SIZE = 128;
		private static readonly TimeSpan DOUBLE_CLICK_THRESHOLD = TimeSpan.FromMilliseconds( 500 );

		private class PreviewBoxInfo
		{
			public string PackName;
			public string TextureName;
			public DateTime PreviousClick;
		}

		private bool _initialized;
		private List<PictureBox> _previewBoxes;
		private string _filterPack;
		private string _filterName;

		public TextureBrowserForm()
		{
			InitializeComponent();
			_initialized = false;
			_previewBoxes = new List<PictureBox>();

			_filterPack = string.Empty;
			_filterName = string.Empty;
		}

		private void TextureBrowserForm_Load( object sender, EventArgs e )
		{
			if( !_initialized )
			{
				cmb_pack.Items.Add( "(All)" );
				cmb_pack.SelectedIndex = 0;

				var packs = TextureMap.Packs.Values;
				foreach( var pack in packs )
				{
					var names = pack.Names;
					var targas = pack.Targas;
					
					for( int i=0; i<targas.Length; i++ )
					{
						var pb = new TexturePreviewControl();
						pb.Size = new Size( PREVIEW_BOX_SIZE, PREVIEW_BOX_SIZE );
						pb.Image = targas[i].ToImage();
						pb.SizeMode = PictureBoxSizeMode.StretchImage;
						pb.Click += pictureBox_Click;
						pb.Font = Font;
						pb.Name = names[i];
						
						pb.Tag = new PreviewBoxInfo
						{
							PackName = pack.Name,
							TextureName = names[i],
							PreviousClick = DateTime.MinValue
						};

						_previewBoxes.Add( pb );
						flow_preview.Controls.Add( pb );
					}

					cmb_pack.Items.Add( pack.Name );
				}

				_initialized = true;
			}
		}

		private void pictureBox_Click( object sender, EventArgs e )
		{
			var pb = sender as PictureBox;
			var info = pb.Tag as PreviewBoxInfo;

			var timeSinceLastClick = DateTime.Now - info.PreviousClick;
			var seconds = timeSinceLastClick.Seconds;

			if( timeSinceLastClick < DOUBLE_CLICK_THRESHOLD ) // double click
			{
				TextureMap.CurrentPackName = info.PackName;
				TextureMap.CurrentTextureName = info.TextureName;

				Close();
			}
			else // single click
			{
				foreach( var box in _previewBoxes )
				{
					box.BorderStyle = BorderStyle.None;
					box.Padding = new Padding( 0 );
				}

				pb.BorderStyle = BorderStyle.FixedSingle;
				pb.Padding = new Padding( 4 );
			}

			info.PreviousClick = DateTime.Now;
		}

		private void cmb_pack_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( cmb_pack.SelectedIndex == 0 )
				_filterPack = string.Empty;
			else
				_filterPack = cmb_pack.SelectedItem as string;

			UpdateFilter();
		}

		private void txt_name_TextChanged( object sender, EventArgs e )
		{
			_filterName = txt_name.Text;

			UpdateFilter();
		}

		private void UpdateFilter()
		{
			foreach( var box in _previewBoxes )
			{
				var info = box.Tag as PreviewBoxInfo;
				//box.Visible = ( info.PackName == _filterPack );
				box.Visible = true;

				if( !string.IsNullOrEmpty( _filterPack ) )
				{
					if( info.PackName != _filterPack )
						box.Visible = false;
				}

				if( !string.IsNullOrEmpty( _filterName ) )
				{
					if( !info.TextureName.Contains( _filterName ) )
						box.Visible = false;
				}
			}
		}
	}
}
