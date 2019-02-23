using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UndoRedo
{
	public class CommandMultipleSolidsChanged : ICommand
	{
		private List<Solid> _solids;
		private List<CommandSolidChanged> _commands;

		private bool _hasChanges;

		public bool HasChanges => _hasChanges;

		public CommandMultipleSolidsChanged( IEnumerable<Solid> solids )
		{
			_solids = new List<Solid>();
			_solids.AddRange( solids );

			_commands = new List<CommandSolidChanged>();
			_hasChanges = false;
		}

		public void Begin()
		{
			foreach( var solid in _solids )
			{
				var command = new CommandSolidChanged( solid );
				command.Begin();

				_commands.Add( command );
			}
		}

		public void End()
		{
			var finalCommands = new List<CommandSolidChanged>();
			foreach( var command in _commands )
			{
				command.End();

				if( command.HasChanges )
				{
					finalCommands.Add( command );
				}
			}

			_hasChanges = ( finalCommands.Count > 0 );
			_commands = finalCommands;
		}

		public void Undo()
		{
			foreach( var command in _commands )
				command.Undo();
		}

		public void Redo()
		{
			foreach( var command in _commands )
				command.Redo();
		}

		public string GetDescription()
		{
			return "Multiple solids changed.";
		}

		public bool AffectsSelection()
		{
			return false;
		}
	}
}
