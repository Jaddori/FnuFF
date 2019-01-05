using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public static class EditorTool
    {
		public delegate void EditorToolChangedHandler( EditorTools prev, EditorTools current );
		public static event EditorToolChangedHandler OnEditorToolChanged;

		private static EditorTools _current;
		
		public static EditorTools Current
		{
			get { return _current; }
			set
			{
				var prev = _current;
				_current = value;
				OnEditorToolChanged?.Invoke( prev, _current );
			}
		}
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
