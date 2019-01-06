#include "player.h"
using namespace System;
using namespace Physics;

#define MAX_COLLISION_PLANES 5

Player::Player()
	: level( NULL ), fontIndex( -1 ), hasStopped( false ), automove( false )
{
	position.y = 0.5f;
	position.z = 3.0f;
}

Player::~Player()
{
}

void Player::load()
{
	fontIndex = coreData->assets->loadFont( "./assets/fonts/verdana12.bin", "./assets/fonts/verdana12.tga" );

	LOG_ASSERT( fontIndex >= 0, "Failed to load font for player debug information." );
}

void Player::update()
{
	categorizePosition();

	move();

	Camera* camera = coreData->graphics->getPerspectiveCamera();
	camera->setPosition( position + glm::vec3( 0, 1.0f, 0 ) );

	// DEBUG:
	if( position.y < 0 )
	{
		position.y = 0.0f;
		velocity.y = 0.0f;
	}

	categorizePosition();
}

void Player::move()
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
	if( input.keyReleased( SDL_SCANCODE_M ) )
		automove = !automove;

	if( automove )
		localMovement.z = 1.0f;

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
	{
		if( (flags & PLAYER_FLAG_ON_GROUND) == 0 )
			globalMovement.y -= PLAYER_GRAVITY;
		else
			globalMovement.y = 0.0f;
	}

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
	if( ( flags & PLAYER_FLAG_ON_GROUND ) && velocity.y < 0.0f )
		velocity.y = 0.0f;

	raydir = glm::normalize( velocity );

	trace_t trace = lineTrace( position, position + velocity );

	if( trace.fraction == -0.0f )
	{
		hasStopped = true;
	}

	if( trace.fraction >= 1.0f )
	{
		position = trace.position;
		return;
	}

	glm::vec3 originalPosition = position;
	glm::vec3 originalVelocity = velocity;

	slideMove();

	return;

	glm::vec3 down = position;
	glm::vec3 downVelocity = velocity;

	position = originalPosition;
	velocity = originalVelocity;

	glm::vec3 dest = position;
	dest.y += PLAYER_STEP_SIZE;

	trace = lineTrace( position, dest );
	//if( trace.fraction > 0.0f )
	if( !trace.startSolid && !trace.allSolid )
	{
		position = trace.position;
	}

	slideMove();

	dest = position;
	dest.y -= PLAYER_STEP_SIZE;

	trace = lineTrace( position, dest );
	if( trace.normal.y < PLAYER_FLOOR_MIN_NORMAL )
	{
		position = down;
		velocity = downVelocity;
	}
	else
	{
		//if( trace.fraction > 0.0f )
		if( !trace.startSolid && !trace.allSolid )
		{
			position = trace.position;
		}

		glm::vec3 up = position;

		float downdist =
			( down.x - originalPosition.x ) * ( down.x - originalPosition.x ) +
			( down.z - originalPosition.z ) * ( down.z - originalPosition.z );

		float updist =
			( up.x - originalPosition.x ) * ( up.x - originalPosition.x ) +
			( up.z - originalPosition.z ) * ( up.z - originalPosition.z );

		if( downdist > updist )
		{
			position = down;
			velocity = downVelocity;
		}
		else
			velocity.y = downVelocity.y;
	}
}

void Player::slideMove()
{
	glm::vec3 planes[MAX_COLLISION_PLANES];
	int numplanes = 0;

	float timeLeft = 1.0f;

	glm::vec3 primalVelocity = velocity;
	for( int bumpcount = 0; bumpcount < 4; bumpcount++ )
	{
		if( fabs( velocity.x ) < EPSILON && fabs( velocity.y ) < EPSILON && fabs( velocity.z ) < EPSILON )
			break;

		glm::vec3 nextPosition = position + velocity * timeLeft;

		trace_t trace = lineTrace( position, nextPosition );
		if( trace.startSolid || trace.allSolid )
		{
			velocity = glm::vec3( 0.0f );
			break;
		}

		if( trace.fraction > 0 )
		{
			position = trace.safePosition;
			numplanes = 0;
		}

		if( trace.fraction >= 1.0f )
			break;

		timeLeft -= timeLeft * trace.fraction;

		planes[numplanes] = trace.normal;
		numplanes++;

		bool validDirection = false;
		for( int i=0; i<numplanes && !validDirection; i++ )
		{
			velocity = clipVelocity( velocity, planes[i], 1.1f );

			validDirection = true;
			for( int j=0; j<numplanes && validDirection; j++ )
			{
				if( j != i )
				{
					if( glm::dot( velocity, planes[j] ) < 0 )
						validDirection = false;
				}
			}
		}

		if( !validDirection )
		{
			if( numplanes == 2 )
			{
				glm::vec3 direction = glm::cross( planes[0], planes[1] );
				float distance = glm::dot( direction, velocity );
				velocity = direction * distance;
			}
			else
			{
				velocity = glm::vec3( 0.0f );
				break;
			}
		}

		if( glm::dot( velocity, primalVelocity ) <= 0.0f )
		{
			velocity = glm::vec3( 0.0f );
			break;
		}
	}
}

