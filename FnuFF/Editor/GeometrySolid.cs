using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;

namespace Editor
{
    public class GeometrySolid
    {
		private static Random random = new Random();
		
		private Color _color;
		private bool _hovered;
		private bool _selected;
		private List<Face> _faces;

		[XmlIgnore]
		public Color Color { get { return _color; } set { _color = value; } }

		[XmlIgnore]
		public bool Hovered { get { return _hovered; } set { _hovered = value; } }
		
		[XmlIgnore]
		public bool Selected { get { return _selected; } set { _selected = value; } }
		
		[XmlIgnore]
		public List<Face> Faces => _faces;

		public GeometrySolid()
		{
			_hovered = _selected = false;
			_faces = new List<Face>();

			GenerateColor();
		}

        public GeometrySolid(Triple min, Triple max)
        {
			_hovered = _selected = false;
			_faces = new List<Face>();

			GenerateColor();
			GenerateFaces(min, max);
        }

		private void GenerateColor()
		{
			var r = random.NextDouble();
			var g = random.NextDouble();
			var b = random.NextDouble();

			var magnitude = Math.Sqrt( r * r + g * g + b * b );

			r /= magnitude;
			g /= magnitude;
			b /= magnitude;

			_color = Color.FromArgb( (int)( r * 255 ), (int)( g * 255 ), (int)( b * 255 ) );
		}

		public void GenerateFaces(Triple min, Triple max)
		{
			// left
			var left = new Face();
			left.Plane.Normal = new Triple( 0, 0, -1 );
			left.Plane.D = min.Dot( left.Plane.Normal );
			left.Vertices[0] = new Triple( min.X, max.Y, max.Z );
			left.Vertices[1] = new Triple( min.X, min.Y, max.Z );
			left.Vertices[2] = new Triple( min.X, min.Y, min.Z );

			// right
			var right = new Face();
			right.Plane.Normal = new Triple( 0, 0, 1 );
			right.Plane.D = max.Dot( right.Plane.Normal );
			right.Vertices[0] = new Triple( max.X, max.Y, min.Z );
			right.Vertices[1] = new Triple( max.X, min.Y, min.Z );
			right.Vertices[2] = new Triple( max.X, min.Y, max.Z );

			// top
			var top = new Face();
			top.Plane.Normal = new Triple( 0, 1, 0 );
			top.Plane.D = max.Dot( top.Plane.Normal );
			top.Vertices[0] = new Triple( min.X, max.Y, max.Z );
			top.Vertices[1] = new Triple( min.X, max.Y, min.Z );
			top.Vertices[2] = new Triple( max.X, max.Y, min.Z );

			// bottom
			var bottom = new Face();
			bottom.Plane.Normal = new Triple( 0, -1, 0 );
			bottom.Plane.D = min.Dot( bottom.Plane.Normal );
			bottom.Vertices[0] = new Triple( min.X, min.Y, min.Z );
			bottom.Vertices[1] = new Triple( min.X, min.Y, max.Z );
			bottom.Vertices[2] = new Triple( max.X, min.Y, max.Z );

			// front
			var front = new Face();
			front.Plane.Normal = new Triple( -1, 0, 0 );
			front.Plane.D = min.Dot( front.Plane.Normal );
			front.Vertices[0] = new Triple( min.X, max.Y, min.Z );
			front.Vertices[1] = new Triple( min.X, min.Y, min.Z );
			front.Vertices[2] = new Triple( max.X, min.Y, min.Z );

			// back
			var back = new Face();
			back.Plane.Normal = new Triple( 1, 0, 0 );
			back.Plane.D = max.Dot( back.Plane.Normal );
			back.Vertices[0] = new Triple( max.X, max.Y, max.Z );
			back.Vertices[1] = new Triple( max.X, min.Y, max.Z );
			back.Vertices[2] = new Triple( min.X, min.Y, max.Z );

			_faces.AddRange( new[] { left, right, top, bottom, front, back } );
		}

