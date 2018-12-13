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
			var xproj = ( point.Z / ( point.X == 0 ? 1.0f : point.X ) ) * _direction.X + ( 1 - _direction.X );
			var yproj = ( point.Z / ( point.Y == 0 ? 1.0f : point.Y ) ) * _direction.Y + ( 1 - _direction.Y );

			return new Point( (int)( xproj * point.X ), (int)( yproj * point.Y ) );
		}

		public Triple Unproject( Point point, int depth = 0 )
		{
			var xproj = point.X - _direction.X * point.X + _direction.X * depth;
			var yproj = point.Y - _direction.Y * point.Y + _direction.Y * depth;
			var zproj = _direction.X * point.X + _direction.Y * point.Y + _direction.Z * depth;

			return new Triple( xproj, yproj, zproj );
		}

		public int Snap( int gapSize, int value )
		{
			var gap = gapSize;// / _zoom;

			if( value < 0 )
				value -= gapSize; // TODO: Change to gap

			return (int)( (int)( value / gap ) * gap );
		}

		public float Snap( int gapSize, float value )
		{
			var gap = gapSize;// / _zoom;

			if( value < 0 )
				value -= gapSize; // TODO: Change to gap

			return (int)( value / gap ) * gap;
		}

		public Point Snap( int gapSize, Point value )
		{
			return new Point( Snap( gapSize, value.X ), Snap( gapSize, value.Y ) );
		}
    }
}
