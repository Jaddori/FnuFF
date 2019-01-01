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

	if( input.keyPressed( SDL_SCANCODE_SPACE ) )
	{
		globalMovement.y = 0.2f;
	}
	else
		globalMovement.y -= PLAYER_GRAVITY;

	if( input.buttonDown( SDL_BUTTON_LEFT ) )
	{
		Point deltaMouse = input.getMouseDelta();
		camera->updateDirection( deltaMouse.x, deltaMouse.y );
	}

	velocity.x *= 0.48f;
	velocity.z *= 0.48f;
	if( velocity.y < PLAYER_TERMINAL_VELOCITY )
		velocity.y = PLAYER_TERMINAL_VELOCITY;

	velocity += globalMovement;

	CollisionSolver& solver = *coreData->collisionSolver;

	if( velocity.x < 0.0f )
	{
		int f = 0;
	}

	glm::vec3 start = position;
	glm::vec3 end = position + velocity;

	Hit hit;

	Ray ray = 
	{
		start,
		glm::normalize( end - start ),
		glm::distance( start, end ),
	};

	const Plane* collisionPlanes[5] = {};
	int numplanes = 0;

	const Solid* solids = level->getSolids();
	const int SOLID_COUNT = level->getSolidCount();
	for( int curSolid=0; curSolid < SOLID_COUNT; curSolid++ )
	{
		const Solid& solid = solids[curSolid];
		const Plane* planes = solid.getPlanes();

		const int PLANE_COUNT = solid.getPlaneCount();
		for( int curPlane = 0; curPlane < PLANE_COUNT; curPlane++ )
		{
			const Plane& plane = planes[curPlane];

			if( glm::dot( plane.normal, ray.direction ) < 0 )
			{
				if( solver.ray( ray, plane, PLAYER_SIZE, &hit ) )
				{
					if( hit.length < ray.length )
					{
						if( hit.length > 0 )
						{
							bool behindAll = true;
							for( int otherPlane = 0; otherPlane < PLANE_COUNT && behindAll; otherPlane++ )
							{
								if( otherPlane != curPlane )
								{
									const Plane& oplane = planes[otherPlane];

									float distance = glm::dot( oplane.normal, hit.position ) - oplane.offset - PLAYER_SIZE;
									if( distance > EPSILON )
										behindAll = false;
								}
							}

							if( behindAll )
							{
								if( numplanes < 5 )
								{
									collisionPlanes[numplanes] = &plane;
									numplanes++;
								}
							}
						}
					}
				}
			}
		}
	}

	if( numplanes > 0 )
	{
		if( numplanes > 1 )
		{
			int f = 0;
		}

		glm::vec3 newVelocity = velocity;
		bool allParallel = false;
		for( int i=0; i<numplanes && !allParallel; i++ )
		{
			const Plane* plane = collisionPlanes[i];
			float backoff = glm::dot( velocity, plane->normal );

			for( int j=0; j<3; j++ )
			{
				float change = plane->normal[j]*backoff;
				newVelocity[j] = velocity[j] - change;

				if( fabs( newVelocity[j] ) < EPSILON )
					newVelocity[j] = 0.0f;
			}

			allParallel = true;
			for( int j=0; j<numplanes && allParallel; j++ )
			{
				if( j != i )
				{
					float dotValue = glm::dot( newVelocity, collisionPlanes[j]->normal );
					if( dotValue < 0.0f )
						allParallel = false;
				}
			}
		}

		if( allParallel )
			velocity = newVelocity;
		else
		{
			if( numplanes == 2 )
			{
				glm::vec3 dir = glm::cross( collisionPlanes[0]->normal, collisionPlanes[1]->normal );
				float d = glm::dot( velocity, dir );
				velocity = dir * d;
			}
			else
			{
				velocity = glm::vec3( 0.0f, 0.0f, 0.0f );
			}
		}
	}

	position += velocity;

	if( position.y < 0 )
	{
		position.y = 0.0f;
		velocity.y = 0.0f;
	}

	camera->setPosition( position + glm::vec3( 0, 1.0f, 0 ) );
}

void Player::render()
{
	coreData->debugShapes->addLine( rayLine, false );
	coreData->debugShapes->addSphere( rayHit, false );
}

void Player::setLevel( Level* lvl )
{
	level = lvl;
}

void Player::setPosition( const glm::vec3& p )
{
	position = p;
}