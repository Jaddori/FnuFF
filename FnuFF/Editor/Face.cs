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

		private List<Triple> _vertices;
		private List<PointF> _uvs;
		private bool _hovered;
		private bool _selected;

		public Plane Plane { get { return _plane; } set { _plane = value; } }
		public string PackName { get { return _packName; } set { _packName = value; } }
		public string TextureName { get { return _textureName; } set { _textureName = value; } }

		public PointF Offset
		{
			get { return _offset; }
			set
			{
				_offset = value;
				UpdateUVs();
			}
		}

		public PointF Scale
		{
			get { return _scale; }
			set
			{
				_scale = value;
				UpdateUVs();
			}
		}

		public float Rotation
		{
			get { return _rotation; }
			set
			{
				_rotation = value;
				UpdateUVs();
			}
		}

		[XmlIgnore]
		public List<Triple> Vertices => _vertices;

		[XmlIgnore]
		public List<PointF> UVs => _uvs;

		[XmlIgnore]
		public bool Hovered { get { return _hovered; } set { _hovered = value; } }

		[XmlIgnore]
		public bool Selected { get { return _selected; } set { _selected = value; } }

		public Face()
		{
			_plane = new Plane();
			_offset = new PointF( 0.0f, 0.0f );
			_scale = new PointF( 1.0f, 1.0f );
			_vertices = new List<Triple>();
			_uvs = new List<PointF>();
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
			_vertices = new List<Triple>();
			_uvs = new List<PointF>();
			_rotation = rotation;
			_hovered = false;
		}

		public Face Copy()
		{
			var result = new Face( _plane.Normal, _plane.D, _offset, _scale, _rotation )
			{
				PackName = _packName,
				TextureName = _textureName
			};

			foreach( var vertex in _vertices )
				result._vertices.Add( vertex );

			foreach( var uv in _uvs )
				result._uvs.Add( uv );

			return result;
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

		public void BuildVertices( GeometrySolid parent )
		{
			_vertices.Clear();

			if( !EditorFlags.TextureLock )
				_uvs.Clear();

			var otherPlanes = parent.Faces.Where( x => x != this ).Select( x => x.Plane ).ToArray();
			var points = Extensions.IntersectPlanes( _plane, otherPlanes );
			var indices = Extensions.WindingIndex3D( points, _plane.Normal );
			var texCoords = points.Select( x => Extensions.EmitTextureCoordinates( _plane.Normal, x, this ) ).ToArray();
			
			for( int i=0; i<indices.Length; i++ )
			{
				var index = indices[i];
				_vertices.Add( points[index] );
				
				if( i > _uvs.Count-1 )
					_uvs.Add( texCoords[index] );
			}
		}

		public void UpdateUVs()
		{
			if( _vertices.Count > 0 )
			{
				_uvs.Clear();

				var texCoords = _vertices.Select( x => Extensions.EmitTextureCoordinates( _plane.Normal, x, this ) ).ToArray();
				_uvs.AddRange( texCoords );
			}
		}

		public void BuildPlane()
		{
			var v1v0 = _vertices[1] - _vertices[0];
			var v2v0 = _vertices[2] - _vertices[0];

			var normal = v2v0.Cross( v1v0 );
			normal.Normalize();

			var d = _vertices[0].Dot( normal );

			if( _plane.Normal.Dot( normal ) < 0 )
				normal *= -1;

			_plane.Normal = normal;
			_plane.D = d;
		}
	}
}
