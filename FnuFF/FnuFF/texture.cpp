#include "texture.h"
using namespace Rendering;

Texture::Texture()
	: id( 0 ), width( 0 ), height( 0 ), format( GL_COMPRESSED_RGBA_S3TC_DXT1_EXT ), pixels( NULL ), uploaded( false )
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
		int32_t magicNumber;
		fread( &magicNumber, sizeof(magicNumber), 1, file );

		if( magicNumber == DDS_MAGIC_NUMBER )
		{
			DDS_HEADER header;
			fread( &header, sizeof(header), 1, file );

			width = header.width;
			height = header.height;
			size = header.pitchOrLinearSize;

			switch( header.format.fourCC )
			{
				default:
				case ID_DXT1: format = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT; break;
				case ID_DXT3: format = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT; break;
				case ID_DXT5: format = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT; break;
			}

			pixels = new GLbyte[size];
			fread( pixels, sizeof(GLbyte), size, file );

			result = true;
			uploaded = false;
		}

		fclose( file );
	}
	else
		LOG_ERROR( "Failed to open file: %s", path );

	return result;
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
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST );
		//glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR );
		//glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR );
		glCompressedTexImage2D( GL_TEXTURE_2D, 0, format, width, height, 0, size, pixels );

		delete[] pixels;
		pixels = NULL;

		uploaded = true;
	}
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