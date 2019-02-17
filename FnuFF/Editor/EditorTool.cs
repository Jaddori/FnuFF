using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Entities;

namespace Editor
{
	public static class EditorTool
	{
		public static Targa CurrentLightmap;
		public static UInt32 CurrentLightmapID;

		public delegate void ChangeHandler<T>( T prev, T current );

		public static event ChangeHandler<EditorTools> OnEditorToolChanged;

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
		
		public static ObservableCollection<Solid> SelectedSolids { get; } = new ObservableCollection<Solid>();
		public static ObservableCollection<Face> SelectedFaces { get; } = new ObservableCollection<Face>();
		public static ObservableCollection<Entity> SelectedEntities { get; } = new ObservableCollection<Entity>();

		public static void ClearSelection()
		{
			SelectedSolids.Clear();
			SelectedFaces.Clear();
			SelectedEntities.Clear();
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
