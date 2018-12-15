using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;

namespace Editor
{
    public class GeometrySolid
    {
		private static Random random = new Random();

        private Triple _min;
        private Triple _max;
		private Color _color;

        public Triple Min { get { return _min; } set { _min = value; } }
        public Triple Max { get { return _max; } set { _max = value; } }

		[XmlIgnore]
		public Color Color { get { return _color; } set { _color = value; } }

		public GeometrySolid()
		{
			_min = new Triple();
			_max = new Triple();

			GenerateColor();
		}

        public GeometrySolid(Triple min, Triple max)
        {
            _min = min;
            _max = max;

			GenerateColor();
        }

		private void GenerateColor()
		{
			var r = random.NextDouble();
			var g = random.NextDouble();
			var b = random.NextDouble();

			var magnitude = Math.Sqrt( r * r + g * g + b * b );

			r /= magnitude;
			g /= magnitude;
			b /= magnitude;

			_color = Color.FromArgb( (int)( r * 255 ), (int)( g * 255 ), (int)( b * 255 ) );
		}
    }
}
