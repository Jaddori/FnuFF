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

		public static void MinMax( ref Point a, ref Point b )
		{
			if( a.X < b.X )
			{
				int temp = b.X;
				b.X = a.X;
				a.X = temp;
			}

			if( a.Y < b.Y )
			{
				int temp = b.Y;
				b.Y = a.Y;
				a.Y = temp;
			}
		}
	}
}
