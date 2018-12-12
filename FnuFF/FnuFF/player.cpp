#include "player.h"
using namespace System;
using namespace Physics;

Player::Player()
	: level( NULL )
{
	position.y = 0.5f;
	position.z = 3.0f;
}

Player::~Player()
{
}

void Player::update()
{
	Input& input = *coreData->input;
	Camera* camera = coreData->graphics->getPerspectiveCamera();

	glm::vec3 localMovement;
	if( input.keyDown( SDL_SCANCODE_W ) )
		localMovement.z += 1.0f;
	if( input.keyDown( SDL_SCANCODE_S ) )
		localMovement.z -= 1.0f;
	if( input.keyDown( SDL_SCANCODE_D ) )
		localMovement.x += 1.0f;
	if( input.keyDown( SDL_SCANCODE_A ) )
		localMovement.x -= 1.0f;

	//localMovement.z = 1.0f;

	glm::vec3 direction = camera->getDirection();
	glm::vec3 globalMovement;

	if( fabs( localMovement.z ) > EPSILON )
	{
		glm::vec3 forward = camera->getForward();
		forward.y = 0.0f;
		forward = glm::normalize( forward );

		globalMovement += forward * localMovement.z;
	}

	if( fabs( localMovement.x ) > EPSILON )
	{
		glm::vec3 right = camera->getRight();
		right.y = 0.0f;
		right = glm::normalize( right );

		globalMovement += right * localMovement.x;
	}

	if( glm::length( globalMovement ) > EPSILON )
		globalMovement = glm::normalize( globalMovement ) * 0.05f;

	/*if( input.keyPressed( SDL_SCANCODE_SPACE ) )
	{
		if( platform )
		{
			glm::vec3 a = platform->v[1] - platform->v[0];
			glm::vec3 b = platform->v[2] - platform->v[0];
			glm::vec3 normal = glm::normalize( glm::cross( a, b ) );
			globalMovement += normal * 0.1f;
		}
	}
	else
		globalMovement.y -= PLAYER_GRAVITY;*/

	if( input.buttonDown( SDL_BUTTON_LEFT ) )
	{
		Point deltaMouse = input.getMouseDelta();
		camera->updateDirection( deltaMouse.x, deltaMouse.y );
	}

	velocity.x *= 0.48f;
	velocity.z *= 0.48f;
	velocity += globalMovement;

	CollisionSolver& solver = *coreData->collisionSolver;

	glm::vec3 start = position;
	glm::vec3 end = position + globalMovement;
	glm::vec3 finalPosition = end;

	Sphere sphere = { end, PLAYER_SIZE };
	const int TRIANGLE_COUNT = level->getTriangleCount();

	for( int i=0; i<TRIANGLE_COUNT; i++ )
	{
		const Triangle* triangle = level->getTriangle( i );
		Hit hit;

		if( solver.sphere( sphere, *triangle, &hit ) )
		{
			finalPosition = start;
		}
	}

	position = finalPosition;
	camera->setPosition( position + glm::vec3( 0, 1.0f, 0 ) );

	// check collision
	/*CollisionSolver& solver = *coreData->collisionSolver;

	glm::vec3 start = position;
	glm::vec3 end = position + velocity;
	Ray ray = rayFromPoints( start, end );
	if( ray.length > EPSILON )
	{
		const int TRIANGLE_COUNT = level->getTriangleCount();

		float shortest = ray.length + 1.0f;
		glm::vec3 finalPosition = position + velocity;

		for( int i=0; i<TRIANGLE_COUNT; i++ )
		{
			const Triangle* triangle = level->getTriangle( i );
			Hit hit;

			if( solver.ray( ray, *triangle, &hit ) )
			{
				if( hit.length < shortest )
				{
					shortest = hit.length;
					velocity = glm::vec3( 0.0f );
					finalPosition = hit.position;
					platform = triangle;
				}
			}
		}

		position = finalPosition;
	}

	position += velocity;
	camera->setPosition( position + glm::vec3( 0.0f, 1.0f, 0.0f ) );*/
}

void Player::setLevel( Level* lvl )
{
	level = lvl;
}

void Player::setPosition( const glm::vec3& p )
{
	position = p;
}