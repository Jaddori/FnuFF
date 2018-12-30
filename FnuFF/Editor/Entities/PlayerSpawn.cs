using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Entities
{
	public class PlayerSpawn : EntityData
	{
		private static Image _icon = Properties.Resources.icon_entity_player_spawn;
		private static Image _selectedIcon = Properties.Resources.icon_entity_player_spawn_selected;

		private float _angle;

		public float Angle { get { return _angle; } set { _angle = value; } }

		public PlayerSpawn()
		{
		}

		public override Image GetIcon(bool hovered, bool selected)
		{
			if( hovered || selected )
				return _selectedIcon;
			return _icon;
		}
	}
}
