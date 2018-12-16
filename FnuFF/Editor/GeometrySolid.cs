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
		private List<int> _indices;

        public Triple Min { get { return _min; } set { _min = value; } }
        public Triple Max { get { return _max; } set { _max = value; } }

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

		public GeometrySolid()
		{
			_min = new Triple();
			_max = new Triple();

			_hovered = _selected = false;

			_points = new List<Triple>();
			_indices = new List<int>();

			GenerateColor();
			GeneratePoints();
		}

        public GeometrySolid(Triple min, Triple max)
        {
            _min = min;
            _max = max;

			_hovered = _selected = false;

			_points = new List<Triple>();
			_indices = new List<int>();

			GenerateColor();
			GeneratePoints();
			GenerateIndices();
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
										if( !plane.InFront( point ) )
											allBehind = false;
									}
								}

								if( allBehind )
								{
									//_indices.AddRange( new[] { i, j, k } );
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
					/*var v0 = pointsOnPlane[0];
					for( int i = 1; i < pointsOnPlane.Count-1; i++ )
					{
						var v1 = pointsOnPlane[i];
						var v2 = pointsOnPlane[i + 1];

						_indices.AddRange( new[] { v0, v1, v2 } );
					}*/

					var innerIndices = new List<int>();
					innerIndices.Add( pointsOnPlane[0] );
					for( int i = 0; i < pointsOnPlane.Count; i++ )
					{
						var i0 = pointsOnPlane[i];
						var v0 = _points[i0];

						var added = false;
						for( int j = 0; j < pointsOnPlane.Count && !added; j++ )
						{
							if( i != j )
							{
								var i1 = pointsOnPlane[j];
								var v1 = _points[i1];

								var v1v0 = v0 - v1;
								v1v0.Normalize();

								var innerNormal = v1v0.Cross( plane.Normal );
								innerNormal.Normalize();

								var distanceAlongNormal = v0.Dot( innerNormal );

								var allBehind = true;
								for( int k = 0; k < pointsOnPlane.Count && allBehind; k++ )
								{
									if( i != k && j != k )
									{
										var innerPlane = new Plane( innerNormal, distanceAlongNormal );

										var index = pointsOnPlane[k];
										var point = _points[index];
										if( innerPlane.InFront( point ) )
											allBehind = false;
									}
								}

								if( allBehind )
								{
									if( !innerIndices.Contains( i1 ) )
									{
										innerIndices.Add( i1 );
										added = true;
									}
								}
							}
						}
					}

					var vi0 = innerIndices[0];
					for( int i = 1; i < innerIndices.Count - 1; i++ )
					{
						var vi1 = innerIndices[i];
						var vi2 = innerIndices[i + 1];

						_indices.AddRange( new[] { vi0, vi1, vi2 } );
					}
				}
			}
		}

		public void Paint3D()
		{
			GL.Begin();

			for( int i = 0; i < _indices.Count; i += 3 )
			{
				var i1 = _indices[i];
				var i2 = _indices[i + 1];
				var i3 = _indices[i + 2];

				var v1 = _points[i1];
				var v2 = _points[i2];
				var v3 = _points[i3];

				GL.Vertex3f( v1.X, v1.Y, v1.Z );
				GL.Vertex3f( v2.X, v2.Y, v2.Z );
				GL.Vertex3f( v3.X, v3.Y, v3.Z );
			}

			GL.End();
		}
    }
}
