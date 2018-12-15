using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UndoRedo
{
	public class CommandNewLevel : ICommand
	{
		public void Undo()
		{
		}

		public void Redo()
		{
		}

		public string GetDescription()
		{
			return "- New Level -";
		}
	}
}
