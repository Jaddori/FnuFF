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
using Editor.UndoRedo;

namespace Editor
{
    public class View3DControl : Control
    {
		public delegate void GlobalInvalidationHandler();
		public event GlobalInvalidationHandler OnGlobalInvalidation;

		private const float CAMERA_SCROLL_SPEED = 10.0f;
		private const float CAMERA_FORWARD_SPEED = 5.0f;
		private const float CAMERA_STRAFE_SPEED = 5.0f;

		private static Keys[] CMD_KEYS =
		{
			Keys.Left,
			Keys.Right,
			Keys.Up,
			Keys.Down,
			Keys.Control,
			Keys.Delete,
			Keys.Space,
			Keys.Escape,
			Keys.Alt
		};

		private Level _level;
		private Camera3D _camera;
		private CommandStack _commandStack;

		private bool _mmbDown;
		private Point _previousMousePosition;

		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Level Level
		{
			get { return _level; }
			set
			{
				_level = value;
				_level.OnSolidChange += () => Invalidate();

				Invalidate();
			}
		}

		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Camera3D Camera
		{
			get { return _camera; }
			set { _camera = value; }
		}

		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public CommandStack CommandStack
		{
			get { return _commandStack; }
			set
			{
				_commandStack = value;
				_commandStack.OnDo += ( command ) => Invalidate();
				_commandStack.OnUndo += ( command ) => Invalidate();
				_commandStack.OnRedo += ( command ) => Invalidate();
			}
		}

		public View3DControl()
        {
        }

		protected override void OnCreateControl()
		{
			if( !DesignMode )
			{
				GL.CreateContext( Handle, Size.Width, Size.Height );

				TextureMap.LoadPack( "./assets/textures/pack01.bin" );
				TextureMap.LoadPack( "./assets/textures/tools.bin" );
			}

			_camera = new Camera3D { HorizontalSensitivity = 0.05f, VerticalSensitivity = 0.05f };
			
			//EditorTool.OnEditorToolChanged += OnEditorToolChanged;
			EditorTool.OnHoveredSolidChanged += ( prev, cur ) => Invalidate();
			EditorTool.OnSelectedSolidChanged += ( prev, cur ) => Invalidate();
			EditorTool.OnHoveredFaceChanged += ( prev, cur ) => Invalidate();
			EditorTool.OnSelectedFaceChanged += ( prev, cur ) => Invalidate();
		}

		protected override void OnPaintBackground( PaintEventArgs pevent )
		{
			if(DesignMode)
				base.OnPaintBackground( pevent );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			if( !DesignMode )
			{
				GL.ClearColor( 0.0f, 0.0f, 0.0f, 0.0f );

				GL.ViewMatrix( _camera.Position, _camera.Direction );

				// draw axes
				GL.BeginLines();

				GL.Color4f( 1.0f, 0.0f, 0.0f, 1.0f );
				GL.Vertex3f( 0.0f, 0.0f, 0.0f );
				GL.Vertex3f( 3.0f, 0.0f, 0.0f );

				GL.Color4f( 0.0f, 1.0f, 0.0f, 1.0f );
				GL.Vertex3f( 0.0f, 0.0f, 0.0f );
				GL.Vertex3f( 0.0f, 3.0f, 0.0f );

				GL.Color4f( 0.0f, 0.0f, 1.0f, 1.0f );
				GL.Vertex3f( 0.0f, 0.0f, 0.0f );
				GL.Vertex3f( 0.0f, 0.0f, 3.0f );

				GL.End();

				// draw opaque solids
				foreach( var solid in _level.Solids )
				{
					solid.PaintOpaque3D();
				}

				// draw transparent solids
				GL.EnableDepthMask( false );
				foreach( var solid in _level.Solids )
				{
					solid.PaintTransparent3D();
				}
				GL.EnableDepthMask( true );

				// (DEBUG): Draw lumel tracing

				/*GL.BeginLines();
				foreach( var solid in _level.Solids )
				{
					foreach( var face in solid.Faces )
					{
						GL.Color4f( 0.0f, 1.0f, 0.0f, 1.0f );
						if( face.GOOD_LINES.Count > 0 )
						{
							foreach( var pair in face.GOOD_LINES )
							{
								GL.Vertex3f( pair.Item1 / Grid.SIZE_BASE );
								GL.Vertex3f( pair.Item2 / Grid.SIZE_BASE );
							}
						}

						GL.Color4f( 1.0f, 0.0f, 0.0f, 1.0f );
						if( face.BAD_LINES.Count > 0 )
						{
							foreach( var pair in face.BAD_LINES )
							{
								GL.Vertex3f( pair.Item1 / Grid.SIZE_BASE );
								GL.Vertex3f( pair.Item2 / Grid.SIZE_BASE );
							}
						}
					}
				}

				GL.End();*/

				// draw entities
				foreach( var entity in _level.Entities )
				{
					entity.Paint3D();
				}

				GL.SwapBuffers( Handle );
			}
		}

