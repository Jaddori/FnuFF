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
	}
}
