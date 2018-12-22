using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Editor
{
	public static class Extensions
	{
		public const float EPSILON = 0.001f;

		public static Rectangle FromMinMax( Point min, Point max )
		{
			var result = new Rectangle();

			if( min.X < max.X )
			{
				result.X = min.X;
				result.Width = max.X - min.X;
			}
			else
			{
				result.X = max.X;
				result.Width = min.X - max.X;
			}

			if( min.Y < max.Y )
			{
				result.Y = min.Y;
				result.Height = max.Y - min.Y;
			}
			else
			{
				result.Y = max.Y;
				result.Height = min.Y - max.Y;
			}

			return result;
		}

		public static Rectangle FromPoint( Point center, int size )
		{
			return new Rectangle( center.X - size / 2, center.Y - size / 2, size, size );
		}

		public static Point GetCenter( Rectangle r )
		{
			return new Point( r.Left + r.Width / 2, r.Top + r.Height / 2 );
		}

		public static Point[] GetHandlePoints( Rectangle r )
		{
			var center = GetCenter( r );

			var result = new Point[]
			{
				new Point(r.Left, r.Top),
				new Point(center.X, r.Top),
				new Point(r.Right, r.Top),
				new Point(r.Left, center.Y),
				new Point(center.X, center.Y),
				new Point(r.Right, center.Y),
				new Point(r.Left, r.Bottom),
				new Point(center.X, r.Bottom),
				new Point(r.Right, r.Bottom)
			};

			return result;
		}

		public static Rectangle[] GetHandles( Rectangle r, int size )
		{
			var points = GetHandlePoints( r );

			var result = new Rectangle[points.Length];
			for( int i = 0; i < points.Length; i++ )
				result[i] = FromPoint( points[i], size );

			return result;
		}

		public static int HandleIndex( int x, int y )
		{
			return ( y * 3 + x );
		}

		public static Point Min( Point a, Point b )
		{
			var result = new Point( Math.Min( a.X, b.X ), Math.Min( a.Y, b.Y ) );
			return result;
		}

		public static Point Max( Point a, Point b )
		{
			var result = new Point( Math.Max( a.X, b.X ), Math.Max( a.Y, b.Y ) );
			return result;
		}

		public static void MinMax( ref Point min, ref Point max )
		{
			if( min.X > max.X )
			{
				var temp = max.X;
				max.X = min.X;
				min.X = temp;
			}

			if( min.Y > max.Y )
			{
				var temp = max.Y;
				max.Y = min.Y;
				min.Y = temp;
			}
		}

		public static void MinMax( ref Triple min, ref Triple max )
		{
			if( min.X > max.X )
			{
				var temp = max.X;
				max.X = min.X;
				min.X = temp;
			}

			if( min.Y > max.Y )
			{
				var temp = max.Y;
				max.Y = min.Y;
				min.Y = temp;
			}

			if( min.Z > max.Z )
			{
				var temp = max.Z;
				max.Z = min.Z;
				min.Z = temp;
			}
		}

		public static Point Inflate( this Point point, float value )
		{
			return new Point((int)( point.X * value ), (int)( point.Y * value ));
		}

		public static Point Deflate( this Point point, float value )
		{
			return new Point((int)( point.X / value ), (int)( point.Y / value ));
		}

		public static Point[] WindingSort2D( Point[] points )
		{
			var cx = 0.0f;
			var cy = 0.0f;
			foreach( var p in points )
			{
				cx += p.X;
				cy += p.Y;
			}

			cx /= points.Length;
			cy /= points.Length;

			var sorted = points.OrderBy( x => Math.Atan2( x.Y - cy, x.X - cx ) ).ToArray();
			return sorted;
		}

		public static Point[] WindingSort3D( Triple[] points, Triple normal )
		{
			normal.Normalize();
			var projectedPoints = points.Select( x => x.Project( normal ) ).ToArray();

			return WindingSort2D( projectedPoints );
		}

		public static int[] WindingIndex2D( Point[] points )
		{
			var cx = 0.0f;
			var cy = 0.0f;
			foreach( var p in points )
			{
				cx += p.X;
				cy += p.Y;
			}

			cx /= points.Length;
			cy /= points.Length;

			var sorted = points.Select((x,i) => new { point = x, index = i }).OrderBy( x => Math.Atan2( x.point.Y - cy, x.point.X - cx ) ).Select( x => x.index ).ToArray();
			return sorted;
		}

		public static int[] WindingIndex3D( Triple[] points, Triple normal )
		{
			normal.Normalize();
			var projectedPoints = points.Select( x => x.Project( normal ) ).ToArray();

			return WindingIndex2D( projectedPoints );
		}

		public static Triple[] IntersectPlanes( Plane p0, Plane[] ps )
		{
			var points = new List<Triple>();

			if( ps.Length > 1 )
			{
				// get planes that are not parallell and not coinciding
				var validPlanes = new List<Plane>();
				foreach( var p in ps )
				{
					if( p0.Normal.Dot( p.Normal ) > -1 + EPSILON )
						validPlanes.Add( p );
				}

				// get intersection points between planes
				var potentialPoints = new List<Triple>();
				for( int i = 0; i < validPlanes.Count; i++ )
				{
					var p1 = validPlanes[i];

					for( int j = 0; j < validPlanes.Count; j++ )
					{
						if( i != j )
						{
							var p2 = validPlanes[j];

							var div = p1.Normal.Cross( p2.Normal ).Dot( p0.Normal );
							if( Math.Abs( div ) > EPSILON )
							{
								var a = p1.Normal.Cross( p2.Normal ) * p0.D;
								var b = p2.Normal.Cross( p0.Normal ) * p1.D;
								var c = p0.Normal.Cross( p1.Normal ) * p2.D;

								var sum = a + b + c;
								var result = sum / div;

								if( !potentialPoints.Contains( result ) )
									potentialPoints.Add( result );
							}
						}
					}
				}

				// make sure all points are behind all planes
				foreach( var point in potentialPoints )
				{
					var isBehind = true;
					foreach( var plane in validPlanes )
					{
						if( plane.InFront( point ) )
						{
							isBehind = false;
							break;
						}
					}

					if( isBehind )
					{
						points.Add( point );
					}
				}
			}

			return points.ToArray();
		}
	}
}
