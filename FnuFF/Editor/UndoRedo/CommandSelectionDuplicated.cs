using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Entities;

namespace Editor.UndoRedo
{
	public class CommandSelectionDuplicated : ICommand
	{
		private List<Solid> _solids;
		private List<Entity> _entities;

		private List<Solid> _newSolids;
		private List<Entity> _newEntities;

		public CommandSelectionDuplicated( List<Solid> solids, List<Entity> entities, IEnumerable<Solid> newSolids, IEnumerable<Entity> newEntities )
		{
			_solids = solids;
			_entities = entities;

			_newSolids = new List<Solid>();
			_newEntities = new List<Entity>();

			_newSolids.AddRange( newSolids );
			_newEntities.AddRange( newEntities );
		}

		public void Undo()
		{
			foreach( var solid in _newSolids )
				_solids.Remove( solid );
			foreach( var entity in _newEntities )
				_entities.Remove( entity );
		}

		public void Redo()
		{
			_solids.AddRange( _newSolids );
			_entities.AddRange( _newEntities );
		}

		public string GetDescription()
		{
			return "Selection duplicated.";
		}

		public bool AffectsSelection()
		{
			return true;
		}
	}
}
