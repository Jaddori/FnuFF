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

	if( input.buttonReleased( SDL_BUTTON_RIGHT ) )
	{
		glm::vec3 forward = camera->getForward();
		Ray ray =
		{
			position + glm::vec3( 0, 1, 0 ),
			glm::normalize( forward ),
			16.0f
		};

		rayLine.start = ray.start;
		rayLine.end = ray.start + ray.direction* 16.0f;
		rayLine.color = glm::vec4( 1.0f, 1.0f, 0.0f, 1.0f );

		Hit hit = {};
		float minHitDistance = 9999.0f;

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

				//if( glm::dot( plane.normal, ray.direction ) < 0 )
				{
					if( solver.ray( ray, plane, PLAYER_SIZE, &hit ) )
					{
						if( hit.length < ray.length )
						{
							if( hit.length > 0 && hit.length < minHitDistance )
							{
								bool behindAll = true;
								for( int otherPlane = 0; otherPlane < PLANE_COUNT && behindAll; otherPlane++ )
								{
									if( otherPlane != curPlane )
									{
										const Plane& oplane = planes[otherPlane];

										float distance = glm::dot( oplane.normal, hit.position ) - oplane.offset;
										if( distance > EPSILON )
											behindAll = false;
									}
								}

								if( behindAll )
								{
									minHitDistance = hit.length;
									rayHit.position = hit.position;
									rayHit.radius = 0.5f;
									rayHit.color = glm::vec4( 1.0f, 0.0f, 0.0f, 1.0f );
								}
							}
						}
					}
				}
			}
		}
	}

	glm::vec3 start = position;
	//glm::vec3 end = position + globalMovement;
	glm::vec3 end = position + velocity;

	Hit hit;

	float minHitDistance = 9999.0f;

	Ray ray = 
	{
		start,
		glm::normalize( end - start ),
		glm::distance( start, end ),
	};

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
								//globalMovement -= plane.normal * glm::dot( globalMovement, plane.normal );
								velocity -= plane.normal * glm::dot( velocity, plane.normal );
							}
						}
					}
				}
			}
		}
	}

	//position += globalMovement;
	position += velocity;
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