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
		
		private List<Lumel> _lumels;
		private List<PointF> _lightmapUVs;
		private Triple _tangent;
		private Triple _bitangent;
		private float _width;
		private float _height;
		private int _lumelWidth;
		private int _lumelHeight;
		private int _lumelSize;

		[XmlIgnore]
		public List<Tuple<Triple, Triple>> GOOD_LINES = new List<Tuple<Triple, Triple>>();

		[XmlIgnore]
		public List<Tuple<Triple, Triple>> BAD_LINES = new List<Tuple<Triple, Triple>>();

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

		public int LumelSize
		{
			get { return _lumelSize; }
			set
			{
				_lumelSize = value;

				if( _lumels != null )
					_lumels.Clear();
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

		[XmlIgnore]
		public List<Lumel> Lumels => _lumels;

		[XmlIgnore]
		public List<PointF> LightmapUVs => _lightmapUVs;

		[XmlIgnore]
		public float Width => _width;

		[XmlIgnore]
		public float Height => _height;

		[XmlIgnore]
		public int LumelWidth => _lumelWidth;

		[XmlIgnore]
		public int LumelHeight => _lumelHeight;

		public Face()
		{
			_plane = new Plane();
			_offset = new PointF( 0.0f, 0.0f );
			_scale = new PointF( 1.0f, 1.0f );
			_vertices = new List<Triple>();
			_uvs = new List<PointF>();
			_rotation = 0.0f;
			_hovered = false;
			//_lumels = new List<Triple>();
			_lumels = new List<Lumel>();
			_lightmapUVs = new List<PointF>();
			_lumelSize = 16;
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
			//_lumels = new List<Triple>();
			_lumels = new List<Lumel>();
			_lightmapUVs = new List<PointF>();
			_lumelSize = 16;
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

			for( int i = 0; i < indices.Length; i++ )
			{
				var index = indices[i];
				_vertices.Add( points[index] );

				if( i > _uvs.Count - 1 )
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
			{
				normal *= -1;
				d = -d;
			}

			_plane.Normal = normal;
			_plane.D = d;
		}

		public void BuildLumels( GeometrySolid parent )
		{
			_lumels.Clear();

			if( _vertices.Count < 3 )
				return;

			var otherPlanes = parent.Faces.Where( x => x != this ).Select( x => x.Plane ).ToArray();

			var v0 = _vertices[0];// / Grid.SIZE_BASE;
			var v1 = _vertices[1];// / Grid.SIZE_BASE;
			var v2 = _vertices[2];// / Grid.SIZE_BASE;

			var uv0 = _uvs[0];
			var uv1 = _uvs[1];
			var uv2 = _uvs[2];

			var deltaPos1 = v1 - v0;
			var deltaPos2 = v2 - v0;

			var deltaUV1 = uv1.Sub( uv0 );
			var deltaUV2 = uv2.Sub( uv0 );

			var r = 1.0f / ( deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X + 0.0001f );
			_tangent = ( deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y ) * r;
			_bitangent = ( deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X ) * r;

			var xaxis = _tangent;
			xaxis.Normalize();

			var yaxis = _bitangent;
			yaxis.Normalize();

			var minx = _vertices.Min( x => x.Dot( xaxis ) );;
			var miny = _vertices.Min( x => x.Dot( yaxis ) );;
			var maxx = _vertices.Max( x => x.Dot( xaxis ) );;
			var maxy = _vertices.Max( x => x.Dot( yaxis ) );;
			var z = _plane.D;

			_width = ( maxx - minx );
			_height = ( maxy - miny );

			//_lumelWidth = (int)( _width / Lightmap.LUMEL_SIZE );
			//_lumelHeight = (int)( _height / Lightmap.LUMEL_SIZE );
			_lumelWidth = (int)( _width / _lumelSize );
			_lumelHeight = (int)( _height / _lumelSize );

			var stepx = _width / _lumelWidth;
			var stepy = _height / _lumelHeight;

			for( int y = 0; y < _lumelHeight; y++ )
			{
				for( int x = 0; x < _lumelWidth; x++ )
				{
					var p0 = xaxis * ( minx + stepx * ( x + 0.5f ) ) + yaxis * ( miny + stepy * ( y + 0.5f ) ) + _plane.Normal * z;

					var behindAll = true;
					for( int j = 0; j < otherPlanes.Length && behindAll; j++ )
					{
						if( otherPlanes[j].InFront( p0 ) )
						{
							behindAll = false;
						}
					}

					if( behindAll )
					{
						_lumels.Add( new Lumel( p0, _plane.Normal ) { Parent = parent } );
					}
				}
			}
		}

		public void BuildLightmapUVs( int x, int y )
		{
			_lightmapUVs.Clear();

			if( _vertices.Count <= 0 )
				return;

			var xaxis = _tangent;
			xaxis.Normalize();

			var yaxis = _bitangent;
			yaxis.Normalize();

			var minx = _vertices.Min( a => a.Dot( xaxis ) );
			var miny = _vertices.Min( a => a.Dot( yaxis ) );
			var maxx = _vertices.Max( a => a.Dot( xaxis ) );
			var maxy = _vertices.Max( a => a.Dot( yaxis ) );

			var width = ( maxx - minx );
			var height = ( maxy - miny );

			for( int i = 0; i < _vertices.Count; i++ )
			{
				var v = _vertices[i];
				var curx = v.Dot( xaxis );
				var cury = v.Dot( yaxis );

				curx = ( curx - minx ) / width;
				cury = ( cury - miny ) / height;

				var finalx = x + curx * _lumelWidth;
				var finaly = y + cury * _lumelHeight;

				_lightmapUVs.Add( new PointF( finalx, finaly ) );
			}
		}
	}
}
