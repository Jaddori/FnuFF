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
		private static UInt32 _icon3D = GL.LoadTexture( "./assets/textures/icon_entity_player_spawn.tga" );

		private float _angle;

		public float Angle { get { return _angle; } set { _angle = value; } }

		public PlayerSpawn()
			: base()
		{
		}

		public override Image GetIcon2D(bool selected)
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
			return new PlayerSpawn { Angle = _angle };
		}
	}
}
