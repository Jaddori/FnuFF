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
		TargaHeader header;
		fread( &header, sizeof(header), 1, file );

		width = header.width;
		height = header.height;

		int bpp = header.bpp / 8;
		size = width * height * bpp;

		if( bpp == 3 )
			format = GL_BGR;
		else if( bpp == 4 )
			format = GL_BGRA;

		pixels = new GLbyte[size];
		fread( pixels, sizeof(GLbyte), size, file );

		fclose( file );

		result = true;
		uploaded = false;
	}

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
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST );
		glTexImage2D( GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, format, GL_UNSIGNED_BYTE, pixels );

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