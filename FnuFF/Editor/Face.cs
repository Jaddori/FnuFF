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
		private string _packName;
		private string _textureName;
		private PointF _offset;
		private PointF _scale;
		private float _rotation;
		private bool _hovered;
		private bool _selected;

		public Plane Plane { get { return _plane; } set { _plane = value; } }
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
			_offset = new PointF( 0.0f, 0.0f );
			_scale = new PointF( 1.0f, 1.0f );
			_rotation = 0.0f;
			_hovered = false;
		}

		public Face( Triple normal, float d )
			: this( normal, d, new PointF( 0, 0 ), new PointF( 1, 1 ), 0.0f )
		{
		}

		public Face( Triple normal, float d, PointF offset, PointF scale, float rotation )
		{
			_plane = new Plane( normal, d );
			_offset = offset;
			_scale = scale;
			_rotation = rotation;
			_hovered = false;
		}

		public Face Copy()
		{
			return new Face( _plane.Normal, _plane.D, _offset, _scale, _rotation ) { PackName = _packName, TextureName = _textureName };
		}

		public override bool Equals( object obj )
		{
			var result = false;

			if( obj is Face )
			{
				var face = obj as Face;
				result =
				(
					_plane.Equals( face._plane ) &&
					_offset.Equals( face._offset ) &&
					_scale.Equals( face._scale ) &&
					_rotation == face._rotation &&
					_packName == face._packName &&
					_textureName == face._textureName
				);
			}

			return result;
		}
	}
}
