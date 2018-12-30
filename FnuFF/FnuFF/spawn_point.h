#pragma once

#include "common.h"

class SpawnPoint
{
public:
	SpawnPoint();
	~SpawnPoint();

	void read( FILE* file );

	void setPosition( const glm::vec3& position );
	const glm::vec3& getPosition() const;

private:
	glm::vec3 position;
};