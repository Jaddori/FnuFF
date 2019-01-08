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

		public delegate void CommandHandler( ICommand command );
		public event CommandHandler OnDo;
		public event CommandHandler OnUndo;
		public event CommandHandler OnRedo;

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

		public void Reset()
		{
			_commands.Clear();
			_commands.Add( new CommandNewLevel() );

			_index = 0;
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

			OnDo?.Invoke( command );
		}

		public void Undo()
		{
			if( _index > 0 )
			{
				var command = _commands[_index];
				command.Undo();
				_index--;

				OnUndo?.Invoke( command );
			}
		}

		public void Redo()
		{
			if( _index < _commands.Count-1 )
			{
				_index++;
				var command = _commands[_index];
				command.Redo();

				OnRedo?.Invoke( command );
			}
		}
	}
}
