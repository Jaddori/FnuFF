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
	public class Targa : IDisposable
	{
		private int _width;
		private int _height;
		private int _bpp;
		private byte[] _pixels;
		private Image _image;
		private bool _imageDirty;

		public int Width { get { return _width; } set { _width = value; _imageDirty = true; } }
		public int Height { get { return _height; } set { _height = value; _imageDirty = true; } }
		public int Bpp { get { return _bpp; } set { _bpp = value; _imageDirty = true; } }
		public byte[] Pixels { get { return _pixels; } set { _pixels = value; _imageDirty = true; } }

		public Targa()
		{
			_width = _height = _bpp = 0;
			_imageDirty = true;
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

			byte[] buffer = new byte[16];
			stream.Read( buffer, 0, 9 ); // skip colormap and origin info

			stream.Read( buffer, 0, 5 );
			_width = BitConverter.ToInt16( buffer, 0 );
			_height = BitConverter.ToInt16( buffer, 2 );
			_bpp = buffer[4];
			_bpp /= 8;

			stream.ReadByte(); // skip image descriptor

			if( _width > 0 && _height > 0 && ( _bpp == 3 || _bpp == 4 ) )
			{
				int pixelCount = _width * _height * _bpp;
				_pixels = new byte[pixelCount];

				if( imageType == 2 ) // unmapped RGB(A)
				{
					stream.Read( _pixels, 0, pixelCount );

					_imageDirty = true;
					result = true;
				}
				else if( imageType == 10 ) // RLE RGB(A)
				{
					var pixelOffset = 0;
					while( pixelOffset < pixelCount )
					{
						var packet = new byte[1];
						stream.Read( packet, 0, 1 );

						if( ( packet[0] & 0x80 ) > 0 ) // RLE header
						{
							int count = ( packet[0] ^ 0x80 ) + 1;

							var pixel = new byte[_bpp];
							stream.Read( pixel, 0, _bpp );

							for( int i = 0; i < count; i++ )
							{
								Array.Copy( pixel, 0, _pixels, pixelOffset, _bpp );
								pixelOffset += _bpp;
							}
						}
						else // raw pixel
						{
							int count = packet[0] + 1;

							stream.Read( _pixels, pixelOffset, count * _bpp );
							pixelOffset += count * _bpp;
						}
					}

					_imageDirty = true;
					result = true;
				}
			}

			return result;
		}

		public bool Read( BinaryReader reader )
		{
			var result = false;

			reader.ReadByte(); // skip id length
			reader.ReadByte(); // skip colormap type

			var imageType = reader.ReadByte();

			byte[] buffer = new byte[16];
			reader.Read( buffer, 0, 9 ); // skip colormap and origin info

			reader.Read( buffer, 0, 5 );
			_width = BitConverter.ToInt16( buffer, 0 );
			_height = BitConverter.ToInt16( buffer, 2 );
			_bpp = buffer[4];
			_bpp /= 8;

			reader.ReadByte(); // skip image descriptor

			if( _width > 0 && _height > 0 && ( _bpp == 3 || _bpp == 4 ) )
			{
				int pixelCount = _width * _height * _bpp;
				_pixels = new byte[pixelCount];

				if( imageType == 2 ) // unmapped RGB(A)
				{
					reader.Read( _pixels, 0, pixelCount );

					_imageDirty = true;
					result = true;
				}
				else if( imageType == 10 ) // RLE RGB(A)
				{
					var pixelOffset = 0;
					while( pixelOffset < pixelCount )
					{
						var packet = new byte[1];
						reader.Read( packet, 0, 1 );

						if( ( packet[0] & 0x80 ) > 0 ) // RLE header
						{
							int count = (packet[0] ^ 0x80) + 1;

							var pixel = new byte[_bpp];
							reader.Read( pixel, 0, _bpp );

							for( int i = 0; i < count; i++ )
							{
								Array.Copy( pixel, 0, _pixels, pixelOffset, _bpp );
								pixelOffset += _bpp;
							}
						}
						else // raw pixel
						{
							int count = packet[0] + 1;

							reader.Read( _pixels, pixelOffset, count * _bpp );
							pixelOffset += count * _bpp;
						}
					}

					_imageDirty = true;
					result = true;
				}
			}

			return result;
		}

		public void Write( string filename )
		{
			var stream = new FileStream( filename, FileMode.Create, FileAccess.Write );
			
			Write( stream );

			stream.Close();
		}

		public void Write( Stream stream )
		{
			if( _width <= 0 )
				throw new InvalidOperationException( "Width is not set." );
			if( _height <= 0 )
				throw new InvalidOperationException( "Height is not set." );
			if( _bpp <= 0 )
				throw new InvalidOperationException( "Bpp is not set." );
			if( _pixels == null )
				throw new InvalidOperationException( "Pixel data is not set." );

			var writer = new BinaryWriter( stream );

			byte imageType = 2;
			Int16 width = (Int16)_width;
			Int16 height = (Int16)_height;
			byte bpp = (byte)(_bpp*8);

			var zeroes = new byte[16];
			writer.Write( zeroes, 0, 2 ); // id, colormap type
			writer.Write( imageType ); // image type (unmapped RGBA)
			writer.Write( zeroes, 0, 9 ); // origin, length, depth, x/y-origin
			writer.Write( width );
			writer.Write( height );
			writer.Write( bpp );
			writer.Write( zeroes, 0, 1 ); // image descriptor

			var pixelCount = _width * _height * _bpp;
			writer.Write( _pixels, 0, pixelCount );

			writer.Close();
		}

		public Image ToImage()
		{
			if( _imageDirty )
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
					Marshal.Copy( _pixels, y * lineWidth, ptr, lineWidth );
					ptr += data.Stride;
				}

				result.UnlockBits( data );

				_image = result;
				_imageDirty = false;
			}

			return _image;
		}

		public void Dispose()
		{
			if( _image != null )
			{
				_image.Dispose();
				_image = null;
			}
		}
	}
}
