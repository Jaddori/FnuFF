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

		private Triple _normal;
		private float _d;

		public Triple Normal { get { return _normal; } set { _normal = value; } }
		public float D { get { return _d; } set { _d = value; } }

		public Plane()
		{
			_normal = new Triple();
			_d = 0.0f;
		}

		public Plane(Triple normal, float d)
		{
			_normal = normal;
			_d = d;
		}

		public override bool Equals( object obj )
		{
			var result = false;

			if( obj is Plane )
			{
				var plane = (Plane)obj;
				result =
				(
					Math.Abs(_normal.X - plane._normal.X) < EPSILON &&
					Math.Abs(_normal.Y - plane._normal.Y) < EPSILON &&
					Math.Abs(_normal.Z - plane._normal.Z) < EPSILON &&
					Math.Abs(_d - plane._d ) < EPSILON
				);

				var derp = Math.Abs( _d - plane._d );
			}

			return result;
		}

		public bool InFront( Triple point )
		{
			var distanceAlongNormal = point.Dot( _normal );
			//var dif = Math.Abs( distanceAlongNormal - _d );
			//return ( dif > -EPSILON );

			//var greater = (distanceAlongNormal + EPSILON > _d);
			var greater = ( distanceAlongNormal > _d );
			return greater;
		}

		public bool OnPlane( Triple point )
		{
			var distanceAlongNormal = point.Dot( _normal );
			return ( Math.Abs( distanceAlongNormal - _d ) < EPSILON );
		}
	}
}
