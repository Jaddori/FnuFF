﻿using System;
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

namespace Editor
{
    public partial class EditorForm : Form
    {
        private List<FlatButtonControl> _toolbarButtons;
		private Level _level;

        public EditorForm()
        {
            InitializeComponent();
        }

        private void Form1_Load( object sender, EventArgs e )
        {
			_level = new Level();

            btn_select.Tag = EditorTools.Select;
            btn_solid.Tag = EditorTools.Solid;

            _toolbarButtons = new List<FlatButtonControl>();
            _toolbarButtons.Add( btn_select );
            _toolbarButtons.Add( btn_solid );

			view_topRight.Level = _level;
			view_bottomLeft.Level = _level;
			view_bottomRight.Level = _level;
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
