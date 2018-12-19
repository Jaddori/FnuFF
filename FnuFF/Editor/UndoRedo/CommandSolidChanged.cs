using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UndoRedo
{
	public class CommandSolidChanged : ICommand
	{
		private GeometrySolid _solid;
		private Delta<Triple> _min;
		private Delta<Triple> _max;

		public CommandSolidChanged()
		{
		}

		public static CommandSolidChanged NewMin( GeometrySolid solid, Triple oldMin )
		{
			var result = new CommandSolidChanged
			{
				_solid = solid,
				_min = new Delta<Triple>( oldMin, solid.Min ),
				_max = new Delta<Triple>( solid.Max, solid.Max )
			};

			return result;
		}

		public static CommandSolidChanged NewMax( GeometrySolid solid, Triple oldMax )
		{
			var result = new CommandSolidChanged
			{
				_solid = solid,
				_min = new Delta<Triple>( solid.Min, solid.Min ),
				_max = new Delta<Triple>( oldMax, solid.Max )
			};

			return result;
		}

		public void Undo()
		{
			_solid.Min = _min.Old;
			_solid.Max = _max.Old;
		}

		public void Redo()
		{
			_solid.Min = _min.New;
			_solid.Max = _max.New;
		}

		public string GetDescription()
		{
			return "Solid changed.";
		}
	}
}
