using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Entities;

namespace Editor.UndoRedo
{
	public class CommandEntityChanged : ICommand
	{
		private Entity _entity;
		private Delta<Triple> _position;
		private bool _hasChanges;

		public bool HasChanges => _hasChanges;

		public CommandEntityChanged()
		{
			_position = new Delta<Triple>();
			_hasChanges = false;
		}

		public CommandEntityChanged( Entity entity )
		{
			_entity = entity;
			_position = new Delta<Triple>();
			_hasChanges = false;
		}

		public CommandEntityChanged( Entity entity, Triple oldPosition )
		{
			_entity = entity;
			_position = new Delta<Triple>( oldPosition, _entity.Position );
			_hasChanges = false;
		}

		public void Begin()
		{
			_position.Old = _entity.Position;
		}

		public void End()
		{
			_position.New = _entity.Position;

			_hasChanges = !_position.Old.Equals( _position.New );
		}

		public void Undo()
		{
			_entity.Position = _position.Old;
		}

		public void Redo()
		{
			_entity.Position = _position.New;
		}

		public string GetDescription()
		{
			return "Entity changed.";
		}

		public bool AffectsSelection()
		{
			return false;
		}
	}
}
