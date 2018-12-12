using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class GeometrySolid
    {
        private Triple _min;
        private Triple _max;

        public Triple Min { get { return _min; } set { _min = value; } }
        public Triple Max { get { return _max; } set { _max = value; } }

        public GeometrySolid()
        {
            _min = new Triple();
            _max = new Triple();
        }

        public GeometrySolid(Triple min, Triple max)
        {
            _min = min;
            _max = max;
        }
    }
}
