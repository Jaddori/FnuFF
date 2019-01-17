using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Editor
{
	public static class Extensions
	{
		public const float EPSILON = 0.001f;

		/*public static readonly Triple[] BASE_AXIS =
		{
			new Triple(0,0,1), new Triple(1,0,0), new Triple(0,-1,0),
			new Triple(0,0,-1), new Triple(1,0,0), new Triple(0,-1,0),
			new Triple(1,0,0), new Triple(0,1,0), new Triple(0,0,-1),
			new Triple(-1,0,0), new Triple(0,1,0), new Triple(0,0,-1),
			new Triple(0,1,0), new Triple(1,0,0), new Triple(0,0,-1),
			new Triple(0,-1,0), new Triple(1,0,0), new Triple(0,0,-1)
		};*/

		public static readonly Triple[] BASE_AXIS =
		{
			new Triple(0,1,0),	new Triple(1,0,0), new Triple(0,0,1),
			new Triple(0,-1,0), new Triple(-1,0,0), new Triple(0,0,-1),
			new Triple(0,0,1),	new Triple(1,0,0), new Triple(0,-1,0),
			new Triple(0,0,-1), new Triple(-1,0,0), new Triple(0,-1,0),
			new Triple(1,0,0),	new Triple(0,0,-1), new Triple(0,-1,0),
			new Triple(-1,0,0),	new Triple(0,0,1), new Triple(0,-1,0)
		};
		
		public static RectangleF FromMinMax( PointF min, PointF max )
		{
			var result = new RectangleF();

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

		public static RectangleF FromPoint( PointF center, int size )
		{
			return new RectangleF( center.X - size / 2, center.Y - size / 2, size, size );
		}

		public static RectangleF FromPoints( PointF[] points )
		{
			var result = new RectangleF();

			if( points.Length > 1 )
			{
				var topLeft = new PointF( points.Min( x => x.X ), points.Min( x => x.Y ) );
				var bottomRight = new PointF( points.Max( x => x.X ), points.Max( x => x.Y ) );

				result = new RectangleF( topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y );
			}

			return result;
		}

		public static Rectangle Downcast( this RectangleF r )
		{
			return new Rectangle( (int)r.X, (int)r.Y, (int)r.Width, (int)r.Height );
		}

		public static PointF GetCenter( this RectangleF r )
		{
			return new PointF( r.Left + r.Width / 2, r.Top + r.Height / 2 );
		}

		public static PointF GetCenter( PointF min, PointF max )
		{
			return new PointF( min.X + ( max.X - min.X ) * 0.5f, min.Y + ( max.Y - min.Y ) * 0.5f );
		}

		public static PointF[] GetHandlePoints( RectangleF r )
		{
			var center = GetCenter( r );

			var result = new PointF[]
			{
				new PointF(r.Left, r.Top),
				new PointF(center.X, r.Top),
				new PointF(r.Right, r.Top),
				new PointF(r.Left, center.Y),
				//new PointF(center.X, center.Y),
				new PointF(r.Right, center.Y),
				new PointF(r.Left, r.Bottom),
				new PointF(center.X, r.Bottom),
				new PointF(r.Right, r.Bottom)
			};

			return result;
		}

		public static RectangleF[] GetHandles( RectangleF r, int size )
		{
			var points = GetHandlePoints( r );

			var result = new RectangleF[points.Length];
			for( int i = 0; i < points.Length; i++ )
				result[i] = FromPoint( points[i], size );

			return result;
		}

		public static PointF TopLeft( this RectangleF r )
		{
			return new PointF( r.Left, r.Top );
		}

		public static PointF TopRight( this RectangleF r )
		{
			return new PointF( r.Right, r.Top );
		}

		public static PointF BottomLeft( this RectangleF r )
		{
			return new PointF( r.Left, r.Bottom );
		}

		public static PointF BottomRight( this RectangleF r )
		{
			return new PointF( r.Right, r.Bottom );
		}

		public static int HandleIndex( int x, int y )
		{
			//return ( y * 3 + x );

			var index = y * 3 + x;
			if( x > 0 && y > 0 )
				index--;

			return index;
		}

		public static PointF Min( PointF a, PointF b )
		{
			var result = new PointF( Math.Min( a.X, b.X ), Math.Min( a.Y, b.Y ) );
			return result;
		}

		public static PointF Max( PointF a, PointF b )
		{
			var result = new PointF( Math.Max( a.X, b.X ), Math.Max( a.Y, b.Y ) );
			return result;
		}

		public static PointF Add( this PointF a, PointF b )
		{
			return new PointF( a.X + b.X, a.Y + b.Y );
		}

		public static PointF Sub( this PointF a, PointF b )
		{
			return new PointF( a.X - b.X, a.Y - b.Y );
		}

		public static PointF Mul( this PointF a, PointF b )
		{
			return new PointF( a.X * b.X, a.Y * b.Y );
		}

		public static PointF Mul( this PointF a, float b )
		{
			return new PointF( a.X * b, a.Y * b );
		}

		public static PointF Div( this PointF a, PointF b )
		{
			return new PointF( a.X / b.X, a.Y / b.Y );
		}

		public static PointF Div( this PointF a, float b )
		{
			return new PointF( a.X / b, a.Y / b );
		}

		public static Triple ToTriple( this PointF point )
		{
			return new Triple( point.X, point.Y, 0 );
		}

		public static Triple TripleLerp( Triple a, Triple b, float t )
		{
			return a + ( b - a ) * t;
		}

		public static void MinMax( ref PointF min, ref PointF max )
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

		public static PointF Inflate( this PointF point, float value )
		{
			return new PointF( point.X * value, point.Y * value );
		}

		public static PointF Deflate( this PointF point, float value )
		{
			return new PointF( point.X / value, point.Y / value );
		}

		public static PointF PointLerp( PointF a, PointF b, float t )
		{
			return new PointF( a.X + ( b.X - a.X ) * t, a.Y + ( b.Y - a.Y ) * t );
		}

		public static PointF Normalize( PointF point )
		{
			var len = (float)Math.Sqrt( point.X * point.X + point.Y * point.Y );
			return new PointF( point.X / len, point.Y / len );
		}

		public static PointF[] WindingSort2D( PointF[] points )
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

		public static PointF[] WindingSort3D( Triple[] points, Triple normal )
		{
			normal.Normalize();
			var projectedPoints = points.Select( x => x.Project( normal ) ).ToArray();

			return WindingSort2D( projectedPoints );
		}

		public static int[] WindingIndex2D( PointF[] points )
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

			var sorted = points.Select( ( x, i ) => new { point = x, index = i } ).OrderBy( x => Math.Atan2( x.point.Y - cy, x.point.X - cx ) ).Select( x => x.index ).ToArray();
			return sorted;
		}

		/*public static int[] WindingIndex3D( Triple[] points, Triple normal )
		{
			var projectedPoints = ProjectPoints( points, normal );

			if( projectedPoints == null )
				return null;

			return WindingIndex2D( projectedPoints );
		}*/

		public static int[] WindingIndex3D( Triple[] points, Triple normal )
		{
			var xaxis = new Triple();
			var yaxis = new Triple();

			TextureAxisFromPlane( normal, ref xaxis, ref yaxis );

			normal.Normalize();

			var projectedPoints = points.Select( x => new PointF( x.Dot( xaxis ), x.Dot( yaxis ) ) ).ToArray();

			return WindingIndex2D( projectedPoints );
		}

		public static void TextureAxisFromPlane( Triple normal, ref Triple xaxis, ref Triple yaxis )
		{
			var bestAxis = 0;
			float dot = 0.0f, best = 0.0f;

			for( int i = 0; i < 6; i++ )
			{
				dot = normal.Dot( BASE_AXIS[i * 3] );
				if( dot > best )
				{
					best = dot;
					bestAxis = i;
				}
			}

			xaxis = BASE_AXIS[bestAxis * 3 + 1];
			yaxis = BASE_AXIS[bestAxis * 3 + 2];
		}

		public static PointF EmitTextureCoordinates( Triple normal, Triple point, Face face )
		{
			point /= Grid.SIZE_BASE;

			Triple xaxis = new Triple();
			Triple yaxis = new Triple();
			TextureAxisFromPlane( normal, ref xaxis, ref yaxis );

			float ang = face.Rotation / 180.0f * (float)Math.PI;
			float sinv = (float)Math.Sin( ang );
			float cosv = (float)Math.Cos( ang );
			
			float s = point.Dot( xaxis );
			float t = point.Dot( yaxis );

			float ns = cosv * s - sinv * t;
			float nt = sinv * s + cosv * t;

			s = ns / face.Scale.X + face.Offset.X;
			t = nt / face.Scale.Y + face.Offset.Y;
			
			return new PointF( s, t );
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

		public static int IndexFromXY( int x, int y, int width )
		{
			return ( y * width + x );
		}

		public static void Write( this BinaryWriter writer, Face f, float divider = 1.0f )
		{
			writer.Write( f.Plane, divider );
		}

		public static void Write( this BinaryWriter writer, Plane p, float divider = 1.0f )
		{
			writer.Write( p.Normal );
			writer.Write( p.D / divider );
		}

		public static void Write( this BinaryWriter writer, Triple t, float divider = 1.0f )
		{
			writer.Write( t.X / divider );
			writer.Write( t.Y / divider );
			writer.Write( t.Z / divider );
		}

		public static void Write( this BinaryWriter writer, PointF p, float divider = 1.0f )
		{
			writer.Write( p.X / divider );
			writer.Write( p.Y / divider );
		}

		public static void Write( this StreamWriter writer, Face f, float divider = 1.0f )
		{
			writer.Write( f.Plane, divider );
		}

		public static void Write( this StreamWriter writer, Plane p, float divider = 1.0f )
		{
			writer.Write( p.Normal );
			writer.WriteLine( ( p.D / divider ).ToString() );
		}

		public static void Write( this StreamWriter writer, Triple t, float divider = 1.0f )
		{
			writer.WriteLine( ( t.X / divider ).ToString() );
			writer.WriteLine( ( t.Y / divider ).ToString() );
			writer.WriteLine( ( t.Z / divider ).ToString() );
		}

		public static void Write( this StreamWriter writer, PointF p, float divider = 1.0f )
		{
			writer.WriteLine( ( p.X / divider ).ToString() );
			writer.WriteLine( ( p.Y / divider ).ToString() );
		}

		public static string NameFromPath( this string path, bool includeExtension = false )
		{
			var slashIndex = 0;
			if( path.Contains( '/' ) )
				slashIndex = path.LastIndexOf( '/' );
			if( path.Contains( '\\' ) )
			{
				var backslashIndex = path.LastIndexOf( '\\' );
				if( backslashIndex > slashIndex )
					slashIndex = backslashIndex;
			}

			var dotIndex = path.Length;
			if( path.Contains( '.' ) && !includeExtension )
			{
				dotIndex = path.LastIndexOf( '.' );
			}

			return path.Substring( slashIndex + 1, dotIndex - slashIndex - 1 );
		}

		public static string ExtensionFromPath( this string path )
		{
			var dotIndex = path.LastIndexOf( '.' );
			return path.Substring( dotIndex + 1 ).Trim();
		}
	}
}
