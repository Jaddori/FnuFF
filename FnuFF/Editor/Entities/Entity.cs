﻿using System;
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

		[XmlIgnore]
		public bool Hovered { get { return _hovered; } set { _hovered = value; } }
		[XmlIgnore]
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

			var icon = _data.GetIcon2D(_hovered, _selected);
			var iconBounds = Extensions.FromPoint( localPosition, icon.Width );
			g.DrawImage( icon, iconBounds );
		}

		public virtual void Paint3D()
		{
			GL.EnablePointSprite( true );
			GL.EnableDepthMask( false );
			GL.SetTexture( _data.GetIcon3D() );

			GL.PointSize( 32 );
			GL.BeginPoints();
			GL.TexCoord2f( 1.0f, 1.0f );
			GL.Color4f( 1.0f, 1.0f, 1.0f, 1.0f );
			GL.Vertex3f( _position.X, _position.Y, _position.Z );
			GL.End();

			GL.SetTexture( 0 );
			GL.EnableDepthMask( true );
			GL.EnablePointSprite( false );

			GL.BeginLines();

			GL.Color4f( 1.0f, 0.0f, 0.0f, 1.0f );
			GL.Vertex3f( _position.X, _position.Y, _position.Z );
			GL.Vertex3f( _position.X + 0.25f, _position.Y, _position.Z );

			GL.Color4f( 0.0f, 1.0f, 0.0f, 1.0f );
			GL.Vertex3f( _position.X, _position.Y, _position.Z );
			GL.Vertex3f( _position.X, _position.Y+ 0.25f, _position.Z );

			GL.Color4f( 0.0f, 0.0f, 1.0f, 1.0f );
			GL.Vertex3f( _position.X, _position.Y, _position.Z );
			GL.Vertex3f( _position.X, _position.Y, _position.Z + 0.25f );

			GL.End();
		}
	}
}