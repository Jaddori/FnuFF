using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.UndoRedo;

namespace Editor.UnitTests
{
	public class CommandStackTestCommand : ICommand
	{
		public int Undos { get; set; } = 0;
		public int Redos { get; set; } = 0;

		public void Undo()
		{
			Undos++;
		}

		public void Redo()
		{
			Redos++;
		}

		public string GetDescription()
		{
			return "Command used in Unit Tests.";
		}
	}
}
