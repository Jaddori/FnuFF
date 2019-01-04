using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Editor
{
	public class Camera3D
	{
		private float _speed;

		private Triple _position;
		private Triple _direction;
		private Triple _right;
		private Triple _up;

		private float _horizontalAngle;
		private float _verticalAngle;

		private float _horizontalSensitivity;
		private float _verticalSensitivity;

		public float Speed
		{
			get { return _speed; }
			set { _speed = value; }
		}

		public Triple Position
		{
			get { return _position; }
			set { _position = value; }
		}

		public Triple Direction
		{
			get { return _direction; }
			set
			{
				_direction = value;
				_direction.Normalize();

				_verticalAngle = (float)Math.Asin( _direction.Y );

				float acva = (float)Math.Acos( _verticalAngle );

				if( Math.Abs( acva ) < Extensions.EPSILON )
					_horizontalAngle = (float)Math.Acos( _direction.Z / Math.Cos( _verticalAngle ) );
				else
					_horizontalAngle = (float)Math.Acos( 0.0f );

				// calculate right vector
				_right.X = (float)Math.Sin( _horizontalAngle - Math.PI * 0.5f );
				_right.Y = 0.0f;
				_right.Z = (float)Math.Cos( _horizontalAngle - Math.PI * 0.5f );

				_up = _right.Cross( _direction );
			}
		}

		public Triple Forward
		{
			get { return _direction; }
			set { _direction = value; }
		}

		public Triple Right
		{
			get { return _right; }
			set { _right = value; }
		}

		public Triple Up
		{
			get { return _up; }
			set { _up = value; }
		}

		public float HorizontalAngle
		{
			get { return _horizontalAngle; }
			set
			{
				_horizontalAngle = value;

				// clamp angles
				if( _horizontalAngle > Math.PI * 2.0f )
					_horizontalAngle -= (float)Math.PI * 2.0f;
				else if( _horizontalAngle < Math.PI * -2.0f )
					_horizontalAngle += (float)Math.PI * 2.0f;

				// calculate new direction
				_direction.X = (float)( Math.Cos( _verticalAngle ) * Math.Sin( _horizontalAngle ) );
				_direction.Y = (float)Math.Sin( _verticalAngle );
				_direction.Z = (float)( Math.Cos( _verticalAngle ) * Math.Cos( _horizontalAngle ) );

				// calculate right vector
				_right.X = (float)Math.Sin( _horizontalAngle - Math.PI * 0.5f );
				_right.Y = 0.0f;
				_right.Z = (float)Math.Cos( _horizontalAngle - Math.PI * 0.5f );

				_up = _right.Cross( _direction );
			}
		}

		public float VerticalAngle
		{
			get { return _verticalAngle; }
			set
			{
				_verticalAngle = value;

				// clamp angles
				if( Math.Abs( _verticalAngle ) > Math.PI * 0.5f )
					_verticalAngle = (float)Math.PI * 0.5f;

				// calculate new direction
				_direction.X = (float)( Math.Cos( _verticalAngle ) * Math.Sin( _horizontalAngle ) );
				_direction.Y = (float)Math.Sin( _verticalAngle );
				_direction.Z = (float)( Math.Cos( _verticalAngle ) * Math.Cos( _horizontalAngle ) );

				// calculate right vector
				_right.X = (float)Math.Sin( _horizontalAngle - Math.PI * 0.5f );
				_right.Y = 0.0f;
				_right.Z = (float)Math.Cos( _horizontalAngle - Math.PI * 0.5f );

				_up = _right.Cross( _direction );
			}
		}

		public float HorizontalSensitivity
		{
			get { return _horizontalSensitivity; }
			set { _horizontalSensitivity = value; }
		}

		public float VerticalSensitivity
		{
			get { return _verticalSensitivity; }
			set { _verticalSensitivity = value; }
		}

		public Camera3D()
		{
			_speed = 0.1f;

			_position = new Triple();
			_direction = new Triple(0,0,1);
			_right = new Triple(1,0,0);
			_up = new Triple(0,1,0);

			_horizontalAngle = 0.0f;
			_verticalAngle = 0.0f;

			_horizontalSensitivity = 1.0f;
			_verticalSensitivity = 1.0f;
		}

		public Point Project( Triple point )
		{
			return new Point();
		}

		public Triple Unproject( Point point, float depth )
		{
			return new Triple();
		}

		public void RelativeMovement( Triple localMovement )
		{
			// move backwards and forwards
			if( Math.Abs( localMovement.Z ) > Extensions.EPSILON )
			{
				Triple forward = _direction;
				forward.Normalize();

				_position += forward * localMovement.Z * _speed;
			}

			// move left and right
			if( Math.Abs( localMovement.X ) > Extensions.EPSILON )
			{
				Triple right = new Triple
				(
					(float)Math.Sin(_horizontalAngle - Math.PI*0.5f),
					0.0f,
					(float)Math.Cos(_horizontalAngle - Math.PI*0.5f)
				);

				_position += right * localMovement.X * _speed;
			}

			// move up and down
			if( Math.Abs( localMovement.Y ) > Extensions.EPSILON )
			{
				Triple up = new Triple( 0, 1, 0 );
				_position += up * localMovement.Y * _speed;
			}
		}

		public void AbsoluteMovement( Triple globalMovement )
		{
			_position += globalMovement;
		}

		public void UpdateDirection( int deltaX, int deltaY )
		{
			_horizontalAngle += deltaX * _horizontalSensitivity;
			_verticalAngle += deltaY * _verticalSensitivity;

			// clamp angles
			if( _horizontalAngle > Math.PI * 2.0f )
				_horizontalAngle -= (float)Math.PI * 2.0f;
			else if( _horizontalAngle < Math.PI * -2.0f )
				_horizontalAngle += (float)Math.PI * 2.0f;

			if( Math.Abs( _verticalAngle ) > Math.PI * 0.5f )
				_verticalAngle = (float)Math.PI * 0.5f;

			// calculate new direction
			_direction.X = (float)( Math.Cos( _verticalAngle ) * Math.Sin( _horizontalAngle ) );
			_direction.Y = (float)( Math.Sin( _verticalAngle ) );
			_direction.Z = (float)( Math.Cos( _verticalAngle ) * Math.Cos( _horizontalAngle ) );

			// calculate up vector
			_right.X = (float)Math.Sin( _horizontalAngle - Math.PI * 0.5f );
			_right.Y = 0.0f;
			_right.Z = (float)Math.Cos( _horizontalAngle - Math.PI * 0.5f );

			_up = _right.Cross( _direction );
		}
	}
}
