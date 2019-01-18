using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Editor
{
	[DebuggerDisplay( "{Normal.X}, {Normal.Y}, {Normal.Z}, {D}" )]
	public class Plane
	{
		private const float EPSILON = 0.001f;
		private const float EPSILON_MINOR = 0.00001f;

		//private Triple _normal;
		//private float _d;
		//
		//public Triple Normal { get { return _normal; } set { _normal = value; } }
		//public float D { get { return _d; } set { _d = value; } }

		public Triple Normal;
		public float D;

		public Plane()
		{
			Normal = new Triple();
			D = 0.0f;
		}

		public Plane(Triple normal, float d)
		{
			Normal = normal;
			Normal.Normalize();
			D = d;
		}

		public Plane( Triple normal, Triple pointOnPlane )
		{
			Normal = normal;
			Normal.Normalize();

			D = pointOnPlane.Dot( Normal );
		}

		public override bool Equals( object obj )
		{
			var result = false;

			if( obj is Plane )
			{
				var plane = (Plane)obj;
				result =
				(
					Math.Abs( Normal.X - plane.Normal.X) < EPSILON &&
					Math.Abs( Normal.Y - plane.Normal.Y) < EPSILON &&
					Math.Abs( Normal.Z - plane.Normal.Z) < EPSILON &&
					Math.Abs(D - plane.D ) < EPSILON
				);

				var derp = Math.Abs( D - plane.D );
			}

			return result;
		}

		public bool InFront( Triple point, float margin = 0.0f )
		{
			var distanceAlongNormal = point.Dot( Normal );
			//var dif = Math.Abs( distanceAlongNormal - _d );
			//return ( dif > -EPSILON );

			var greater = (distanceAlongNormal - EPSILON_MINOR + margin > D );
			//var greater = ( distanceAlongNormal > _d );
			return greater;
		}

		public bool OnPlane( Triple point )
		{
			var distanceAlongNormal = point.Dot( Normal );
			return ( Math.Abs( distanceAlongNormal - D ) < EPSILON );
		}

		public float Distance( Triple point )
		{
			var distanceAlongNormal = point.Dot( Normal );
			return ( D - distanceAlongNormal );
		}
	}
}
