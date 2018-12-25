using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UndoRedo
{
	public class CommandStack
	{
		public const int MAX_COMMANDS = 50;

		private List<ICommand> _commands;
		private int _index;

		public List<ICommand> Commands => _commands;
		public int Index => _index;

		public CommandStack()
		{
			_commands = new List<ICommand>();
			_index = 0;

			_commands.Add( new CommandNewLevel() );
		}

		public void Do( ICommand command )
		{
			if( _index >= MAX_COMMANDS-1 )
			{
				_commands.RemoveAt( 0 );
			}

			var currentCommand = _index + 1;
			if( currentCommand < _commands.Count )
			{
				_commands.RemoveRange( currentCommand, _commands.Count - currentCommand );
			}

			_commands.Add( command );
			_index = _commands.Count - 1;
		}

		public void Undo()
		{
			if( _index > 0 )
			{
				_commands[_index].Undo();
				_index--;
			}
		}

		public void Redo()
		{
			if( _index < _commands.Count-1 )
			{
				_index++;
				_commands[_index].Redo();
			}
		}
	}
}
