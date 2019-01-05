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
		private string _packName;
		private string _textureName;
		private bool _hovered;

		public Plane Plane { get { return _plane; } set { _plane = value; } }
		public Triple U { get { return _u; } set { _u = value; } }
		public Triple V { get { return _v; } set { _v = value; } }
		public string PackName { get { return _packName; } set { _packName = value; } }
		public string TextureName { get { return _textureName; } set { _textureName = value; } }
		public bool Hovered { get { return _hovered; } set { _hovered = value; } }

		public Face()
		{
			_plane = new Plane();
			_u = new Triple();
			_v = new Triple();
			_hovered = false;
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
			_hovered = false;
		}

		public Face Copy()
		{
			return new Face( _plane.Normal, _plane.D, _u, _v );
		}
	}
}
