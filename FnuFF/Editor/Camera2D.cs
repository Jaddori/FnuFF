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

		private Point _position;
		private float _zoom;
		private int _speed;
		private Triple _direction;

		public Point Position { get { return _position; } set { _position = value; } }
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

		public void Move( int deltaX, int deltaY )
		{
			_position.X += deltaX * _speed;
			_position.Y += deltaY * _speed;
		}

		public Point ToLocal( Point point )
		{
			return new Point
			(
				(int)( point.X / _zoom ) - _position.X,
				(int)( point.Y / _zoom ) - _position.Y
			);
		}

		public Point ToGlobal( Point point )
		{
			return new Point( (int)( ( point.X + _position.X ) * _zoom ), (int)( ( point.Y + _position.Y ) * _zoom ) );
		}

		public Point Project( Triple point )
		{
			var bx = _direction.Y * point.X;
			var az = _direction.X * point.Z;
			var cx = _direction.Z * point.X;

			var ay = _direction.X * point.Y;
			var bz = _direction.Y * point.Z;
			var cy = _direction.Z * point.Y;

			var x = bx + az + cx;
			var y = -(ay + bz + cy);

			return new Point( (int)x, (int)y );
		}

		public Triple Unproject( Point point, int depth = 0 )
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

			return new Triple( bx + cx + ad, bd - ay - cy, ax - by + cd );
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

			return (int)( value / gap ) * gap;
		}

		public Point Snap( int gapSize, Point value )
		{
			return new Point( Snap( gapSize, value.X ), Snap( gapSize, value.Y ) );
		}
    }
}
