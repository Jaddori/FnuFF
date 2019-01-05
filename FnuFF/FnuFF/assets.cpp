#include "assets.h"
using namespace Rendering;

/*Assets::Assets()
	: dirtyTextures( false ), dirtyMeshes( false )
{
#if _DEBUG
	lastHotload = SDL_GetTicks();
#endif
}

Assets::~Assets()
{
}

void Assets::unload()
{
	for( int i=0; i<textures.getSize(); i++ )
		textures[i].unload();

	for( int i=0; i<meshes.getSize(); i++ )
		meshes[i].unload();

	textureHashes.clear();
	textures.clear();

	meshHashes.clear();
	meshes.clear();
}

void Assets::upload()
{
	if( dirtyTextures )
	{
		LOG_INFO( "Uploading textures." );

		for( int i=0; i<textures.getSize(); i++ )
			textures[i].upload();

		dirtyTextures = false;
	}

	if( dirtyMeshes )
	{
		LOG_INFO( "Uploading meshes." );

		for( int i=0; i<meshes.getSize(); i++ )
			meshes[i].upload();

		dirtyMeshes = false;
	}

	if( dirtyFonts )
	{
		LOG_INFO( "Uploading fonts." );

		for( int i=0; i<fonts.getSize(); i++ )
			fonts[i].upload();

		dirtyFonts = false;
	}
}

#if _DEBUG
void Assets::hotload()
{
	uint64_t curTick = SDL_GetTicks();
	if( curTick - lastHotload > ASSETS_HOTLOAD_DELAY )
	{
		lastHotload = curTick;

		// hotload textures
		const int TEXTURE_COUNT = texturePaths.getSize();
		for( int i=0; i<TEXTURE_COUNT; i++ )
		{
			uint64_t curTimestamp = getTimestamp( texturePaths[i] );
			if( curTimestamp != textureTimestamps[i] )
			{
				reloadTexture( i );
				textureTimestamps[i] = curTimestamp;
			}
		}

		// hotload meshes
		const int MESH_COUNT = meshPaths.getSize();
		for( int i=0; i<MESH_COUNT; i++ )
		{
			uint64_t curTimestamp = getTimestamp( meshPaths[i] );
			if( curTimestamp != meshTimestamps[i] )
			{
				reloadMesh( i );
				meshTimestamps[i] = curTimestamp;
			}
		}
	}
}
#endif

int Assets::loadTexture( const char* path )
{
	uint64_t hash = hashPath( path );
	int result = textureHashes.find( hash );
	if( result < 0 )
	{
		Texture texture;
		if( texture.load( path ) )
		{
			result = textures.getSize();
			textures.add( texture );
			textureHashes.add( hash );

			dirtyTextures = true;

#if _DEBUG
			char* texturePath = new char[ASSETS_PATH_MAX_LEN];
			strcpy( texturePath, path );
			texturePath[strlen( path )] = 0;
			texturePaths.add( texturePath );

			uint64_t timestamp = getTimestamp( path );
			textureTimestamps.add( timestamp );
#endif
		}
	}

	return result;
}

int Assets::loadMesh( const char* path )
{
	uint64_t hash = hashPath( path );
	int result = meshHashes.find( hash );
	if( result < 0 )
	{
		Mesh mesh;
		if( mesh.load( path ) )
		{
			result = meshes.getSize();
			meshes.add( mesh );
			meshHashes.add( hash );

			dirtyMeshes = true;

#if _DEBUG
			char* meshPath = new char[ASSETS_PATH_MAX_LEN];
			strcpy( meshPath, path );
			meshPath[strlen( path )] = 0;
			meshPaths.add( meshPath );

			uint64_t timestamp = getTimestamp( path );
			meshTimestamps.add( timestamp );
#endif
		}
	}

	return result;
}

int Assets::loadFont( const char* info, const char* texture )
{
	uint64_t hash = hashPath( info );
	int result = fontHashes.find( hash );
	if( result < 0 )
	{
		Font font;
		if( font.load( info, texture ) )
		{
			result = fonts.getSize();

			fonts.add( font );
			fontHashes.add( hash );

			dirtyFonts = true;
		}
	}

	return result;
}

const Texture* Assets::getTexture( int index ) const
{
	LOG_ASSERT( index >= 0 && index < textures.getSize(), "Index %d out of range %d.", index, textures.getSize() );

	return &textures[index];
}

const Mesh* Assets::getMesh( int index ) const
{
	LOG_ASSERT( index >= 0 && index < meshes.getSize(), "Index %d out of range %d.", index, meshes.getSize() );

	return &meshes[index];
}

const Font* Assets::getFont( int index ) const
{
	LOG_ASSERT( index >= 0 && index < fonts.getSize(), "Index %d out of range %d.",
		index, fonts.getSize() );

	return &fonts[index];
}

uint64_t Assets::hashPath( const char* path )
{
	uint64_t hash = 5381;
	int c;

	while( c = *path++ )
		hash = ((hash << 5) + hash) + c; // hash * 33 + c

	return hash;
}

#if _DEBUG
uint64_t Assets::getTimestamp( const char* path )
{
	uint64_t result = 0;

	HANDLE file = CreateFile( path, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL );
	if( file )
	{
		FILETIME writeTime;
		if( GetFileTime( file, NULL, NULL, &writeTime ) )
		{
			result = ((uint64_t)writeTime.dwHighDateTime << 32) | (uint64_t)writeTime.dwLowDateTime;
		}

		CloseHandle( file );
	}

	return result;
}

void Assets::reloadTexture( int index )
{
	LOG_INFO( "Hotloading texture: %s", texturePaths[index] );
	textures[index].load( texturePaths[index] );
	dirtyTextures = true;
}

void Assets::reloadMesh( int index )
{
	LOG_INFO( "Hotloading mesh: %s", meshPaths[index] );
	meshes[index].load( meshPaths[index] );
	dirtyMeshes = true;
}
#endif*/

Assets::Assets()
	: dirtyPacks( false )
{
}

Assets::~Assets()
{
}

void Assets::unload()
{
	const int PACK_COUNT = packs.getSize();
	for( int i=0; i<PACK_COUNT; i++ )
		packs[i].unload();

	packs.clear();
}

void Assets::upload()
{
	if( dirtyPacks )
	{
		const int PACK_COUNT = packs.getSize();
		for( int i=0; i<PACK_COUNT; i++ )
			packs[i].upload();

		dirtyPacks = false;
	}
}

void Assets::loadPack( const char* path )
{
	AssetPack& pack = packs.append();
	if( pack.load( path ) )
	{
		const int TEXTURE_COUNT = pack.getTextureCount();
		for( int i=0; i<pack.getTextureCount(); i++ )
		{
			textures.add( pack.getTexture( i ) );
			textureNames.add( pack.getName( i ) );
		}

		dirtyPacks = true;
	}
}

int Assets::getTextureIndex( const char* name, int len )
{
	int index = -1;

	if( len <= 0 )
		len = strlen( name );
	
	assert( len < ASSET_PACK_NAME_LEN );

	const int TEXTURE_COUNT = textureNames.getSize();
	for( int i=0; i<TEXTURE_COUNT && index < 0; i++ )
		if( strncmp( name, textureNames[i], len ) == 0 )
			index = i;

	return index;
}

const Texture* Assets::getTexture( int index ) const
{
	assert( index >= 0 && index < textures.getSize() );

	return textures[index];
}