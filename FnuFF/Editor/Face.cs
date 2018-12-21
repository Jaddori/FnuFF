using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
	public class Face
	{
		private List<Triple> _points;
		private List<Triple> _coords;
		private List<int> _indices;
		private Triple _normal;
		private float _d;

		public List<Triple> Points => _points;
		public List<Triple> Coords => _coords;
		public List<int> Indices => _indices;
		public Triple Normal { get { return _normal; } set { _normal = value; } }
		public float D { get { return _d; } set { _d = value; } }

		public Face()
		{
			_points = new List<Triple>();
			_coords = new List<Triple>();
			_indices = new List<int>();
		}
	}
}
