using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
	public static class TextureMap
	{
		private static Dictionary<string, string> _paths = new Dictionary<string, string>();
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
		}
	}
}
