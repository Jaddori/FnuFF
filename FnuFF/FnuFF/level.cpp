#include "level.h"
using namespace Physics;

Level::Level()
	: uploaded( false )
{
}

Level::~Level()
{
}

bool Level::load( const char* filepath )
{
	bool result = false;

	FILE* file = fopen( filepath, "rb" );
	if( file )
	{
		solidCount = 0;
		spawnPointCount = 0;

		fread( &solidCount, sizeof(solidCount), 1, file );
		fread( &spawnPointCount, sizeof(spawnPointCount), 1, file );

		int contentPacks = 0;
		fread( &contentPacks, sizeof(contentPacks), 1, file );
		for( int i=0; i<contentPacks; i++ )
			fseek( file, 64, SEEK_CUR );

		int textureNameCount = 0;
		fread( &textureNameCount, sizeof(textureNameCount), 1, file );

		name_t* textureNames = new name_t[textureNameCount];
		for( int i=0; i<textureNameCount; i++ )
			fread( textureNames[i], 1, ASSET_PACK_NAME_LEN, file );

		solids = new Solid[solidCount];
		spawnPoints = new SpawnPoint[spawnPointCount];

		for( int i=0; i<solidCount; i++ )
		{
			solids[i].read( coreData->assets, textureNames, file, coreData->transientMemory );
		}

		for( int i=0; i<spawnPointCount; i++ )
		{
			spawnPoints[i].read( file );
		}

		fclose( file );

		delete[] textureNames;
		uploaded = false;

#ifdef _DEBUG
		timestamp = getTimestamp( filepath );
		strcpy( path, filepath );
		path[strlen(filepath)] = 0;
#endif
	}

	return result;
}

void Level::unload()
{
	for( int i=0; i<solidCount; i++ )
	{
		solids[i].unload();
	}

	delete[] solids;
}

void Level::upload()
{
	if( !uploaded )
	{
		for( int i=0; i<solidCount; i++ )
		{
			solids[i].upload();
		}

		uploaded = true;
	}
}

void Level::render()
{
	for( int curSolid=0; curSolid<solidCount; curSolid++ )
	{
		coreData->graphics->queueSolid( solids + curSolid );
	}
}

const SpawnPoint& Level::getRandomSpawnPoint() const
{
	int index = rand() % spawnPointCount; 
	return spawnPoints[index];
}

const Solid* Level::getSolids() const
{
	return solids;
}

int Level::getSolidCount() const
{
	return solidCount;
}

#ifdef _DEBUG
bool Level::hotload()
{
	bool result = false;

	uint64_t curTimestamp = getTimestamp( path );
	if( curTimestamp != timestamp )
	{
		unload();
		load( path );

		result = true;
	}

	return result;
}

uint64_t Level::getTimestamp( const char* path )
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
#endif