		private void PaintBox( Triple min, Triple max )
		{
			min /= Grid.SIZE_BASE;
			max /= Grid.SIZE_BASE;

			// bottom circle
			GL.Vertex3f( min );
			GL.Vertex3f( new Triple( max.X, min.Y, min.Z ) );

			GL.Vertex3f( new Triple( max.X, min.Y, min.Z ) );
			GL.Vertex3f( new Triple( max.X, min.Y, max.Z ) );

			GL.Vertex3f( new Triple( max.X, min.Y, max.Z ) );
			GL.Vertex3f( new Triple( min.X, min.Y, max.Z ) );

			GL.Vertex3f( new Triple( min.X, min.Y, max.Z ) );
			GL.Vertex3f( min );

			// top circle
			GL.Vertex3f( new Triple( min.X, max.Y, min.Z ) );
			GL.Vertex3f( new Triple( max.X, max.Y, min.Z ) );

			GL.Vertex3f( new Triple( max.X, max.Y, min.Z ) );
			GL.Vertex3f( new Triple( max.X, max.Y, max.Z ) );

			GL.Vertex3f( new Triple( max.X, max.Y, max.Z ) );
			GL.Vertex3f( new Triple( min.X, max.Y, max.Z ) );

			GL.Vertex3f( new Triple( min.X, max.Y, max.Z ) );
			GL.Vertex3f( new Triple( min.X, max.Y, min.Z ) );

			// supports
			GL.Vertex3f( min );
			GL.Vertex3f( new Triple( min.X, max.Y, min.Z ) );

			GL.Vertex3f( new Triple( max.X, min.Y, min.Z ) );
			GL.Vertex3f( new Triple( max.X, max.Y, min.Z ) );

			GL.Vertex3f( new Triple( max.X, min.Y, max.Z ) );
			GL.Vertex3f( new Triple( max.X, max.Y, max.Z ) );

			GL.Vertex3f( new Triple( min.X, min.Y, max.Z ) );
			GL.Vertex3f( new Triple( min.X, max.Y, max.Z ) );
		}

		protected override void OnMouseEnter( EventArgs e )
		{
			base.OnMouseEnter( e );

			Focus();
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			if( e.Button == MouseButtons.Middle )
				_mmbDown = true;

			if( EditorTool.Current == EditorTools.Select )
			{
				if( e.Button == MouseButtons.Left )
				{
					GeometrySolid hitSolid;
					Face hitFace;

					if( IntersectFace( e.X, e.Y, out hitSolid, out hitFace ) )
					{
						hitSolid.Selected = true;
					}

					EditorTool.SelectedSolid = hitSolid;
				}
			}
			else if( EditorTool.Current == EditorTools.Face )
			{
				if( e.Button == MouseButtons.Left )
				{
					GeometrySolid hitSolid;
					Face hitFace;

					IntersectFace( e.X, e.Y, out hitSolid, out hitFace );
					EditorTool.SelectedFace = hitFace;
				}
				else if( e.Button == MouseButtons.Right )
				{
					GeometrySolid hitSolid;
					Face hitFace;

					if( IntersectFace( e.X, e.Y, out hitSolid, out hitFace ) )
					{
						var newTexture = ( hitFace.PackName != TextureMap.CurrentPackName || hitFace.TextureName != TextureMap.CurrentTextureName );

						if( newTexture )
						{
							var command = new CommandSolidChanged( hitSolid );
							command.Begin();

							hitFace.PackName = TextureMap.CurrentPackName;
							hitFace.TextureName = TextureMap.CurrentTextureName;
							Invalidate();

							command.End();
							_commandStack.Do( command );
						}
					}
				}
			}
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			if( e.Button == MouseButtons.Middle )
				_mmbDown = false;
		}

