using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Entities;

namespace Editor.UndoRedo
{
	public class CommandEntityCreated : ICommand
	{
		private List<Entity> _entities;
		private Entity _entity;
		private bool _inverted;

		public CommandEntityCreated(List<Entity> entities, Entity entity, bool inverted = false)
		{
			_entities = entities;
			_entity = entity;
			_inverted = inverted;
		}

		public void Undo()
		{
			if( _inverted )
			{
				_entities.Add( _entity );
			}
			else
			{
				_entities.Remove( _entity );
			}
		}

		public void Redo()
		{
			if( _inverted )
			{
				_entities.Remove( _entity );
			}
			else
			{
				_entities.Add( _entity );
			}
		}

		public string GetDescription()
		{
			if( _inverted )
				return "Entity deleted.";
			return "Entity created.";
		}

		public bool AffectsSelection()
		{
			return true;
		}
	}
}
