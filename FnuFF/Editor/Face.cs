using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Drawing;

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
		private PointF _offset;
		private PointF _scale;
		private float _rotation;
		private bool _hovered;
		private bool _selected;

		public Plane Plane { get { return _plane; } set { _plane = value; } }
		public Triple U { get { return _u; } set { _u = value; } }
		public Triple V { get { return _v; } set { _v = value; } }
		public string PackName { get { return _packName; } set { _packName = value; } }
		public string TextureName { get { return _textureName; } set { _textureName = value; } }
		public PointF Offset { get { return _offset; } set { _offset = value; } }
		public PointF Scale { get { return _scale; } set { _scale = value; } }
		public float Rotation { get { return _rotation; } set { _rotation = value; } }

		[XmlIgnore]
		public bool Hovered { get { return _hovered; } set { _hovered = value; } }

		[XmlIgnore]
		public bool Selected { get { return _selected; } set { _selected = value; } }

		public Face()
		{
			_plane = new Plane();
			_u = new Triple();
			_v = new Triple();
			_offset = new PointF( 0.0f, 0.0f );
			_scale = new PointF( 1.0f, 1.0f );
			_rotation = 0.0f;
			_hovered = false;
		}

		public Face( Triple normal, float d )
			: this(normal, d, new Triple(0,0,0), new Triple(1,1,0))
		{
		}

		public Face( Triple normal, float d, Triple u, Triple v )
			: this( normal, d, u, v, new PointF( 0, 0 ), new PointF( 1, 1 ), 0.0f )
		{
		}

		public Face( Triple normal, float d, Triple u, Triple v, PointF offset, PointF scale, float rotation )
		{
			_plane = new Plane( normal, d );
			_u = u;
			_v = v;
			_offset = offset;
			_scale = scale;
			_rotation = rotation;
			_hovered = false;
		}

		public Face Copy()
		{
			return new Face( _plane.Normal, _plane.D, _u, _v );
		}
	}
}
