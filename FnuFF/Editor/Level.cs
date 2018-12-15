using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Editor
{
	public class Level
	{
		public delegate void ChangeHandler();
		public event ChangeHandler OnSolidChange;

		private List<GeometrySolid> _solids;

		public List<GeometrySolid> Solids => _solids;

		public Level()
		{
			_solids = new List<GeometrySolid>();
		}

		public void AddSolid( GeometrySolid solid )
		{
			_solids.Add( solid );
			OnSolidChange?.Invoke();
		}

		public void Reset()
		{
			_solids.Clear();
			OnSolidChange?.Invoke();
		}
	}
}
