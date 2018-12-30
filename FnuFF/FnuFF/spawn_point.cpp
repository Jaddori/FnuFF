#include "spawn_point.h"

SpawnPoint::SpawnPoint()
{
}

SpawnPoint::~SpawnPoint()
{
}

void SpawnPoint::read( FILE* file )
{
	fread( &position, sizeof(position), 1, file );
}

void SpawnPoint::setPosition( const glm::vec3& p )
{
	position = p;
}

const glm::vec3& SpawnPoint::getPosition() const
{
	return position;
}