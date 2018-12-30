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
		private bool _hovered;
		private bool _selected;

		public Triple Position { get { return _position; } set { _position = value; } }
		public bool Active { get { return _active; } set { _active = value; } }
		public EntityData Data { get { return _data; } set { _data = value; } }
		public bool Hovered { get { return _hovered; } set { _hovered = value; } }
		public bool Selected { get { return _selected; } set { _selected = value; } }

		public Entity()
		{
			_position = new Triple();
			_active = true;
			_data = new EntityData();
			_hovered = false;
			_selected = false;
		}

		public Entity( Triple position )
		{
			_position = position;
			_active = true;
			_data = new EntityData();
			_hovered = false;
			_selected = false;
		}

		public bool Contains2D( Point point, Camera2D camera, float gridSize, float gridGap )
		{
			var gpos = camera.Project( _position );
			var lpos = camera.ToLocal( gpos.Deflate( gridSize ).Inflate( gridGap ) );

			var bounds = Extensions.FromPoint( lpos, 32 );
			var result = bounds.Contains( point );

			return result;
		}

		public virtual void Paint2D( Graphics g, Camera2D camera, float gridSize, float gridGap )
		{
			var localPosition = camera.ToLocal( camera.Project( _position ).Deflate( gridSize ).Inflate( gridGap ) );

			var icon = _data.GetIcon(_hovered, _selected);
			var iconBounds = Extensions.FromPoint( localPosition, icon.Width );
			g.DrawImage( icon, iconBounds );
		}

		public virtual void Paint3D()
		{
		}
	}
}