trace_t Player::lineTrace( const glm::vec3& start, const glm::vec3& end )
{
	trace_t result;
	result.fraction = 1.0f;
	result.position = end;
	result.safePosition = end;
	result.startSolid = false;
	
	bool endSolid = false;

	CollisionSolver& solver = *coreData->collisionSolver;

	Hit hit = {};
	Ray ray =
	{
		start,
		glm::normalize( end - start ),
		glm::distance( start, end )
	};

	const Solid* solids = level->getSolids();
	const int SOLID_COUNT = level->getSolidCount();

	for( int curSolid = 0; curSolid < SOLID_COUNT; curSolid++ )
	{
		const Solid& solid = solids[curSolid];
		const Plane* planes = solid.getPlanes();
		const int FACE_COUNT = solid.getFaceCount();

		bool startInsideSolid = true;
		bool endInsideSolid = true;
		for( int curPlane = 0; curPlane < FACE_COUNT; curPlane++ )
		{
			const Plane& plane = planes[curPlane];

			if( startInsideSolid )
			{
				float distance = glm::dot( plane.normal, start ) - plane.offset;
				if( distance > EPSILON )
					startInsideSolid = false;
			}

			if( endInsideSolid )
			{
				float distance = glm::dot( plane.normal, end ) - plane.offset;
				if( distance > EPSILON )
					endInsideSolid = false;
			}

			//if( glm::dot( plane.normal, ray.direction ) < 0 )
			{
				if( solver.ray( ray, plane, PLAYER_SIZE, &hit ) )
				{
					if( hit.length > -EPSILON && hit.length < ray.length )
					{
						bool behindAll = true;
						for( int curOtherPlane = 0; curOtherPlane < FACE_COUNT; curOtherPlane++ )
						{
							if( curOtherPlane != curPlane )
							{
								const Plane& otherPlane = planes[curOtherPlane];

								float distance = glm::dot( otherPlane.normal, hit.position ) - otherPlane.offset - PLAYER_SIZE;
								if( distance > EPSILON )
									behindAll = false;
							}
						}

						if( behindAll )
						{
							float fraction = hit.length / ray.length;
							if( fraction < result.fraction )
							{
								result.fraction = fraction;
								if( result.fraction > 1.0f )
									result.fraction = 1.0f;
								result.position = hit.position;
								result.normal = plane.normal;
								result.safePosition = ray.start + ray.direction * ( hit.length - PLAYER_TRACE_MARGIN );
							}
						}
					}
				}
			}
		}

		if( startInsideSolid )
			result.startSolid = true;
		if( endInsideSolid )
			endSolid = true;
	}

	result.allSolid = (result.startSolid && endSolid);

	return result;
}

void Player::categorizePosition()
{
	flags = 0;

	glm::vec3 below = position - glm::vec3( 0.0f, 0.0125f, 0.0f );

	trace_t trace = lineTrace( position, below );
	if( trace.startSolid || (trace.fraction < 1.0f && trace.normal.y > PLAYER_FLOOR_MIN_NORMAL) )
	{
		flags |= PLAYER_FLAG_ON_GROUND;
	}
}

glm::vec3 Player::clipVelocity( const glm::vec3& v, const glm::vec3& normal, float overbounce )
{
	glm::vec3 result;

	float backoff = glm::dot( v, normal ) * overbounce;

	for( int i=0; i<3; i++ )
	{
		float change = normal[i] * backoff;
		result[i] = v[i] - change;
		if( result[i] > -EPSILON && result[i] < EPSILON )
			result[i] = 0;
	}

	return result;
}

void Player::render()
{
	coreData->debugShapes->addLine( rayLine, false );
	coreData->debugShapes->addSphere( rayHit, false );

	char buffer[1024] = {};

	_snprintf( buffer, 1024, "Position: (%f, %f, %f)", position.x, position.y, position.z );
	coreData->graphics->queueText( fontIndex, buffer, glm::vec3( 32, 32, 0 ), glm::vec4( 1.0f ) );

	_snprintf( buffer, 1024, "On ground: %d", ( flags & PLAYER_FLAG_ON_GROUND ) );
	coreData->graphics->queueText( fontIndex, buffer, glm::vec3( 32, 48, 0 ), glm::vec4( 1.0f ) );

	_snprintf( buffer, 1024, "Velocity: (%f, %f, %f)", velocity.x, velocity.y, velocity.z );
	coreData->graphics->queueText( fontIndex, buffer, glm::vec3( 32, 64, 0 ), glm::vec4( 1.0f ) );
}

void Player::setLevel( Level* lvl )
{
	level = lvl;
}

void Player::setPosition( const glm::vec3& p )
{
	position = p;
}