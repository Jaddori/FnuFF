using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Editor
{
	public class Targa
	{
		private int _width;
		private int _height;
		private int _bpp;
		private byte[] _pixels;

		public int Width => _width;
		public int Height => _height;
		public int Bpp => _bpp;
		public byte[] Pixels => _pixels;

		public Targa()
		{
			_width = _height = _bpp = 0;
		}

		public bool Load( string filename )
		{
			var result = false;

			if( !File.Exists( filename ) )
				throw new FileNotFoundException( "File not found: " + filename );

			using( var stream = new MemoryStream( File.ReadAllBytes( filename ) ) )
			{
				result = Read( stream );
			}

			return result;
		}

		public bool Read( MemoryStream stream )
		{
			var result = false;

			stream.ReadByte(); // skip id length
			stream.ReadByte(); // skip colormap type

			var imageType = stream.ReadByte();
			if( imageType == 2 ) // can only handle unmapped RGB(A)
			{
				byte[] buffer = new byte[16];
				stream.Read( buffer, 0, 9 ); // skip colormap and origin info

				stream.Read( buffer, 0, 5 );
				_width = BitConverter.ToInt16( buffer, 0 );
				_height = BitConverter.ToInt16( buffer, 2 );
				_bpp = buffer[4];
				_bpp /= 8;

				if( _width > 0 && _height > 0 && ( _bpp == 3 || _bpp == 4 ) )
				{
					int pixelCount = _width * _height * _bpp;
					_pixels = new byte[pixelCount];
					stream.Read( _pixels, 0, pixelCount );

					// convert from GBR(A) to RGB(A)
					for( int i = 0; i < pixelCount; i += _bpp )
					{
						var temp = _pixels[i];
						_pixels[i] = _pixels[i + 2];
						_pixels[i + 2] = temp;

						temp = _pixels[i+1];
						_pixels[i + 1] = _pixels[i];
						_pixels[i] = temp;
					}

					result = true;
				}
			}

			return result;
		}

		public Image ToImage()
		{
			if( _width <= 0 || _height <= 0 || _bpp <= 0 )
				throw new InvalidOperationException( "Targa file has not been loaded." );

			var format = ( _bpp == 3 ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb );
			var result = new Bitmap( _width, _height, format );
			var bounds = new Rectangle( 0, 0, _width, _height );

			var data = result.LockBits( bounds, ImageLockMode.WriteOnly, format );
			
			var lineWidth = _width * _bpp;
			var ptr = data.Scan0;
			for( int y = 0; y < _height; y++ )
			{
				Marshal.Copy( _pixels, (_height-y-1) * lineWidth, ptr, lineWidth );
				ptr += data.Stride;
			}

			result.UnlockBits( data );

			return result;
		}
	}
}