		public void Paint3D()
		{
			GL.BeginTriangles();
			//GL.BeginPoints();

			float red = _color.R / 255.0f;
			float green = _color.G / 255.0f;
			float blue = _color.B / 255.0f;
			float alpha = 1.0f;

			if( _selected )
			{
				red = green = blue = 1.0f;
			}

			GL.Color4f( 1.0f, 1.0f, 1.0f, 1.0f );

			/*for( int i = 0; i < _faces.Count; i++ )
			{
				var points = new List<Triple>();
				var f0 = _faces[i];

				var validFaces = new List<Face>();
				for( int j = 0; j < _faces.Count; j++ )
				{
					if( i != j )
					{
						if( f0.Normal.Dot( _faces[j].Normal ) > -1 + Extensions.EPSILON )
							validFaces.Add( _faces[j] );
					}
				}

				for( int j = 0; j < validFaces.Count; j++ )
				{
					var f1 = validFaces[j];

					for( int k = 0; k < validFaces.Count; k++ )
					{
						if( j != k )
						{
							var f2 = validFaces[k];

							var div = f1.Normal.Cross( f2.Normal ).Dot( f0.Normal );
							if( Math.Abs( div ) > Extensions.EPSILON )
							{
								var a = f1.Normal.Cross( f2.Normal ) * ( f0.D );
								var b = f2.Normal.Cross( f0.Normal ) * ( f1.D );
								var c = f0.Normal.Cross( f1.Normal ) * ( f2.D );
								
								var sum = a + b + c;
								var result = sum / div;

								if(!points.Contains(result))
									points.Add( result );
							}
						}
					}
				}

				var indices = Extensions.WindingIndex3D( points.ToArray(), f0.Normal );
				var v0 = points[indices[0]];
				for( int j = 1; j < indices.Length-1; j++ )
				{
					var i1 = indices[j];
					var i2 = indices[j + 1];

					var v1 = points[i1];
					var v2 = points[i2];

					var v1v0 = v0 - v1;
					var v2v0 = v0 - v2;

					var n = v2v0.Cross( v1v0 );
					n.Normalize();

					var flip = f0.Normal.Dot( n ) > 0;
					if( flip )
					{
						var temp = v2;
						v2 = v1;
						v1 = temp;
					}

					GL.Vertex3f( v0.X, v0.Y, v0.Z );
					GL.Vertex3f( v1.X, v1.Y, v1.Z );
					GL.Vertex3f( v2.X, v2.Y, v2.Z );
				}
			}*/

			foreach( var face in _faces )
			{
				var otherPlanes = _faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();
				var points = Extensions.IntersectPlanes( face.Plane, otherPlanes );
				var indices = Extensions.WindingIndex3D( points, face.Plane.Normal );

				var v0 = points[indices[0]];
				for( int j = 1; j < indices.Length - 1; j++ )
				{
					var i1 = indices[j];
					var i2 = indices[j + 1];

					var v1 = points[i1];
					var v2 = points[i2];

					var v1v0 = v0 - v1;
					var v2v0 = v0 - v2;

					var n = v2v0.Cross( v1v0 );
					n.Normalize();

					var flip = face.Plane.Normal.Dot( n ) > 0;
					if( flip )
					{
						var temp = v2;
						v2 = v1;
						v1 = temp;
					}

					GL.Vertex3f( v0.X, v0.Y, v0.Z );
					GL.Vertex3f( v1.X, v1.Y, v1.Z );
					GL.Vertex3f( v2.X, v2.Y, v2.Z );
				}
			}

			GL.End();
		}

		/*public Rectangle Project( Camera2D camera, int gridGap, int gridSize )
		{
			var gmin = camera.Project( _min );
			var gmax = camera.Project( _max );
			
			gmin = gmin.Inflate( gridGap );
			gmax = gmax.Inflate( gridGap );

			gmin = gmin.Deflate( gridSize );
			gmax = gmax.Deflate( gridSize );

			var lmin = camera.ToLocal( gmin );
			var lmax = camera.ToLocal( gmax );

			var result = Extensions.FromMinMax( lmin, lmax );

			return result;
		}

		public void Unproject( Camera2D camera, Rectangle bounds, int gridGap, int gridSize )
		{
			var lmin = new Point( bounds.Left, bounds.Top );
			var lmax = new Point( bounds.Right, bounds.Bottom );

			var gmin = camera.ToGlobal( lmin );
			var gmax = camera.ToGlobal( lmax );

			var minTriple = camera.Unproject( gmin, (int)_min.Dot(camera.Direction)*gridGap );
			var maxTriple = camera.Unproject( gmax, (int)_max.Dot(camera.Direction)*gridGap );

			Extensions.MinMax( ref minTriple, ref maxTriple );

			minTriple.Deflate( gridGap );
			maxTriple.Deflate( gridGap );

			minTriple.Inflate( gridSize );
			maxTriple.Inflate( gridSize );

			_min = minTriple;
			_max = maxTriple;

			_points.Clear();
			GeneratePoints();
		}*/
    }
}
