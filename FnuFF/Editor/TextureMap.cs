using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
	public static class TextureMap
	{
		/*private static Dictionary<string, string> _paths = new Dictionary<string, string>();
		private static Dictionary<string, Targa> _targas = new Dictionary<string, Targa>();
		private static Dictionary<string, UInt32> _ids = new Dictionary<string, uint>();
		private static string _current;

		public static string GetCurrent()
		{
			return _current;
		}

		public static void SetCurrent( string name )
		{
			if( _targas.ContainsKey( name ) )
				_current = name;
			else
				_current = string.Empty;
		}

		public static string GetName( string path )
		{
			if( _paths.ContainsKey( path ) )
				return _paths[path];
			return string.Empty;
		}

		public static string GetPath( string name )
		{
			return _paths.FirstOrDefault( x => x.Value == name ).Value;
		}

		public static bool HasTexture( string pathOrName )
		{
			return _paths.ContainsKey( pathOrName ) || _paths.ContainsValue( pathOrName );
		}

		public static Targa GetTarga( string name )
		{
			if( _targas.ContainsKey( name ) )
				return _targas[name];
			return null;
		}

		public static UInt32 GetID( string name )
		{
			if( _ids.ContainsKey( name ) )
				return _ids[name];
			return 0;
		}

		public static string LoadTexture( string filename )
		{
			var name = string.Empty;

			if( _paths.ContainsKey( filename ) )
				name = _paths[filename];
			else
			{
				var targa = new Targa();
				if(targa.Load( filename ) )
				{
					name = filename.Substring( filename.LastIndexOf( '\\' ) + 1 );
					_paths.Add( filename, name );
					_targas.Add( name, targa );

					var id = GL.LoadTexture( filename );
					_ids.Add( name, id );

					if( string.IsNullOrEmpty( _current ) )
						_current = name;
				}
			}

			return name;
		}*/

		private static Dictionary<string, string> _names = new Dictionary<string, string>();
		private static Dictionary<string, string> _paths = new Dictionary<string, string>();
		private static Dictionary<string, ContentPack> _packs = new Dictionary<string, ContentPack>();

		private static string _currentPackName;
		private static string _currentTextureName;

		public static Dictionary<string, ContentPack> Packs => _packs;

		public static string CurrentPackName { get { return _currentPackName; } set { _currentPackName = value; } }
		public static string CurrentTextureName { get { return _currentTextureName; } set { _currentTextureName = value; } }

		public static void LoadPack( string filename )
		{
			if( !_packs.ContainsKey( filename ) )
			{
				var pack = new ContentPack();
				if( pack.Load( filename ) )
				{
					var name = pack.Name;

					_names.Add( filename, name );
					_paths.Add( name, filename );
					_packs.Add( name, pack );
				}
			}
		}

		public static Targa GetTarga( string packName, string textureName )
		{
			Targa result = null;

			if( _packs.ContainsKey( packName ) )
			{
				var pack = _packs[packName];
				result = pack.GetTarga( textureName );
			}

			return result;
		}

		public static UInt32 GetID( string packName, string textureName )
		{
			UInt32 result = 0;

			if( _packs.ContainsKey( packName ) )
			{
				var pack = _packs[packName];
				result = pack.GetID( textureName );
			}

			return result;
		}

		public static ContentPack GetPack( string name )
		{
			if( _packs.ContainsKey( name ) )
				return _packs[name];
			return null;
		}
	}
}
