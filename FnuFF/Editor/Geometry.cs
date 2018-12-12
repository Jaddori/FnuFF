using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public static class Geometry
    {
        public delegate void ChangeHandler();
        public static event ChangeHandler OnSolidChange;

        private static List<GeometrySolid> _solids = new List<GeometrySolid>();

        public static List<GeometrySolid> Solids { get { return _solids; } }

        public static void AddSolid( GeometrySolid solid )
        {
            _solids.Add( solid );
            OnSolidChange?.Invoke();
        }
    }
}
