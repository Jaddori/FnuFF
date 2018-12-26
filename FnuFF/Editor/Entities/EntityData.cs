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

		private string _name;

		public string Name { get { return _name; } set { _name = value; } }

		public EntityData()
		{
		}

		public virtual Image GetIcon()
		{
			return _icon;
		}
	}
}
