using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Editor.Entities
{
	public class PointLight : EntityData
	{
		private static Image _icon = Properties.Resources.icon_light;
		private static Image _selectedIcon = Properties.Resources.icon_light_selected;
		private static UInt32 _icon3D = GL.LoadTexture( "./assets/textures/icon_entity_player_spawn.tga" );

		private Color _color;
		private float _intensity;

		[XmlIgnore]
		public Color Color { get { return _color; } set { _color = value; } }

		[XmlElement( "Color" )]
		[Browsable(false)]
		public int XmlColor
		{
			get { return _color.ToArgb(); }
			set { _color = Color.FromArgb( value ); }
		}

		public float Intensity { get { return _intensity; } set { _intensity = value; } }

		public PointLight()
			: base()
		{
		}

		public override Image GetIcon2D( bool selected )
		{
			if( selected )
				return _selectedIcon;
			return _icon;
		}

		public override UInt32 GetIcon3D()
		{
			return _icon3D;
		}

		public override EntityData Copy()
		{
			return new PointLight { Color = _color, Intensity = _intensity };
		}
	}
}
