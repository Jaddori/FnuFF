using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Entities;

namespace Editor.UndoRedo
{
	public class CommandSelectionMoved : ICommand
	{
		private List<Solid> _solids;
		private List<Entity> _entities;
		private Triple _startPosition;
		private Triple _movement;
		private bool _hasChanges;

		public bool HasChanges => _hasChanges;

		public CommandSelectionMoved( IEnumerable<Solid> solids, IEnumerable<Entity> entities )
		{
			_solids = new List<Solid>();
			_entities = new List<Entity>();

			_solids.AddRange( solids );
			_entities.AddRange( entities );

			_hasChanges = false;
		}

		public void Begin()
		{
			if( _entities.Count > 0 )
			{
				_startPosition = _entities[0].Position;
			}
			else if( _solids.Count > 0 )
			{
				_startPosition = _solids[0].Faces[0].Vertices[0];
			}
		}

		public void End()
		{
			if( _entities.Count > 0 )
			{
				var endPosition = _entities[0].Position;
				_movement = endPosition - _startPosition;
			}
			else if( _solids.Count > 0 )
			{
				var endPosition = _solids[0].Faces[0].Vertices[0];
				_movement = endPosition - _startPosition;
			}

			_hasChanges = ( _movement.Length() > 0 );
		}

		public void Undo()
		{
			foreach( var s in _solids )
				s.Move( -_movement );
			foreach( var e in _entities )
				e.Move( -_movement );
		}

		public void Redo()
		{
			foreach( var s in _solids )
				s.Move( _movement );
			foreach( var e in _entities )
				e.Move( _movement );
		}

		public string GetDescription()
		{
			return "Selection moved.";
		}

		public bool AffectsSelection()
		{
			return false;
		}
	}
}
