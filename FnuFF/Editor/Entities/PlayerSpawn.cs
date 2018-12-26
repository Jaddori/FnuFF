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

		private float _angle;

		public float Angle { get { return _angle; } set { _angle = value; } }

		public PlayerSpawn()
		{
		}

		public override Image GetIcon()
		{
			return _icon;
		}
	}
}
