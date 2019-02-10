using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Serialization;

namespace Editor.Entities
{
	[XmlInclude( typeof( PlayerSpawn ) )]
	[XmlInclude( typeof( PointLight) )]
	[XmlInclude( typeof( WorldLight ) )]
	public class EntityData
	{
		private static Image _icon = Properties.Resources.icon_entity;
		private static Image _selectedIcon = Properties.Resources.icon_entity_selected;
		private static UInt32 _icon3D = 0;

		private string _name;

		public string Name { get { return _name; } set { _name = value; } }

		public EntityData()
		{
			_name = string.Empty;
		}

		public virtual Image GetIcon2D(bool hovered, bool selected)
		{
			if( hovered || selected )
				return _selectedIcon;
			return _icon;
		}

		public virtual UInt32 GetIcon3D()
		{
			return _icon3D;
		}
	}
}
