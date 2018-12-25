using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Editor
{
	[DebuggerDisplay( "X = {X}, Y = {Y}, Z = {Z}" )]
	public struct Triple
	{
		private const float EPSILON = 0.0001f;

		public static Triple PosX = new Triple( 1, 0, 0 );
		public static Triple PosY = new Triple( 0, 1, 0 );
		public static Triple PosZ = new Triple( 0, 0, 1 );
		public static Triple NegX = new Triple( -1, 0, 0 );
		public static Triple NegY = new Triple( 0, -1, 0 );
		public static Triple NegZ = new Triple( 0, 0, -1 );

		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public Triple( float x, float y, float z )
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static bool operator ==( Triple a, Triple b )
		{
			return
			(
				Math.Abs( a.X - b.X ) < EPSILON &&
				Math.Abs( a.Y - b.Y ) < EPSILON &&
				Math.Abs( a.Z - b.Z ) < EPSILON
			);
		}

		public static bool operator !=( Triple a, Triple b )
		{
			return !( a == b );
		}

		public static Triple operator +( Triple a, Triple b )
		{
			return new Triple( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
		}

		public static Triple operator -( Triple t )
		{
			return new Triple( -t.X, -t.Y, -t.Z );
		}

		public static Triple operator -( Triple a, Triple b )
		{
			return new Triple( a.X - b.X, a.Y - b.Y, a.Z - b.Z );
		}

		public static Triple operator *( Triple a, Triple b )
		{
			return new Triple( a.X * b.X, a.Y * b.Y, a.Z * b.Z );
		}

		public static Triple operator *( Triple a, float b )
		{
			return new Triple( a.X * b, a.Y * b, a.Z * b );
		}

		public static Triple operator /( Triple a, Triple b )
		{
			return new Triple( a.X / b.X, a.Y / b.Y, a.Z / b.Z );
		}

		public static Triple operator /( Triple a, float b )
		{
			return new Triple( a.X / b, a.Y / b, a.Z / b );
		}

		public float Length()
		{
			return (float)Math.Sqrt( X * X + Y * Y + Z * Z );
		}

		public float Dot( Triple t )
		{
			return ( X * t.X + Y * t.Y + Z * t.Z );
		}

		public Triple Cross( Triple t )
		{
			return new Triple
			(
				Y*t.Z - Z*t.Y,
				Z*t.X - X*t.Z,
				X*t.Y - Y*t.X
			);
		}

		public void Normalize()
		{
			var magnitude = (float)Math.Sqrt( X * X + Y * Y + Z * Z );
			X /= magnitude;
			Y /= magnitude;
			Z /= magnitude;
		}

		public PointF Project( Triple t )
		{
			var bx = t.Y * X;
			var az = t.X * Z;
			var cx = t.Z * X;

			var ay = t.X * Y;
			var bz = t.Y * Z;
			var cy = t.Z * Y;

			var x = bx + az + cx;
			var y = ay + bz + cy;

			return new PointF( x, y );
		}

		public PointF ProjectF( Triple t )
		{
			var mx = X;
			var my = Y;
			var mz = Z;

			if( t.X < 0 )
			{
				t.X *= -1.0f;
				mx *= -0.5f;
			}

			if( t.Y < 0 )
			{
				t.Y *= -1.0f;
				my *= -0.5f;
			}

			if( t.Z < 0 )
			{
				t.Z *= -1.0f;
				mz *= -0.5f;
			}

			var bx = t.Y * mx;
			var az = t.X * mz;
			var cx = t.Z * mx;

			var ay = t.X * my;
			var bz = t.Y * mz;
			var cy = t.Z * my;

			var x = bx + az + cx;
			var y = ay + bz + cy;

			return new PointF( x,y );
		}

		public void Inflate( float value )
		{
			X *= value;
			Y *= value;
			Z *= value;
		}

		public void Deflate( float value )
		{
			X /= value;
			Y /= value;
			Z /= value;
		}

        public override bool Equals( object obj )
        {
            var result = false;

            if( obj is Triple )
            {
                var triple = (Triple)obj;

                result =
                (
                    X == triple.X &&
                    Y == triple.Y &&
                    Z == triple.Z
                );
            }

            return result;
        }
    }
}
