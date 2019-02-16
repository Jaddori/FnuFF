using System;
using System.Collections.Generic;
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

		public static event ChangeHandler<Solid> OnHoveredSolidChanged;
		public static event ChangeHandler<Solid> OnSelectedSolidChanged;

		public static event ChangeHandler<Face> OnHoveredFaceChanged;
		public static event ChangeHandler<Face> OnSelectedFaceChanged;

		public static event ChangeHandler<Entity> OnSelectedEntityChanged;

		private static EditorTools _current;
		private static Solid _hoveredSolid;
		private static Solid _selectedSolid;
		private static Face _hoveredFace;
		private static Face _selectedFace;
		private static Entity _selectedEntity;

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

		public static Solid HoveredSolid
		{
			get { return _hoveredSolid; }
			set
			{
				if( value != _hoveredSolid )
				{
					var prev = _hoveredSolid;

					if( prev != null )
						prev.Hovered = false;

					_hoveredSolid = value;

					if( _hoveredSolid != null )
						_hoveredSolid.Hovered = true;

					OnHoveredSolidChanged?.Invoke( prev, _hoveredSolid );
				}
			}
		}

		public static Solid SelectedSolid
		{
			get { return _selectedSolid; }
			set
			{
				if( value != _selectedSolid )
				{
					var prev = _selectedSolid;

					if( prev != null )
						prev.Selected = false;

					_selectedSolid = value;

					if( _selectedSolid != null )
						_selectedSolid.Selected = true;

					OnSelectedSolidChanged?.Invoke( prev, _selectedSolid );
				}
			}
		}

		public static Face HoveredFace
		{
			get { return _hoveredFace; }
			set
			{
				if( value != _hoveredFace )
				{
					var prev = _hoveredFace;

					if( prev != null )
						prev.Hovered = false;

					_hoveredFace = value;

					if( _hoveredFace != null )
						_hoveredFace.Hovered = true;

					OnHoveredFaceChanged?.Invoke( prev, _hoveredFace );
				}
			}
		}

		public static Face SelectedFace
		{
			get { return _selectedFace; }
			set
			{
				if( value != _selectedFace )
				{
					var prev = _selectedFace;

					if( prev != null )
						prev.Selected = false;

					_selectedFace = value;

					if( _selectedFace != null )
						_selectedFace.Selected = true;

					OnSelectedFaceChanged?.Invoke( prev, _selectedFace );
				}
			}
		}

		public static Entity SelectedEntity
		{
			get { return _selectedEntity; }
			set
			{
				if( value != _selectedEntity )
				{
					var prev = _selectedEntity;

					if( prev != null )
						prev.Selected = false;

					_selectedEntity = value;

					if( _selectedEntity != null )
						_selectedEntity.Selected = true;

					OnSelectedEntityChanged?.Invoke( prev, _selectedEntity );
				}
			}
		}

		public static void ClearSelection()
		{
			HoveredSolid = null;
			SelectedSolid = null;

			HoveredFace = null;
			SelectedFace = null;

			SelectedEntity = null;
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
