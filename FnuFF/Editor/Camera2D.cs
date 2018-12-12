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
            return new Point( point.X - _position.X, point.Y - _position.Y );
        }

        public Point ToGlobal( Point point )
        {
            return new Point( (int)((point.X + _position.X) / _zoom), (int)((point.Y + _position.Y) / _zoom) );
        }
    }
}
