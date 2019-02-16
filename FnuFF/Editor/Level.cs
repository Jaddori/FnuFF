using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Editor.Entities;

namespace Editor
{
	public class Level
	{
		public delegate void ChangeHandler();
		public event ChangeHandler OnSolidChange;
		public event ChangeHandler OnEntityChange;

		private List<Solid> _solids;
		private List<Entity> _entities;

		public List<Solid> Solids => _solids;
		public List<Entity> Entities => _entities;

		public Level()
		{
			_solids = new List<Solid>();
			_entities = new List<Entity>();
		}

		public void AddSolid( Solid solid )
		{
			_solids.Add( solid );
			OnSolidChange?.Invoke();
		}

		public void RemoveSolid( Solid solid )
		{
			_solids.Remove( solid );
			OnSolidChange?.Invoke();
		}

		public void AddEntity( Entity entity )
		{
			_entities.Add( entity );
			OnEntityChange?.Invoke();
		}

		public void RemoveEntity( Entity entity )
		{
			_entities.Remove( entity );
			OnEntityChange?.Invoke();
		}

		public void Reset()
		{
			_solids.Clear();
			_entities.Clear();
			OnSolidChange?.Invoke();
			OnEntityChange?.Invoke();
		}
	}
}
