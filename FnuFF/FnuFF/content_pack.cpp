#include "content_pack.h"
using namespace Rendering;

ContentPack::ContentPack()
	: textureCount( 0 ), textures( NULL ), uploaded( false )
{
}

ContentPack::~ContentPack()
{
}

bool ContentPack::load( const char* path )
{
	bool result = false;

	FILE* file = fopen( path, "rb" );
	if( file )
	{
		result = true;

		// read count
		fread( &textureCount, sizeof(textureCount), 1, file );

		// read names
		names = new name_t[textureCount];
		for( int i=0; i<textureCount; i++ )
		{
			fread( names[i], 1, TEXTURE_NAME_LEN, file );
		}

		// read textures
		textures = new Texture[textureCount];
		for( int i=0; i<textureCount && result; i++ )
		{
			if( !textures[i].read( file ) )
				result = false;
		}

		fclose( file );
	}

	return result;
}

void ContentPack::unload()
{
	for( int i=0; i<textureCount; i++ )
	{
		textures[i].unload();
	}

	textureCount = 0;
	delete[] textures;
	uploaded = false;

	name[0] = 0;
}

void ContentPack::upload()
{
	if( !uploaded )
	{
		for( int i=0; i<textureCount; i++ )
		{
			textures[i].upload();
		}

		uploaded = true;
	}
}

int ContentPack::getIndex( const char* name )
{
	int result = -1;
	
	for( int i=0; i<textureCount && result < 0; i++ )
		if( strcmp( names[i], name ) == 0 )
			result = i;

	return result;
}

const Texture* ContentPack::getTexture( int index ) const
{
	assert( index >= 0 && index < textureCount );

	return &textures[index];
}