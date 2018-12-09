#pragma once

#include "entity.h"
#include "prop.h"
#include "collision_solver.h"

class Level : public Entity
{
public:
	Level();
	~Level();

	bool load( const char* filepath );
	void unload();

	void render();
	int raytrace( const Physics::Ray& ray, glm::vec3& hitPoint );

	const Physics::Triangle* getTriangle( int index ) const;

private:
	Transform transform;
	int meshIndex;
	Physics::Triangle* triangles;
	int triangleCount;
};