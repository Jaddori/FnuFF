using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Editor
{
	public static class Grid
	{
		public const int MAX_LINES = 100;
		public const int SIZE_BASE = 64;
		public const int SIZE_MIN = 1;
		public const int SIZE_MAX = 512;
		public const int GAP_BASE = 128;

		public static int Size { get; set; } = SIZE_BASE;
		public static int Gap { get; set; } = GAP_BASE;

		public static void Paint2D( Graphics g, Camera2D camera, Size controlSize )
		{
			using( var pen = new Pen( EditorColors.GRID ) )
			{
				// horizontal
				for( int x = -MAX_LINES; x <= MAX_LINES; x++ )
				{
					var p1 = new PointF( x * Gap, -MAX_LINES * Gap );
					var p2 = new PointF( p1.X, MAX_LINES * Gap );

					var g1 = camera.ToLocal( p1 );
					var g2 = camera.ToLocal( p2 );

					if( g1.X > 0 && g1.X < controlSize.Width )
						g.DrawLine( pen, g1, g2 );
				}

				// vertical
				for( int y = -MAX_LINES; y <= MAX_LINES; y++ )
				{
					var p1 = new PointF( -MAX_LINES * Gap, y * Gap );
					var p2 = new PointF( MAX_LINES * Gap, p1.Y );

					var g1 = camera.ToLocal( p1 );
					var g2 = camera.ToLocal( p2 );

					if( g1.Y > 0 && g1.Y < controlSize.Height )
						g.DrawLine( pen, g1, g2 );
				}
			}
		}

		public static void Increase()
		{
			Size = Size << 1;

			if( Size < SIZE_MIN )
				Size = SIZE_MIN;
			else if( Size > SIZE_MAX )
				Size = SIZE_MAX;
		}

		public static void Decrease()
		{
			Size = Size >> 1;

			if( Size < SIZE_MIN )
				Size = SIZE_MIN;
			else if( Size > SIZE_MAX )
				Size = SIZE_MAX;
		}
	}
}
