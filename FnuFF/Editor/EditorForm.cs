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
		private const string LEVEL_NAME_UNNAMED = "Unnamed";

		private List<FlatButtonControl> _toolbarButtons;
		private List<FlatTabButtonControl> _tabButtons;
		private Level _level;
		private CommandStack _commandStack;

		private CommandSolidChanged _solidChangedCommand;

		private int _lastSaveCommandIndex;
		private string _levelPath;
		private string _levelName;

		public EditorForm()
		{
			InitializeComponent();
		}

		private void Form1_Load( object sender, EventArgs e )
		{
			_commandStack = new CommandStack();
			Solid.CommandStack = _commandStack;

			_commandStack.OnDo += ( command ) => UpdateTitle();
			_commandStack.OnUndo += ( command ) => UpdateTitle();
			_commandStack.OnRedo += ( command ) => UpdateTitle();

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

			btn_tab_entity.Tag = tab_entity;
			btn_tab_face.Tag = tab_face;

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
			tab_face.SetDefaultTexture();

			cntl_commandHistory.CommandStack = _commandStack;

			_lastSaveCommandIndex = _commandStack.Index;
			_levelName = LEVEL_NAME_UNNAMED;

			UpdateTitle();
		}

		private void toolbarButton_Click( object sender, EventArgs e )
		{
			foreach( var b in _toolbarButtons )
				b.Selected = false;

			var button = sender as FlatButtonControl;
			button.Selected = true;

			var prevTool = EditorTool.Current;
			EditorTool.Current = (EditorTools)button.Tag;

			if( EditorTool.Current != EditorTools.Clip && EditorTool.Current != EditorTools.Vertex )
			{
				if( !(prevTool == EditorTools.Clip && EditorTool.Current == EditorTools.Select) &&
					!(prevTool == EditorTools.Vertex && EditorTool.Current == EditorTools.Select) )
					EditorTool.ClearSelection();
			}

			//Text = "FnuFF Editor - " + EditorTool.Current.ToString();

			ViewGlobalInvalidation();
		}

		private void tabButton_Click( object sender, EventArgs e )
		{
			foreach( var b in _tabButtons )
			{
				b.Selected = false;
				b.BackColor = EditorColors.BACKGROUND_HIGH;

				if( b.Tag != null )
				{
					var c = b.Tag as UserControl;
					c.Visible = false;
				}
			}

			var button = sender as FlatTabButtonControl;
			button.Selected = true;
			button.BackColor = EditorColors.BACKGROUND_LOW;

			if( button.Tag != null )
			{
				var tabControl = button.Tag as UserControl;
				tabControl.Visible = true;
			}
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
			_commandStack.Reset();

			_lastSaveCommandIndex = _commandStack.Index;
			_levelPath = string.Empty;
			_levelName = LEVEL_NAME_UNNAMED;

			UpdateTitle();
		}

		private void ViewGlobalInvalidation()
		{
			view_3d.Invalidate();
			view_topRight.Invalidate();
			view_bottomLeft.Invalidate();
			view_bottomRight.Invalidate();
		}

		private void OnFaceSelected( Solid solid, Face face )
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

		private void LoadLevel( string path )
		{
			var success = false;

			var ser = new XmlSerializer( typeof( Level ) );
			using( var stream = new FileStream( path, FileMode.Open, FileAccess.Read ) )
			{
				using( var reader = XmlReader.Create( stream ) )
				{
					if( ser.CanDeserialize( reader ) )
					{
						_level = (Level)ser.Deserialize( reader );

						foreach( var solid in _level.Solids )
						{
							foreach( var face in solid.Faces )
							{
								face.BuildVertices( solid );
							}
						}

						success = true;
					}
				}
			}

			if( success )
			{
				EditorTool.Current = EditorTools.Select;

				_levelPath = path;
				_levelName = _levelPath.NameFromPath();

				view_3d.Level = _level;
				view_topRight.Level = _level;
				view_bottomLeft.Level = _level;
				view_bottomRight.Level = _level;
			}
			else
				MessageBox.Show( "Failed to load level." );
		}

		private void Level_Load()
		{
			openFileDialog.Filter = "XML Files|*.xml|All files|*.*";

			if( openFileDialog.ShowDialog() == DialogResult.OK )
			{
				var path = openFileDialog.FileName;

				LoadLevel( path );
			}
		}
		

		private void Level_Save()
		{
			var hasPath = !string.IsNullOrEmpty( _levelPath );

			if( !hasPath )
				hasPath = PromptLevelPath();

			if( hasPath )
				SaveLevel();
		}

		private void Level_SaveAs()
		{
			if( PromptLevelPath() )
				SaveLevel();
		}

		private void ExportLevel( string path )
		{
			//GenerateLightmap( path + "_light.tga" );
			var lightmapForm = new LightmapForm( _level );
			lightmapForm.ShowDialog();

			Lightmap.Upload( path + "_light.tga" );

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
					writer.Write( face.Plane, Grid.SIZE_BASE );
				}

				var visibleFaceCount = solid.Faces.Count( x => x.PackName != "tools" );
				writer.Write( visibleFaceCount );

				foreach( var face in solid.Faces )
				{
					if( face.PackName == "tools" )
						continue;

					// write texture information
					var textureIndex = -1;
					if( !string.IsNullOrEmpty( face.TextureName ) )
					{
						textureIndex = textureNames.IndexOf( face.TextureName );
					}

					writer.Write( textureIndex );

					// write vertex information
					var otherPlanes = solid.Faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();

					var vertices = face.Vertices;
					var uvs = face.UVs;
					var lms = face.LightmapUVs;

					var indexCount = ( vertices.Count - 2 ) * 3;
					writer.Write( indexCount );

					var v0 = vertices[0];
					var uv0 = uvs[0];
					var lm0 = new PointF( Lightmap.SIZE * 0.5f, Lightmap.SIZE * 0.5f );
					if( lms.Count > 0 )
						lm0 = lms[0];

					for( int i = 1; i < vertices.Count - 1; i++ )
					{
						var v1 = vertices[i];
						var v2 = vertices[i + 1];

						var uv1 = uvs[i];
						var uv2 = uvs[i + 1];

						var lm1 = new PointF( Lightmap.SIZE * 0.5f, Lightmap.SIZE * 0.5f );
						var lm2 = new PointF( Lightmap.SIZE * 0.5f, Lightmap.SIZE * 0.5f );

						if( lms.Count > i )
							lm1 = lms[i];
						if( lms.Count > i + 1 )
							lm2 = lms[i + 1];

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

							var tempLM = lm2;
							lm2 = lm1;
							lm1 = tempLM;
						}

						writer.Write( v0, Grid.SIZE_BASE );
						writer.Write( uv0 );
						writer.Write( lm0, Lightmap.SIZE );

						writer.Write( v1, Grid.SIZE_BASE );
						writer.Write( uv1 );
						writer.Write( lm1, Lightmap.SIZE );

						writer.Write( v2, Grid.SIZE_BASE );
						writer.Write( uv2 );
						writer.Write( lm2, Lightmap.SIZE );
					}
				}
			}

			foreach( var entity in entities )
			{
				writer.Write( entity.Position, Grid.SIZE_BASE );
			}

			writer.Close();
			stream.Close();
		}

		private void Level_Export()
		{
			saveFileDialog.Title = "Export";
			saveFileDialog.DefaultExt = ".lvl";
			saveFileDialog.Filter = "Level files|*.lvl|All files|*.*";

			if( saveFileDialog.ShowDialog() == DialogResult.OK )
			{
				var path = saveFileDialog.FileName;

				ExportLevel( path );
			}
		}

		private void loadToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Level_Load();
		}

		private void SaveLevel()
		{
			var ser = new XmlSerializer( typeof( Level ) );
			using( var stream = new FileStream( _levelPath, FileMode.Create, FileAccess.Write ) )
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

			_lastSaveCommandIndex = _commandStack.Index;
			UpdateTitle();
		}

		private bool PromptLevelPath()
		{
			var result = false;

			saveFileDialog.Title = "Save";
			saveFileDialog.DefaultExt = ".xml";
			saveFileDialog.Filter = "XML files|*.xml|All files|*.*";

			if( saveFileDialog.ShowDialog() == DialogResult.OK )
			{
				_levelPath = saveFileDialog.FileName;
				_levelName = _levelPath.NameFromPath();

				result = true;
			}

			return result;
		}

		private void saveToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Level_Save();
		}

		private void saveAsToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Level_SaveAs();
		}

		private void exportToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Level_Export();
		}
		
		private void GenerateLightmap( string filename )
		{
			Lightmap.Generate( _level );
			Lightmap.Upload( filename );

			view_3d.Invalidate();
		}

		private void exitToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void UpdateTitle()
		{
			Text = "FnuFF Editor - " + _levelName;
			if( _lastSaveCommandIndex != _commandStack.Index )
				Text += "*";
		}

		private void EditorForm_FormClosing( object sender, FormClosingEventArgs e )
		{
#if false
			if( _lastSaveCommandIndex != _commandStack.Index )
			{
				var dialogResult = MessageBox.Show( "There are pending changes. Would you like to save before exiting?", "FnuFF Editor - Unsaved Changes", MessageBoxButtons.YesNoCancel );
				if( dialogResult == DialogResult.Yes )
				{
					var hasPath = !string.IsNullOrEmpty( _levelPath );

					if( !hasPath )
						hasPath = PromptLevelPath();

					if( hasPath )
						SaveLevel();
				}
				else if( dialogResult == DialogResult.Cancel )
					e.Cancel = true;
			}
#endif
		}

		private void timer_hotload_Tick( object sender, EventArgs e )
		{
			var didHotload = TextureMap.CheckHotload();
			if( didHotload )
			{
				ViewGlobalInvalidation();
			}
		}

		private void btn_textureLock_Click( object sender, EventArgs e )
		{
			EditorFlags.TextureLock = btn_textureLock.Selected;
		}

		private void btn_snapToGrid_Click( object sender, EventArgs e )
		{
			EditorFlags.SnapToGrid = btn_snapToGrid.Selected;
		}

		private void btn_showLumels_Click( object sender, EventArgs e )
		{
			EditorFlags.ShowLumels = btn_showLumels.Selected;
			view_3d.Invalidate();
		}

		private void btn_showLumelsOnFace_Click( object sender, EventArgs e )
		{
			EditorFlags.ShowLumelsOnFace = btn_showLumelsOnFace.Selected;
			view_3d.Invalidate();
		}

		private void EditorForm_KeyUp( object sender, KeyEventArgs e )
		{
			if( e.Control )
			{
				if( e.KeyCode == Keys.O )
				{
					Level_Load();
				}
				else if( e.KeyCode == Keys.S )
				{
					if( e.Shift )
						Level_SaveAs();
					else
						Level_Save();
				}
				else if( e.KeyCode == Keys.E )
				{
					Level_Export();
				}
				else if( e.KeyCode == Keys.L )
				{
					LoadLevel( "./assets/levels/lightmap_test04.xml" );
					ExportLevel( "./assets/levels/lightmap_test04.lvl" );
				}
				// command stack manipulation
				else if( e.KeyCode == Keys.Z )
				{
					_commandStack.Undo();
					ViewGlobalInvalidation();
				}
				else if( e.KeyCode == Keys.Y )
				{
					_commandStack.Redo();
					ViewGlobalInvalidation();
				}
			}

			if( e.KeyCode == Keys.Delete )
			{
				var selectedSolid = EditorTool.SelectedSolid;
				var selectedEntity = EditorTool.SelectedEntity;

				if( selectedSolid != null )
				{
					_level.RemoveSolid( selectedSolid );

					var deleteCommand = new CommandSolidCreated( _level.Solids, selectedSolid, true );
					_commandStack.Do( deleteCommand );

					EditorTool.SelectedSolid = null;

					ViewGlobalInvalidation();
				}
				else if( selectedEntity != null )
				{
					_level.RemoveEntity( selectedEntity );

					var deleteCommand = new CommandEntityCreated( _level.Entities, selectedEntity, true );
					_commandStack.Do( deleteCommand );

					EditorTool.SelectedEntity = null;

					ViewGlobalInvalidation();
				}
			}
		}
	}
}
