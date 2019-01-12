#include "texture.h"
using namespace Rendering;

Texture::Texture()
	: id( 0 ), width( 0 ), height( 0 ), format( GL_RGB ), pixels( NULL ), uploaded( false )
{
}

Texture::~Texture()
{
}

bool Texture::load( const char* path )
{
	bool result = false;

	FILE* file = fopen( path, "rb" );
	if( file )
	{
		read( file );

		fclose( file );

		result = true;
		uploaded = false;
	}

	return result;
}

bool Texture::read( FILE* file )
{
	TargaHeader header;
	fread( &header, sizeof(header), 1, file );

	width = header.width;
	height = header.height;

	int bpp = header.bpp / 8;
	size = width * height * bpp;

	format = GL_BGR;
	if( bpp == 4 )
		format = GL_BGRA;

	pixels = new GLbyte[size];

	if( header.imageType == 2 ) // uncompressed RGB(A)
	{
		fread( pixels, sizeof(GLbyte), size, file );
	}
	else // RLE compressed RGB(A)
	{
		char* pixelBuffer[4] = {};

		int offset = 0;
		while( offset < size )
		{
			char packet = 0;
			fread( &packet, sizeof(packet), 1, file );

			if( (packet & 0x80) ) // RLE
			{
				packet ^= 0x80;
				int count = packet + 1;
				fread( pixelBuffer, 1, bpp, file );

				for( int i=0; i<count; i++ )
				{
					memcpy( pixels + offset, pixelBuffer, bpp );
					offset += bpp;
				}
			}
			else // raw pixels
			{
				int count = packet + 1;
				fread( pixels + offset, 1, count*bpp, file );

				offset += count*bpp;
			}
		}
	}

	// need to flip the image data vertically
	if( header.yorigin == 0 )
	{
		int lineSize = width*bpp;
		GLbyte* buffer = new GLbyte[lineSize];

		for( int y = 0; y<height / 2; y++ )
		{
			GLbyte* top = pixels + y * lineSize;
			GLbyte* bottom = pixels + ( height - y - 1 ) * lineSize;

			memcpy( buffer, top, lineSize );
			memcpy( top, bottom, lineSize );
			memcpy( bottom, buffer, lineSize );
		}

		delete[] buffer;
	}

	uploaded = false;

	return true;
}

void Texture::unload()
{
	if( id )
		glDeleteTextures( 1, &id );

	id = 0;
	width = height = 0;

	if( pixels )
	{
		delete[] pixels;
		pixels = NULL;
	}
}

void Texture::upload()
{
	if( !uploaded )
	{
		if( id == 0 )
			glGenTextures( 1, &id );

		glBindTexture( GL_TEXTURE_2D, id );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST );
		glTexImage2D( GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, format, GL_UNSIGNED_BYTE, pixels );

		delete[] pixels;
		pixels = NULL;

		uploaded = true;
	}
}

bool Texture::hasAlpha() const
{
	return ( format == TEXTURE_TRANSPARENT_FORMAT );
}

GLuint Texture::getID() const
{
	return id;
}

int Texture::getWidth() const
{
	return width;
}

int Texture::getHeight() const
{
	return height;
}

GLenum Texture::getFormat() const
{
	return format;
}