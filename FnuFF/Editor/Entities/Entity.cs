using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Serialization;

namespace Editor.Entities
{
	public class Entity
	{
		public const int ICON_SIZE = 32;

		private Triple _position;
		private bool _active;
		private EntityData _data;
		private bool _selected;

		public Triple Position { get { return _position; } set { _position = value; } }
		public bool Active { get { return _active; } set { _active = value; } }

		public EntityData Data { get { return _data; } set { _data = value; } }
		
		[XmlIgnore]
		public bool Selected { get { return _selected; } set { _selected = value; } }

		public Entity()
		{
			_position = new Triple();
			_active = true;
			_data = new EntityData();
			_selected = false;
		}

		public Entity( Triple position )
		{
			_position = position;
			_active = true;
			_data = new EntityData();
			_selected = false;
		}

		public void Move( Triple movement )
		{
			_position += movement;
		}

		public Entity Copy()
		{
			return new Entity( _position )
			{
				Active = _active,
				Data = _data.Copy()
			};
		}

		public bool Contains2D( Point point, Camera2D camera )
		{
			var gpos = camera.Project( _position );
			var lpos = camera.ToLocal( gpos.Deflate( Grid.Size ).Inflate( Grid.Gap ) );

			var bounds = Extensions.FromPoint( lpos, ICON_SIZE );
			var result = bounds.Contains( point );

			return result;
		}

		public virtual void Paint2D( Graphics g, Camera2D camera )
		{
			var localPosition = camera.ToLocal( camera.Project( _position ).Deflate( Grid.Size ).Inflate( Grid.Gap ) );

			var icon = _data.GetIcon2D( _selected );
			var iconBounds = Extensions.FromPoint( localPosition, ICON_SIZE );
			g.DrawImage( icon, iconBounds );
		}

		public virtual void Paint3D()
		{
			var position = _position / Grid.SIZE_BASE;

			GL.EnablePointSprite( true );
			GL.EnableDepthMask( false );
			GL.SetTexture( _data.GetIcon3D() );

			GL.PointSize( 32 );
			GL.BeginPoints();
			GL.TexCoord2f( 1.0f, 1.0f );
			GL.Color4f( 1.0f, 1.0f, 1.0f, 1.0f );
			GL.Vertex3f( position.X, position.Y, position.Z );
			GL.End();

			GL.SetTexture( 0 );
			GL.EnableDepthMask( true );
			GL.EnablePointSprite( false );

			GL.BeginLines();

			GL.Color4f( 1.0f, 0.0f, 0.0f, 1.0f );
			GL.Vertex3f( position.X, position.Y, position.Z );
			GL.Vertex3f( position.X + 0.25f, position.Y, position.Z );

			GL.Color4f( 0.0f, 1.0f, 0.0f, 1.0f );
			GL.Vertex3f( position.X, position.Y, position.Z );
			GL.Vertex3f( position.X, position.Y + 0.25f, position.Z );

			GL.Color4f( 0.0f, 0.0f, 1.0f, 1.0f );
			GL.Vertex3f( position.X, position.Y, position.Z );
			GL.Vertex3f( position.X, position.Y, position.Z + 0.25f );

			GL.End();
		}
	}

	public static class EntityList
	{
		public static string[] Names =
		{
			"Player Spawn",
			"Point Light",
			"World Light",
		};

		public static Type[] Types =
		{
			typeof(PlayerSpawn),
			typeof(PointLight),
			typeof(WorldLight),
		};

		public static EntityData CreateData( int index )
		{
			return Activator.CreateInstance( Types[index] ) as EntityData;
		}
	}
}
