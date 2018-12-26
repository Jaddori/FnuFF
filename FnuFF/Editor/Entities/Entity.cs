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
		private Triple _position;
		private bool _active;
		private EntityData _data;

		public Triple Position { get { return _position; } set { _position = value; } }
		public bool Active { get { return _active; } set { _active = value; } }
		public EntityData Data { get { return _data; } set { _data = value; } }

		public Entity()
		{
			_position = new Triple();
			_active = true;
			_data = new EntityData();
		}

		public Entity( Triple position )
		{
			_position = position;
			_active = true;
			_data = new EntityData();
		}

		public virtual void Paint2D( Graphics g, Camera2D camera, float gridSize, float gridGap )
		{
			var localPosition = camera.ToLocal( camera.Project( _position ).Deflate( gridSize ).Inflate( gridGap ) );

			var icon = _data.GetIcon();
			var iconBounds = Extensions.FromPoint( localPosition, icon.Width );
			g.DrawImage( _data.GetIcon(), iconBounds );
		}

		public virtual void Paint3D()
		{
		}
	}
}
