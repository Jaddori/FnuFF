#pragma once

#include "entity.h"
#include "level.h"
#include "collision_solver.h"

#define PLAYER_SPEED 0.075f
#define PLAYER_SIZE 0.5f
#define PLAYER_HEIGHT 0.5f
#define PLAYER_GRAVITY 0.00982f
#define PLAYER_TERMINAL_VELOCITY -0.175f
#define PLAYER_MAX_CLIP_PLANES 5

#define PLAYER_FLOOR_MIN_NORMAL 0.7f
#define PLAYER_STEP_SIZE 1.5f

#define PLAYER_FLAG_ON_GROUND 0x1

struct trace_t
{
	float fraction;
	glm::vec3 normal;
	glm::vec3 position;
	bool startSolid, allSolid;
};

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
	void move();
	void slideMove();
	trace_t lineTrace( const glm::vec3& start, const glm::vec3& end );
	void categorizePosition();
	glm::vec3 clipVelocity( const glm::vec3& v, const glm::vec3& normal, float overbounce );

	Level* level;
	glm::vec3 velocity;
	glm::vec3 position;
	int flags;

	DebugLine rayLine;
	DebugSphere rayHit;
};