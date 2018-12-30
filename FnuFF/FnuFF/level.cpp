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

	FILE* file = fopen( filepath, "r" );
	if( file )
	{
		solidCount = 0;
		spawnPointCount = 0;

		fread( &solidCount, sizeof(solidCount), 1, file );
		fread( &spawnPointCount, sizeof(spawnPointCount), 1, file );

		solids = new Solid[solidCount];
		spawnPoints = new SpawnPoint[spawnPointCount];

		for( int i=0; i<solidCount; i++ )
		{
			solids[i].read( file, coreData->transientMemory );
		}

		for( int i=0; i<spawnPointCount; i++ )
		{
			spawnPoints[i].read( file );
		}

		fclose( file );
	}

	return result;
}

void Level::unload()
{
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
	for( int i=0; i<solidCount; i++ )
	{
		coreData->graphics->queueVao( solids[i].getVAO(), solids[i].getVertexCount() );
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