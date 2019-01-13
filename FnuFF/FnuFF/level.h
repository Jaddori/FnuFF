#pragma once

#include "entity.h"
#include "collision_solver.h"
#include "solid.h"
#include "spawn_point.h"

class Level : public Entity
{
public:
	Level();
	~Level();

	bool load( const char* filepath );
	void unload();
	void upload();

	void render();

	const SpawnPoint& getRandomSpawnPoint() const;
	const Solid* getSolids() const;
	int getSolidCount() const;

#ifdef _DEBUG
	bool hotload();
#endif

private:
#ifdef _DEBUG
	uint64_t getTimestamp( const char* path );

	uint64_t timestamp;
	char path[1024];
#endif

	bool uploaded;

	Solid* solids;
	int solidCount;

	SpawnPoint* spawnPoints;
	int spawnPointCount;
};