using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Editor.Entities
{
	public class WorldLight : EntityData
	{
		private static Image _icon = Properties.Resources.icon_light;
		private static Image _selectedIcon = Properties.Resources.icon_light_selected;
		private UInt32 _icon3D = GL.LoadTexture( "./assets/textures/icon_entity_player_spawn.tga" );

		private Color _ambientColor;
		private float _ambientIntensity;
		private Color _skyColor;
		private float _skyIntensity;

		[XmlIgnore]
		public Color AmbientColor { get { return _ambientColor; } set { _ambientColor = value; } }
		public float AmbientIntensity { get { return _ambientIntensity; } set { _ambientIntensity = value; } }

		[XmlIgnore]
		public Color SkyColor { get { return _skyColor; } set { _skyColor = value; } }
		public float SkyIntensity { get { return _skyIntensity; } set { _skyIntensity = value; } }

		[XmlElement( "AmbientColor" )]
		[Browsable( false )]
		public int XmlAmbientColor
		{
			get { return _ambientColor.ToArgb(); }
			set { _ambientColor = Color.FromArgb( value ); }
		}

		[XmlElement( "SkyColor" )]
		[Browsable( false )]
		public int XmlSkyColor
		{
			get { return _skyColor.ToArgb(); }
			set { _skyColor = Color.FromArgb( value ); }
		}

		public WorldLight()
			: base()
		{
		}

		public override Image GetIcon2D( bool selected )
		{
			if( selected )
				return _selectedIcon;
			return _icon;
		}

		public override uint GetIcon3D()
		{
			return _icon3D;
		}
	}
}
