#pragma once

#include "entity.h"
#include "level.h"
#include "collision_solver.h"

#define PLAYER_SIZE 0.5f
#define PLAYER_GRAVITY 0.00982f

class Player : public Entity
{
public:
	Player();
	~Player();

	void update();
	void render();

	void setLevel( Level* level );
	void setPosition( const glm::vec3& position );

private:
	Level* level;
	glm::vec3 velocity;
	glm::vec3 position;
	const Physics::Triangle* platform;

	DebugLine rayLine;
	DebugSphere rayHit;
};