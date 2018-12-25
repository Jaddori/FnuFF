using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Editor.UndoRedo;

namespace Editor
{
    public partial class EditorForm : Form
    {
        private List<FlatButtonControl> _toolbarButtons;
		private Level _level;
		private CommandStack _commandStack;

        public EditorForm()
        {
            InitializeComponent();
        }

        private void Form1_Load( object sender, EventArgs e )
        {
			_commandStack = new CommandStack();
			GeometrySolid.CommandStack = _commandStack;

			_level = new Level();

            btn_select.Tag = EditorTools.Select;
            btn_solid.Tag = EditorTools.Solid;
			btn_clip.Tag = EditorTools.Clip;
			btn_vertex.Tag = EditorTools.Vertex;
			btn_texture.Tag = EditorTools.Texture;
			btn_face.Tag = EditorTools.Face;

            _toolbarButtons = new List<FlatButtonControl>();
            _toolbarButtons.Add( btn_select );
            _toolbarButtons.Add( btn_solid );
			_toolbarButtons.Add( btn_clip );
			_toolbarButtons.Add( btn_vertex );
			_toolbarButtons.Add( btn_texture );
			_toolbarButtons.Add( btn_face );

			view_3d.Level = _level;
			view_topRight.Level = _level;
			view_bottomLeft.Level = _level;
			view_bottomRight.Level = _level;

			view_topRight.CommandStack = _commandStack;
			view_bottomLeft.CommandStack = _commandStack;
			view_bottomRight.CommandStack = _commandStack;

			view_topRight.OnGlobalInvalidation += ViewGlobalInvalidation;
			view_bottomLeft.OnGlobalInvalidation += ViewGlobalInvalidation;
			view_bottomRight.OnGlobalInvalidation += ViewGlobalInvalidation;
		}

        private void toolbarButton_Click( object sender, EventArgs e )
        {
            foreach( var b in _toolbarButtons )
                b.Selected = false;

            var button = sender as FlatButtonControl;
            button.Selected = true;

            EditorTool.Current = (EditorTools)button.Tag;

            Text = "FnuFF Editor - " + EditorTool.Current.ToString();
        }

		private void newToolStripMenuItem_Click( object sender, EventArgs e )
		{
			_level.Reset();
		}

		private void ViewGlobalInvalidation()
		{
			view_3d.Invalidate();
			view_topRight.Invalidate();
			view_bottomLeft.Invalidate();
			view_bottomRight.Invalidate();
		}

		private void loadToolStripMenuItem_Click( object sender, EventArgs e )
		{
			openFileDialog.Filter = "XML Files|*.xml|All files|*.*";

			if( openFileDialog.ShowDialog() == DialogResult.OK )
			{
				var path = openFileDialog.FileName;

				var success = false;

				var ser = new XmlSerializer( typeof( Level ) );
				using( var stream = new FileStream( path, FileMode.Open, FileAccess.Read ) )
				{
					using( var reader = XmlReader.Create( stream ) )
					{
						if( ser.CanDeserialize( reader ) )
						{
							_level = (Level)ser.Deserialize( reader );
							success = true;
						}
					}
				}

				if( success )
				{
					view_3d.Level = _level;
					view_topRight.Level = _level;
					view_bottomLeft.Level = _level;
					view_bottomRight.Level = _level;

					MessageBox.Show( "Level loaded" );
				}
				else
					MessageBox.Show( "Failed to load level." );
			}
		}

		private void saveToolStripMenuItem_Click( object sender, EventArgs e )
		{
			saveFileDialog.DefaultExt = ".xml";
			saveFileDialog.Filter = "XML files|*.xml|All files|*.*";

			if( saveFileDialog.ShowDialog() == DialogResult.OK )
			{
				var path = saveFileDialog.FileName;

				var ser = new XmlSerializer( typeof( Level ) );
				using( var stream = new FileStream( path, FileMode.Create, FileAccess.Write ) )
				{
					var settings = new XmlWriterSettings();
					settings.Indent = true;
					settings.IndentChars = "\t";
					settings.OmitXmlDeclaration = false;

					using( var writer = XmlWriter.Create( stream, settings ) )
					{
						ser.Serialize( writer, _level );
					}
				}

				MessageBox.Show( "Level saved." );
			}
		}

		private void saveAsToolStripMenuItem_Click( object sender, EventArgs e )
		{

		}

		private void exportToolStripMenuItem_Click( object sender, EventArgs e )
		{

		}

		private void exitToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Close();
		}
	}
}
