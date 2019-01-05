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
		private int _frame;
		private Camera3D _camera;

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

		public View3DControl()
        {
        }

		protected override void OnCreateControl()
		{
			if( !DesignMode )
			{
				GL.CreateContext( Handle, Size.Width, Size.Height );

				TextureMap.LoadPack( "./assets/textures/pack01.bin" );
				TextureMap.LoadPack( "./assets/textures/pack02.bin" );
			}

			_camera = new Camera3D { HorizontalSensitivity = 0.05f, VerticalSensitivity = 0.05f };

			_frame = 0;
			Log.AddFunctor( Name, () => "Frame: " + _frame.ToString() );
		}

		protected override void OnPaintBackground( PaintEventArgs pevent )
		{
			if(DesignMode)
				base.OnPaintBackground( pevent );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			//base.OnPaint( e );

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

				//GL.SetTexture( _texture );
				
				// draw solids
				foreach( var solid in _level.Solids )
				{
					solid.Paint3D();
				}

				// draw entities
				foreach( var entity in _level.Entities )
				{
					entity.Paint3D();
				}

				GL.SwapBuffers( Handle );
				_frame++;
			}
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

			if( EditorTool.Current == EditorTools.Face )
			{
				if( e.Button == MouseButtons.Right )
				{
					var start = GL.Unproject( e.X, Size.Height - e.Y, 0 );
					var end = GL.Unproject( e.X, Size.Height - e.Y, 1 );

					var ray = new Ray( start, end );

					var minLength = float.MaxValue;
					Face minFace = null;

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
										minFace = face;
									}
								}
							}
						}
					}

					if( minFace != null )
					{
						minFace.PackName = TextureMap.CurrentPackName;
						minFace.TextureName = TextureMap.CurrentTextureName;
						Invalidate();
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

			if( EditorTool.Current == EditorTools.Face )
			{
				var start = GL.Unproject( e.X, Size.Height - e.Y, 0 );
				var end = GL.Unproject( e.X, Size.Height - e.Y, 1 );

				var ray = new Ray( start, end );

				var minLength = float.MaxValue;
				Face minFace = null;

				bool prevHovered = false;
				foreach( var solid in _level.Solids )
				{
					for( int i = 0; i < solid.Faces.Count; i++ )
					{
						Face face = solid.Faces[i];
						if( face.Hovered )
							prevHovered = true;
						face.Hovered = false;

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
									minFace = face;
								}
							}
						}
					}
				}

				if( minFace != null )
				{
					minFace.Hovered = true;
					Invalidate();
				}
				else if( prevHovered )
					Invalidate();
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
	}
}
