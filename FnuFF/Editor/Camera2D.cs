using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Editor
{
	public class Camera2D
	{
		public const float MIN_ZOOM = 1.0f;
		public const float MAX_ZOOM = 10.0f;

		private PointF _position;
		private float _zoom;
		private int _speed;
		private Triple _direction;

		public PointF Position { get { return _position; } set { _position = value; } }
		public float Zoom
		{
			get { return _zoom; }
			set
			{
				_zoom = value;
				if( _zoom < MIN_ZOOM )
					_zoom = MIN_ZOOM;
				else if( _zoom > MAX_ZOOM )
					_zoom = MAX_ZOOM;
			}
		}
		public int Speed { get { return _speed; } set { _speed = value; } }

		public Triple Direction { get { return _direction; } set { _direction = value; } }

		public Camera2D()
		{
			_zoom = MIN_ZOOM;
			_speed = 4;
		}

		public void Move( float deltaX, float deltaY )
		{
			_position.X += deltaX * _speed;
			_position.Y += deltaY * _speed;
		}

		public PointF ToLocal( PointF point )
		{
			return new PointF
			(
				point.X / _zoom - _position.X,
				point.Y / _zoom - _position.Y
			);
		}

		public PointF ToGlobal( PointF point )
		{
			return new PointF( ( point.X + _position.X ) * _zoom, ( point.Y + _position.Y ) * _zoom );
		}

		public PointF Project( Triple point )
		{
			var bx = _direction.Y * point.X;
			var az = _direction.X * point.Z;
			var cx = _direction.Z * point.X;

			var ay = _direction.X * point.Y;
			var bz = _direction.Y * point.Z;
			var cy = _direction.Z * point.Y;

			var x = az + bx + cx;
			var y = bz - ay - cy;

			return new PointF( x, y );
		}

		public Triple Unproject( PointF point, float depth = 0.0f )
		{
			var bx = _direction.Y * point.X;
			var cx = _direction.Z * point.X;
			var ad = _direction.X * depth;

			var ay = _direction.X * point.Y;
			var cy = _direction.Z * point.Y;
			var bd = _direction.Y * depth;

			var ax = _direction.X * point.X;
			var by = _direction.Y * point.Y;
			var cd = _direction.Z * depth;
			
			var x = ad + bx + cx;
			var y = -ay + bd - cy;
			var z = ax + by + cd;
			
			return new Triple( x, y, z );
		}

		public int Snap( int gapSize, int value )
		{
			var gap = gapSize;// / _zoom;

			//if( value < 0 )
				//value -= gapSize; // TODO: Change to gap

			//return (int)( (int)( value / gap ) * gap );
			return (int)( Math.Round( (float)value / gap ) * gap );
		}

		public float Snap( int gapSize, float value )
		{
			var gap = gapSize;// / _zoom;

			//if( value < 0 )
				//value -= gapSize; // TODO: Change to gap

			return (int)( Math.Round( value / gap ) * gap);
		}

		public PointF Snap( int gapSize, PointF value )
		{
			return new PointF( Snap( gapSize, value.X ), Snap( gapSize, value.Y ) );
		}
    }
}
