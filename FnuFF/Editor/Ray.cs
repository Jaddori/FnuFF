using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
	public class Ray
	{
		private Triple _start;
		private Triple _direction;
		private float _length;

		public Triple Start => _start;
		public Triple Direction => _direction;
		public float Length => _length;

		public Ray( Triple start, Triple end )
		{
			_start = start;
			_direction = ( end - start );
			_length = _direction.Length();
			_direction.Normalize();
		}

		public Ray( Triple start, Triple direction, float length )
		{
			_start = start;
			_direction = direction;
			_length = length;
		}

		public static Ray FromPoints( Triple start, Triple end )
		{
			return new Ray( start, end );
		}

		public bool Intersect( Plane plane, ref float length )
		{
			float denom = plane.Normal.Dot( _direction );
			if( Math.Abs( denom ) > Extensions.EPSILON )
			{
				var center = plane.Normal * plane.D;
				float t = ( center - _start ).Dot( plane.Normal ) / denom;
				if( t >= -0.0f )
				{
					length = t;
					return true;
				}
			}
			return false;
		}
	}
}
