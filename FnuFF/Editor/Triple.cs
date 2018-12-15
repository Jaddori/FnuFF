using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Editor
{
	[DebuggerDisplay( "X = {X}, Y = {Y}, Z = {Z}" )]
	public struct Triple
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public Triple( float x, float y, float z )
		{
			X = x;
			Y = y;
			Z = z;
		}

		public float Dot( Triple t )
		{
			return ( X * t.X + Y * t.Y + Z * t.Z );
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
