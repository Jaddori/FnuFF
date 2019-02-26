using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UndoRedo
{
	public class CommandSolidChanged : ICommand
	{
		private Solid _solid;
		private List<Face> _oldFaces;
		private List<Face> _newFaces;
		private bool _hasChanges;

		public bool HasChanges { get { return _hasChanges; } }

		public CommandSolidChanged()
		{
		}

		public CommandSolidChanged(Solid solid)
		{
			_solid = solid;
			_oldFaces = new List<Face>();
			_newFaces = new List<Face>();
		}

		public void Begin()
		{
			_oldFaces.Clear();

			foreach( var face in _solid.Faces )
				_oldFaces.Add( face.Copy() );
		}

		public void End()
		{
			_newFaces.Clear();

			foreach( var face in _solid.Faces )
				_newFaces.Add( face.Copy() );

			// check if we have any changes
			if( _oldFaces.Count != _newFaces.Count )
			{
				_hasChanges = true;
			}
			else
			{
				_hasChanges = false;
				var faceCount = _oldFaces.Count;
				for( int i = 0; i < faceCount && !_hasChanges; i++ )
				{
					if( !_oldFaces[i].Equals( _newFaces[i] ) )
						_hasChanges = true;
				}
			}
		}

		public void Undo()
		{
			_solid.Faces.Clear();
			foreach( var face in _oldFaces )
				_solid.Faces.Add( face.Copy() );
		}

		public void Redo()
		{
			_solid.Faces.Clear();
			foreach( var face in _newFaces )
				_solid.Faces.Add( face.Copy() );
		}

		public string GetDescription()
		{
			return "Solid changed.";
		}

		public bool AffectsSelection()
		{
			return false;
		}
	}
}
