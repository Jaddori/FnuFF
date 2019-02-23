using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UndoRedo
{
	public interface ICommand
	{
		void Undo();
		void Redo();

		string GetDescription();
		bool AffectsSelection();
	}
}
