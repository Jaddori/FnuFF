using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UndoRedo
{
	public class CommandSolidCreated : ICommand
	{
		private IList<Solid> _container;
		private Solid _solid;
		private bool _inverted;

		public CommandSolidCreated( IList<Solid> container, Solid solid, bool inverted = false )
		{
			_container = container;
			_solid = solid;
			_inverted = inverted;
		}

		public void Undo()
		{
			if( _inverted )
				_container.Add( _solid );
			else
				_container.Remove( _solid );
		}

		public void Redo()
		{
			if( _inverted )
				_container.Remove( _solid );
			else
				_container.Add( _solid );
		}

		public string GetDescription()
		{
			return "Solid created.";
		}

		public bool AffectsSelection()
		{
			return true;
		}
	}
}
