using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Editor.Entities
{
	public class EntityData
	{
		private static Image _icon = Properties.Resources.icon_entity;
		private static Image _selectedIcon = Properties.Resources.icon_entity_selected;

		private string _name;

		public string Name { get { return _name; } set { _name = value; } }

		public EntityData()
		{
		}

		public virtual Image GetIcon(bool hovered, bool selected)
		{
			if( hovered || selected )
				return _selectedIcon;
			return _icon;
		}
	}
}
