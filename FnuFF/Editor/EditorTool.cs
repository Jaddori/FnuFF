using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public static class EditorTool
    {
        public static EditorTools Current { get; set; } = EditorTools.Select;
    }

    public enum EditorTools
    {
        None,
        Select,
        Solid,
		Clip,
		Vertex,
		Texture,
		Face,
		Entity
    }
}
