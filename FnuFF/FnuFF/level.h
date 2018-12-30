#pragma once

#include "entity.h"
#include "prop.h"
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

private:
	//Transform transform;
	//int meshIndex;
	//Physics::Triangle* triangles;
	//int triangleCount;

	bool uploaded;

	Solid* solids;
	int solidCount;

	SpawnPoint* spawnPoints;
	int spawnPointCount;
};