		protected override void OnMouseWheel( MouseEventArgs e )
		{
			base.OnMouseWheel( e );

			var dir = ( e.Delta > 0 ? 1 : -1 );
			_camera.RelativeMovement( new Triple( 0, 0, CAMERA_SCROLL_SPEED * dir ) );

			Invalidate();
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			if( e.Location == _previousMousePosition )
				return;

			var mouseDelta = new Point( e.Location.X - _previousMousePosition.X, e.Location.Y - _previousMousePosition.Y );

			if( _mmbDown )
			{
				_camera.UpdateDirection( -mouseDelta.X, -mouseDelta.Y );
				Invalidate();
			}

			if( EditorTool.Current == EditorTools.Select )
			{
				GeometrySolid hitSolid;
				Face hitFace;

				IntersectFace( e.X, e.Y, out hitSolid, out hitFace );
				EditorTool.HoveredSolid = hitSolid;
			}
			else if( EditorTool.Current == EditorTools.Face )
			{
				GeometrySolid hitSolid;
				Face hitFace;

				IntersectFace( e.X, e.Y, out hitSolid, out hitFace );
				EditorTool.HoveredFace = hitFace;
			}

			_previousMousePosition = e.Location;
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( CMD_KEYS.Contains( keyData ) )
				OnKeyDown( new KeyEventArgs( keyData ) );
			return base.ProcessCmdKey( ref msg, keyData );
		}

		protected override void OnKeyDown( KeyEventArgs e )
		{
			base.OnKeyDown( e );

			if( e.KeyCode == Keys.Left )
				_camera.UpdateDirection( 1, 0 );
			else if( e.KeyCode == Keys.Right )
				_camera.UpdateDirection( -1, 0 );
			else if( e.KeyCode == Keys.Up )
				_camera.UpdateDirection( 0, 1 );
			else if( e.KeyCode == Keys.Down )
				_camera.UpdateDirection( 0, -1 );

			if( e.KeyCode == Keys.W )
				_camera.RelativeMovement( new Triple( 0, 0, CAMERA_FORWARD_SPEED ) );
			else if( e.KeyCode == Keys.S )
				_camera.RelativeMovement( new Triple( 0, 0, -CAMERA_FORWARD_SPEED ) );
			else if( e.KeyCode == Keys.A )
				_camera.RelativeMovement( new Triple( -CAMERA_STRAFE_SPEED, 0, 0 ) );
			else if( e.KeyCode == Keys.D )
				_camera.RelativeMovement( new Triple( CAMERA_STRAFE_SPEED, 0, 0 ) );

			Invalidate();
		}

		protected override void OnKeyUp( KeyEventArgs e )
		{
			base.OnKeyUp( e );
		}

		private bool IntersectFace( int x, int y, out GeometrySolid hitSolid, out Face hitFace )
		{
			var result = false;

			hitSolid = null;
			hitFace = null;

			var start = GL.Unproject( x, Size.Height - y, 0 );
			var end = GL.Unproject( x, Size.Height - y, 1 );

			start *= Grid.SIZE_BASE;
			end *= Grid.SIZE_BASE;

			var ray = new Ray( start, end );

			var minLength = float.MaxValue;

			foreach( var solid in _level.Solids )
			{
				for( int i = 0; i < solid.Faces.Count; i++ )
				{
					Face face = solid.Faces[i];

					var length = 0.0f;
					if( ray.Intersect( face.Plane, ref length ) )
					{
						if( length < minLength )
						{
							// make sure point is behind all other planes of the solid
							var point = ray.Start + ( ray.Direction * length );

							bool behind = true;
							for( int j = 0; j < solid.Faces.Count && behind; j++ )
							{
								if( j != i )
								{
									Face otherFace = solid.Faces[j];
									if( otherFace.Plane.InFront( point ) )
										behind = false;
								}
							}

							if( behind )
							{
								minLength = length;
								hitFace = face;
								hitSolid = solid;
								result = true;
							}
						}
					}
				}
			}

			return result;
		}
	}
}
