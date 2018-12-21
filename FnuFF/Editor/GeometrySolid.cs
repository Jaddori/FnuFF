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

        private Triple _min;
        private Triple _max;
		private Color _color;
		private bool _hovered;
		private bool _selected;
		private List<Triple> _points;
		private List<Triple> _coords;
		private List<int> _indices;
		private List<Face> _faces;

        public Triple Min { get { return _min; } set { _min = value; _points.Clear(); GeneratePoints(); } }
        public Triple Max { get { return _max; } set { _max = value; _points.Clear(); GeneratePoints(); } }

		[XmlIgnore]
		public Color Color { get { return _color; } set { _color = value; } }

		[XmlIgnore]
		public bool Hovered { get { return _hovered; } set { _hovered = value; } }
		
		[XmlIgnore]
		public bool Selected { get { return _selected; } set { _selected = value; } }

		[XmlIgnore]
		public List<Triple> Points => _points;

		[XmlIgnore]
		public List<int> Indices => _indices;

		[XmlIgnore]
		public List<Face> Faces => _faces;

		public GeometrySolid()
		{
			_min = new Triple();
			_max = new Triple();

			_hovered = _selected = false;

			_points = new List<Triple>();
			_coords = new List<Triple>();
			_indices = new List<int>();
			_faces = new List<Face>();

			GenerateColor();
			//GeneratePoints();
		}

        public GeometrySolid(Triple min, Triple max)
        {
            _min = min;
            _max = max;

			_hovered = _selected = false;

			_points = new List<Triple>();
			_coords = new List<Triple>();
			_indices = new List<int>();
			_faces = new List<Face>();

			GenerateColor();
			GeneratePoints();
			//GenerateIndices();
			GenerateFaces();
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

		private void GeneratePoints()
		{
			// bottom
			Triple v1 = _min;
			var v2 = new Triple( _max.X, _min.Y, _min.Z );
			var v3 = new Triple( _max.X, _min.Y, _max.Z );
			var v4 = new Triple( _min.X, _min.Y, _max.Z );

			// top
			Triple v5 = _max;
			var v6 = new Triple( _min.X, _max.Y, _max.Z );
			var v7 = new Triple( _min.X, _max.Y, _min.Z );
			var v8 = new Triple( _max.X, _max.Y, _min.Z );

			_points.AddRange( new[]{ v1, v2, v3, v4, v5, v6, v7, v8} );
		}

		public void GenerateFaces()
		{
			// left
			var left = new Face();
			left.Points.AddRange( new[] { _points[7], _points[1], _points[0], _points[6] } );
			left.Indices.AddRange( new[] { 0, 1, 2, 0, 2, 3 } );
			left.Normal = new Triple( 0, 0, -1 );
			left.D = _points[7].Dot( left.Normal );

			// right
			var right = new Face();
			right.Points.AddRange( new[] { _points[5], _points[3], _points[2], _points[4] } );
			right.Indices.AddRange( new[] { 0, 1, 2, 0, 2, 3 } );
			right.Normal = new Triple( 0, 0, 1 );
			right.D = _points[5].Dot( right.Normal );

			// top
			var top = new Face();
			top.Points.AddRange( new[] { _points[7], _points[6], _points[5], _points[4] } );
			top.Indices.AddRange( new[] { 0, 1, 2, 0, 2, 3 } );
			top.Normal = new Triple( 0, 1, 0 );
			top.D = _points[7].Dot( top.Normal );

			// bottom
			var bottom = new Face();
			bottom.Points.AddRange( new[] { _points[0], _points[1], _points[2], _points[3] } );
			bottom.Indices.AddRange( new[] { 0, 1, 2, 0, 2, 3 } );
			bottom.Normal = new Triple( 0, -1, 0 );
			bottom.D = _points[0].Dot( bottom.Normal );

			// front
			var front = new Face();
			front.Points.AddRange( new[] { _points[6], _points[0], _points[3], _points[5] } );
			front.Indices.AddRange( new[] { 0, 1, 2, 0, 2, 3 } );
			front.Normal = new Triple( -1, 0, 0 );
			front.D = _points[6].Dot( front.Normal );

			// back
			var back = new Face();
			back.Points.AddRange( new[] { _points[4], _points[2], _points[1], _points[7] } );
			back.Indices.AddRange( new[] { 0, 1, 2, 0, 2, 3 } );
			back.Normal = new Triple( 1, 0, 0 );
			back.D = _points[4].Dot( back.Normal );

			_faces.AddRange( new[] { left, right, top, bottom, front, back } );
		}

		public void GenerateIndices()
		{
			_indices.Clear();

			var planes = new List<Plane>();
			for( int i = 0; i < _points.Count; i++ )
			{
				var v0 = _points[i];

				for( int j = 0; j < _points.Count; j++ )
				{
					if( i != j )
					{
						var v1 = _points[j];

						for( int k = 0; k < _points.Count; k++ )
						{
							if( i != k && j != k )
							{
								var v2 = _points[k];

								var v2v0 = v0 - v2;
								var v1v0 = v0 - v1;

								var normal = v2v0.Cross( v1v0 );
								normal.Normalize();

								var distanceAlongNormal = v0.Dot( normal );

								var plane = new Plane() { Normal = normal, D = distanceAlongNormal };

								var allBehind = true;
								for( int a = 0; a < _points.Count && allBehind; a++ )
								{
									if( i != a && j != a && k != a )
									{
										var point = _points[a];
										if( plane.InFront( point ) )
											allBehind = false;
									}
								}

								if( allBehind )
								{
									var newPlane = true;
									for( int a = 0; a < planes.Count && newPlane; a++ )
									{
										if( planes[a].Equals(plane) )
											newPlane = false;
									}

									if( newPlane )
										planes.Add( plane );
								}
							}
						}
					}
				}
			}

			foreach( var plane in planes )
			{
				var pointsOnPlane = new List<int>();
				for( int i = 0; i < _points.Count; i++ )
				{
					if( plane.OnPlane( _points[i] ) )
						pointsOnPlane.Add( i );
				}

				if( pointsOnPlane.Count > 0 )
				{
					var projectedPoints = new List<Point>();
					for(int i=0; i<pointsOnPlane.Count; i++)
					{
						var projectedPoint = _points[pointsOnPlane[i]].Project( plane.Normal );
						projectedPoints.Add( projectedPoint );
					}

					var startIndex = 0;
					var bottomLeft = projectedPoints[startIndex];
					for( int i = 1; i < projectedPoints.Count; i++ )
					{
						var point = projectedPoints[i];
						if( Math.Abs( point.X - bottomLeft.X ) < 0.001f )
						{
							if( point.Y < bottomLeft.Y )
							{
								bottomLeft = point;
								startIndex = i;
							}
						}
						else if( point.X < bottomLeft.X )
						{
							bottomLeft = point;
							startIndex = i;
						}
					}

					var index = startIndex;
					var innerIndices = new List<int>();
					innerIndices.Add( index );

					for( int i = 0; i < projectedPoints.Count-1; i++ )
					{
						var outerPoint = projectedPoints[index];

						var nextIndex = i;
						var smallestAngle = 9999.0f;
						for( int j = 0; j < projectedPoints.Count; j++ )
						{
							if( j != index )
							{
								var innerPoint = projectedPoints[j];

								var angle = (float)Math.Atan2( innerPoint.Y-outerPoint.Y, innerPoint.X- outerPoint.X );

								if( angle < 0 )
									angle += (float)(Math.PI * 2);

								if( angle < smallestAngle )
								{
									nextIndex = j;
									smallestAngle = angle;
								}
							}
						}
							
						innerIndices.Add( nextIndex );
						index = nextIndex;
					}
					
					var vi0 = innerIndices[0];

					var face = new Face();
					face.Normal = plane.Normal;
					face.D = plane.D;

					for( int i = 0; i < innerIndices.Count; i++ )
						face.Points.Add( _points[pointsOnPlane[innerIndices[i]]] );

					for( int i = 1; i < innerIndices.Count - 1; i++ )
					{
						var vi1 = innerIndices[i];
						var vi2 = innerIndices[i + 1];

						var ai0 = pointsOnPlane[vi0];
						var ai1 = pointsOnPlane[vi1];
						var ai2 = pointsOnPlane[vi2];

						var v0 = _points[ai0];
						var v1 = _points[ai1];
						var v2 = _points[ai2];

						var v2v0 = v0 - v2;
						var v1v0 = v0 - v1;

						var normal = v2v0.Cross( v1v0 );
						normal.Normalize();

						var flip = ( normal.Dot( plane.Normal ) > 0 );

						if( flip )
						{
							var temp = ai2;
							ai2 = ai1;
							ai1 = temp;
						}

						//var face = new Face();
						//face.Points.AddRange( new[] { v0, v1, v2 } );
						//face.Normal = plane.Normal;
						//face.D = plane.D;

						_indices.AddRange( new[] { ai0, ai1, ai2 } );
						face.Indices.AddRange( new[] { ai0, ai1, ai2 } );

						_coords.Add( new Triple( 0, 0, 0 ) );
						face.Coords.Add( new Triple( 0, 0, 0 ) );
						if( (i == 1 && !flip) || ( i > 1 && flip) )
						{
							_coords.Add( new Triple( 0, 1, 0 ) );
							_coords.Add( new Triple( 1, 1, 0 ) );
							face.Coords.Add( new Triple( 0, 1, 0 ) );
							face.Coords.Add( new Triple( 1, 1, 0 ) );
						}
						else
						{
							_coords.Add( new Triple( 1, 1, 0 ) );
							_coords.Add( new Triple( 1, 0, 0 ) );
							face.Coords.Add( new Triple( 1, 1, 0 ) );
							face.Coords.Add( new Triple( 1, 0, 0 ) );
						}

						//_faces.Add( face );
					}

					_faces.Add( face );
				}
			}
		}

		public void Paint3D()
		{
			GL.BeginTriangles();

			float r = _color.R / 255.0f;
			float g = _color.G / 255.0f;
			float b = _color.B / 255.0f;
			float a = 1.0f;

			if( _selected )
			{
				r = g = b = 1.0f;
			}

			GL.Color4f( 1.0f, 1.0f, 1.0f, 1.0f );
			/*for( int i = 0; i < _indices.Count; i += 3 )
			{
				var i1 = _indices[i];
				var i2 = _indices[i + 1];
				var i3 = _indices[i + 2];

				var v1 = _points[i1];
				var v2 = _points[i2];
				var v3 = _points[i3];

				var c1 = _coords[i];
				var c2 = _coords[i + 1];
				var c3 = _coords[i + 2];

				//GL.Color4f( r, g, b, a );
				GL.TexCoord2f( c1.X, c1.Y );
				GL.Vertex3f( v1.X, v1.Y, v1.Z );

				//GL.Color4f( r, g, b, a );
				GL.TexCoord2f( c2.X, c2.Y );
				GL.Vertex3f( v2.X, v2.Y, v2.Z );

				//GL.Color4f( r, g, b, a );
				GL.TexCoord2f( c3.X, c3.Y );
				GL.Vertex3f( v3.X, v3.Y, v3.Z );
			}*/

			foreach( var face in _faces )
			{
				for( int i = 0; i < face.Indices.Count; i += 3 )
				{
					var i0 = face.Indices[i];
					var i1 = face.Indices[i+1];
					var i2 = face.Indices[i+2];

					var v0 = face.Points[i0];
					var v1 = face.Points[i1];
					var v2 = face.Points[i2];

					GL.Color4f( r, g, b, a );
					GL.Vertex3f( v0.X, v0.Y, v0.Z );

					GL.Color4f( r, g, b, a );
					GL.Vertex3f( v1.X, v1.Y, v1.Z );

					GL.Color4f( r, g, b, a );
					GL.Vertex3f( v2.X, v2.Y, v2.Z );
				}
			}

			GL.End();
		}

		public Rectangle Project( Camera2D camera, int gridGap, int gridSize )
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
		}
    }
}
