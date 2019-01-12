﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
	public static class TextureMap
	{
		private static Dictionary<string, string> _names = new Dictionary<string, string>();
		private static Dictionary<string, string> _paths = new Dictionary<string, string>();
		private static Dictionary<string, AssetPack> _packs = new Dictionary<string, AssetPack>();

		private static string _currentPackName;
		private static string _currentTextureName;

		public static Dictionary<string, AssetPack> Packs => _packs;

		public static string CurrentPackName { get { return _currentPackName; } set { _currentPackName = value; } }
		public static string CurrentTextureName { get { return _currentTextureName; } set { _currentTextureName = value; } }

		public static void LoadPack( string filename )
		{
			if( !_packs.ContainsKey( filename ) )
			{
				var pack = new AssetPack();
				if( pack.Load( filename ) )
				{
					if( pack.Targas.Length > 0 )
					{
						var name = pack.Name;

						_names.Add( filename, name );
						_paths.Add( name, filename );
						_packs.Add( name, pack );

						if( string.IsNullOrEmpty( _currentPackName ) || string.IsNullOrEmpty( _currentTextureName ) )
						{
							_currentPackName = pack.Name;
							_currentTextureName = pack.Names[0];
						}
					}
				}
			}
		}

		public static Targa GetTarga( string packName, string textureName )
		{
			Targa result = null;

			if( !string.IsNullOrEmpty(packName) && !string.IsNullOrEmpty(textureName) &&  _packs.ContainsKey( packName ) )
			{
				var pack = _packs[packName];
				result = pack.GetTarga( textureName );
			}

			return result;
		}

		public static UInt32 GetID( string packName, string textureName )
		{
			UInt32 result = 0;

			if( !string.IsNullOrEmpty(packName) && !string.IsNullOrEmpty(textureName) && _packs.ContainsKey( packName ) )
			{
				var pack = _packs[packName];
				result = pack.GetID( textureName );
			}

			return result;
		}

		public static int GetBPP( string packName, string textureName )
		{
			var result = -1;

			var targa = GetTarga( packName, textureName );
			if( targa != null )
				result = targa.Bpp;

			return result;
		}

		public static AssetPack GetPack( string name )
		{
			if( _packs.ContainsKey( name ) )
				return _packs[name];
			return null;
		}

		public static Targa GetCurrentTarga()
		{
			if( !string.IsNullOrEmpty( _currentPackName ) && !string.IsNullOrEmpty( _currentTextureName ) )
				return _packs[_currentPackName].GetTarga( _currentTextureName );
			return null;
		}
	}
}
