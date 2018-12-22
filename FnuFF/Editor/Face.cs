using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Editor
{
	[DebuggerDisplay( "{Plane.Normal.X}, {Plane.Normal.Y}, {Plane.Normal.Z}, {Plane.D}" )]
	public class Face
	{
		private Plane _plane;
		private Triple _u;
		private Triple _v;
		//private Triple[] _vertices;
		
		//public Triple Normal { get { return _normal; } set { _normal = value; } }
		//public float D { get { return _d; } set { _d = value; } }
		public Plane Plane { get { return _plane; } set { _plane = value; } }
		public Triple U { get { return _u; } set { _u = value; } }
		public Triple V { get { return _v; } set { _v = value; } }
		//public Triple[] Vertices { get { return _vertices; } set { _vertices = value; } }
		public Triple[] Vertices;

		public Face()
		{
			Vertices = new Triple[3];
			_plane = new Plane();
			_u = new Triple();
			_v = new Triple();
		}

		public Face( Triple normal, float d )
			: this(normal, d, new Triple(0,0,0), new Triple(1,1,0))
		{
		}

		public Face( Triple normal, float d, Triple u, Triple v )
		{
			_plane = new Plane( normal, d );
			_u = u;
			_v = v;

			Vertices = new Triple[3];
		}
	}
}
