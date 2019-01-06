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
		private List<FlatTabButtonControl> _tabButtons;
		private Level _level;
		private CommandStack _commandStack;
		
		private CommandSolidChanged _solidChangedCommand;

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
			btn_entity.Tag = EditorTools.Entity;

            _toolbarButtons = new List<FlatButtonControl>();
            _toolbarButtons.Add( btn_select );
            _toolbarButtons.Add( btn_solid );
			_toolbarButtons.Add( btn_clip );
			_toolbarButtons.Add( btn_vertex );
			_toolbarButtons.Add( btn_texture );
			_toolbarButtons.Add( btn_face );
			_toolbarButtons.Add( btn_entity );

			_tabButtons = new List<FlatTabButtonControl>();
			_tabButtons.Add( btn_tab_entity );
			_tabButtons.Add( btn_tab_face );

			view_3d.Level = _level;
			view_topRight.Level = _level;
			view_bottomLeft.Level = _level;
			view_bottomRight.Level = _level;

			view_topRight.CommandStack = _commandStack;
			view_bottomLeft.CommandStack = _commandStack;
			view_bottomRight.CommandStack = _commandStack;
			view_3d.CommandStack = _commandStack;

			view_topRight.OnGlobalInvalidation += ViewGlobalInvalidation;
			view_bottomLeft.OnGlobalInvalidation += ViewGlobalInvalidation;
			view_bottomRight.OnGlobalInvalidation += ViewGlobalInvalidation;
			view_3d.OnGlobalInvalidation += ViewGlobalInvalidation;

			//view_3d.OnFaceSelected += OnFaceSelected;

			tab_face.OnFaceMetricsChanged += OnFaceMetricsChanged;
		}

        private void toolbarButton_Click( object sender, EventArgs e )
        {
            foreach( var b in _toolbarButtons )
                b.Selected = false;

            var button = sender as FlatButtonControl;
            button.Selected = true;

            EditorTool.Current = (EditorTools)button.Tag;
			EditorTool.ClearSelection();

            Text = "FnuFF Editor - " + EditorTool.Current.ToString();

			ViewGlobalInvalidation();
        }

		private void tabButton_Click( object sender, EventArgs e )
		{
			foreach( var b in _tabButtons )
				b.Selected = false;

			var button = sender as FlatTabButtonControl;
			button.Selected = true;
		}

		private void btn_texture_Click( object sender, EventArgs e )
		{
			var selectedSolid = _level.Solids.FirstOrDefault( x => x.Selected );
			if( selectedSolid != null )
			{
				var command = new CommandSolidChanged( selectedSolid );
				command.Begin();

				foreach( var face in selectedSolid.Faces )
				{
					face.PackName = TextureMap.CurrentPackName;
					face.TextureName = TextureMap.CurrentTextureName;
				}

				command.End();
				if( command.HasChanges )
					_commandStack.Do( command );

				view_3d.Invalidate();
			}
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

		private void OnFaceSelected( GeometrySolid solid, Face face )
		{
			if( _solidChangedCommand != null )
			{
				_solidChangedCommand.End();

				if( _solidChangedCommand.HasChanges )
					_commandStack.Do( _solidChangedCommand );

				_solidChangedCommand = null;
			}

			if( solid != null )
			{
				_solidChangedCommand = new CommandSolidChanged( solid );
				_solidChangedCommand.Begin();
			}

			tab_face.SetFace( face );
		}

		private void OnFaceMetricsChanged()
		{
			view_3d.Invalidate();
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
			saveFileDialog.DefaultExt = ".lvl";
			saveFileDialog.Filter = "Level files|*.lvl|All files|*.*";

			if( saveFileDialog.ShowDialog() == DialogResult.OK )
			{
				var path = saveFileDialog.FileName;

				var solids = _level.Solids;
				var entities = _level.Entities;
				
				var stream = new FileStream( path, FileMode.Create, FileAccess.Write );
				var writer = new BinaryWriter( stream );
				
				writer.Write( solids.Count );
				writer.Write( entities.Count );

				writer.Write( TextureMap.Packs.Count );

				var packNames = new List<string>();
				foreach( var pack in TextureMap.Packs.Values )
				{
					packNames.Add( pack.Name );

					byte[] buffer = new byte[64];
					byte[] str = Encoding.Default.GetBytes( pack.Name );
					Array.Copy( str, buffer, str.Length );

					writer.Write( buffer );
				}

				var textureNames = new List<string>();
				foreach( var solid in solids )
				{
					foreach( var face in solid.Faces )
					{
						if( !textureNames.Contains( face.TextureName ) )
						{
							textureNames.Add( face.TextureName );
						}
					}
				}

				writer.Write( textureNames.Count );
				foreach( var name in textureNames )
				{
					byte[] buffer = new byte[64];
					byte[] str = Encoding.Default.GetBytes( name );
					Array.Copy( str, buffer, str.Length );

					writer.Write( buffer );
				}

				foreach( var solid in solids )
				{
					writer.Write( solid.Faces.Count );
					foreach( var face in solid.Faces )
					{
						writer.Write( face.Plane );
					}
					
					foreach( var face in solid.Faces )
					{
						// write texture information
						var textureIndex = -1;
						if( !string.IsNullOrEmpty( face.TextureName ) )
						{
							textureIndex = textureNames.IndexOf( face.TextureName );
						}
						
						writer.Write( textureIndex );

						// write vertex information
						var otherPlanes = solid.Faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();
						var points = Extensions.IntersectPlanes( face.Plane, otherPlanes );
						var indices = Extensions.WindingIndex3D( points, face.Plane.Normal );
						var texCoords = points.Select( x => Extensions.EmitTextureCoordinates( face.Plane.Normal, x, face ) ).ToArray();

						int indexCount = ( indices.Length - 2 ) * 3;
						writer.Write( indexCount );

						var v0 = points[indices[0]];
						var uv0 = texCoords[indices[0]];
						for( int i = 1; i < indices.Length-1; i++ )
						{
							var i1 = indices[i];
							var i2 = indices[i + 1];

							var v1 = points[i1];
							var v2 = points[i2];

							var uv1 = texCoords[i1];
							var uv2 = texCoords[i2];

							var v1v0 = v0 - v1;
							var v2v0 = v0 - v2;

							var n = v2v0.Cross( v1v0 );
							n.Normalize();

							var flip = face.Plane.Normal.Dot( n ) > 0;
							if( flip )
							{
								var temp = v2;
								v2 = v1;
								v1 = temp;

								var tempUV = uv2;
								uv2 = uv1;
								uv1 = tempUV;
							}

							writer.Write( v0 );
							writer.Write( uv0 );

							writer.Write( v1 );
							writer.Write( uv1 );

							writer.Write( v2 );
							writer.Write( uv2 );
						}
					}
				}

				foreach( var entity in entities )
				{
					writer.Write( entity.Position );
				}

				writer.Close();
				stream.Close();

				MessageBox.Show( "Level exported." );
			}
		}

		private void exitToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Close();
		}
	}
}
