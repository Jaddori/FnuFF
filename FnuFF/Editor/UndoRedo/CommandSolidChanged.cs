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
		private List<Face> _oldFaces;
		private List<Face> _newFaces;

		public CommandSolidChanged()
		{
		}

		public CommandSolidChanged(GeometrySolid solid)
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
	}
}
