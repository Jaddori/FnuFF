using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Editor
{
	public class AssetPack
	{
		private const int NAME_LENGTH = 64;

		private string _name;
		private string[] _names;
		private Targa[] _targas;
		private UInt32[] _ids;

		public string Name => _name;
		public string[] Names => _names;
		public Targa[] Targas => _targas;
		public UInt32[] IDs => _ids;

		public AssetPack()
		{
		}

		public bool Load( string filename )
		{
			var result = true;

			var slashIndex = filename.LastIndexOf( '\\' );
			if( slashIndex < 0 )
				slashIndex = filename.LastIndexOf( '/' );
			if( slashIndex < 0 )
				slashIndex = 0;
			var dotIndex = filename.LastIndexOf( '.' );
			_name = filename.Substring( slashIndex + 1, dotIndex - slashIndex - 1 );

			var stream = new FileStream( filename, FileMode.Open, FileAccess.Read );
			var reader = new BinaryReader( stream );

			var files = reader.ReadUInt32();

			// read names
			_names = new string[files];
			byte[] nameBuffer = new byte[NAME_LENGTH];
			for( int i = 0; i < files; i++ )
			{
				reader.Read( nameBuffer, 0, NAME_LENGTH );

				int nameLength = Array.IndexOf( nameBuffer, (byte)0 );
				_names[i] = Encoding.Default.GetString( nameBuffer, 0, nameLength );
			}

			// read textures
			_targas = new Targa[files];
			_ids = new UInt32[files];
			for( int i = 0; i < files && result; i++ )
			{
				var targa = new Targa();
				if( targa.Read( reader ) )
				{
					_ids[i] = GL.UploadTexture( targa.Width, targa.Height, targa.Bpp, targa.Pixels );
					_targas[i] = targa;
				}
				else
					result = false;
			}

			reader.Close();
			stream.Close();

			return result;
		}

		public Targa GetTarga( string name )
		{
			Targa result = null;

			//var index = _name.IndexOf( name );
			var index = Array.IndexOf( _names, name );
			if( index >= 0 )
			{
				result = _targas[index];
			}

			return result;
		}

		public Targa GetTarga( int index )
		{
			Targa result = null;

			if( index >= 0 && index < _targas.Length )
				result = _targas[index];

			return result;
		}

		public UInt32 GetID( string name )
		{
			UInt32 result = 0;

			//var index = _names.IndexOf( name );
			var index = Array.IndexOf( _names, name );
			if( index >= 0 )
			{
				result = _ids[index];
			}

			return result;
		}

		public UInt32 GetID( int index )
		{
			UInt32 result = 0;

			if( index >= 0 && index < _targas.Length )
				result = _ids[index];

			return result;
		}

		public int GetTextureIndex( string name )
		{
			var result = -1;

			for( int i = 0; i < _names.Length && result < 0; i++ )
				if( _names[i] == name )
					result = i;

			return result;
		}
	}
}
