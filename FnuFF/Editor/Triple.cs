using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public struct Triple
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Triple( float x, float y, float z )
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
