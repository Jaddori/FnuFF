#include "camera.h"
using namespace Rendering;

Camera::Camera()
	: position( 0.0f ), direction( 0.0f, 0.0f, 1.0f ),
	right( 1.0f, 0.0f, 0.0f ), up( 0.0f, 1.0f, 0.0f ),
	dirtyViewMatrix( true ), dirtyFrustum( true ),
	horizontalAngle( 0.0f ), verticalAngle( 0.0f )
{
}

Camera::~Camera()
{
}

void Camera::finalize()
{
	if( dirtyViewMatrix )
	{
		viewMatrix = glm::lookAt( position, position + direction, glm::vec3( 0.0f, 1.0f, 0.0f ) );
		dirtyViewMatrix = false;
	}

	if( dirtyFrustum )
	{
	}

	finalPosition = position;
	finalDirection = direction;
	finalRight = right;
	finalUp = up;
}

void Camera::project( const glm::vec3& worldCoordinates, Point& result )
{
	glm::vec3 windowCoordinates = glm::project( worldCoordinates, viewMatrix, projectionMatrix, WINDOW_VIEWPORT );

	windowCoordinates.y = WINDOW_HEIGHT - windowCoordinates.y;

	result.x = (int)( windowCoordinates.x+0.5f );
	result.y = (int)( windowCoordinates.y+0.5f );
}

void Camera::unproject( const Point& windowCoordinates, float depth, glm::vec3& result )
{
	result = glm::unProject( glm::vec3( windowCoordinates.x, WINDOW_HEIGHT - windowCoordinates.y, depth ), viewMatrix, projectionMatrix, WINDOW_VIEWPORT );
}

void Camera::relativeMovement( const glm::vec3& localMovement )
{
	// move backwards and forwards
	if( fabs( localMovement.z ) > EPSILON )
	{
		glm::vec3 forward = glm::normalize( direction );
		position += forward * localMovement.z;
	}

	// move left and right
	if( fabs( localMovement.x ) > EPSILON )
	{
		glm::vec3 right( glm::sin( horizontalAngle - PI*0.5f ),
							0.0f,
							glm::cos( horizontalAngle - PI*0.5f ) );
		position += right * localMovement.x;
	}

	// move up and down
	if( fabs( localMovement.y ) > EPSILON )
	{
		glm::vec3 up( 0.0f, 1.0f, 0.0f );
		position += up * localMovement.y;
	}

	dirtyViewMatrix = true;
	dirtyFrustum = true;
}

void Camera::absoluteMovement( const glm::vec3& worldMovement )
{
	position += worldMovement;

	dirtyViewMatrix = true;
	dirtyFrustum = true;
}

void Camera::updateDirection( int deltaX, int deltaY )
{
	horizontalAngle += (float)deltaX * CAMERA_HORIZONTAL_SENSITIVITY;
	verticalAngle += (float)deltaY * CAMERA_VERTICAL_SENSITIVITY;

	// clamp angles
	if( horizontalAngle > 2*PI )
		horizontalAngle -= 2*PI;
	else if( horizontalAngle < -2*PI )
		horizontalAngle += 2*PI;

	if( fabs( verticalAngle ) > PI*0.5f )
		verticalAngle = PI*0.5f;

	// calculate new direction
	direction = glm::vec3
	(
		glm::cos( verticalAngle ) * glm::sin( horizontalAngle ),
		glm::sin( verticalAngle ),
		glm::cos( verticalAngle ) * glm::cos( horizontalAngle )
	);

	// calculate up vector
	right = glm::vec3
	(
		glm::sin( horizontalAngle - PI * 0.5f ),
		0,
		glm::cos( horizontalAngle - PI * 0.5f )
	);

	up = glm::cross( right, direction );

	dirtyViewMatrix = true;
	dirtyFrustum = true;
}

void Camera::updatePerspective( float width, float height )
{
	projectionMatrix = glm::perspectiveFov( CAMERA_FOV, width, height, CAMERA_NEAR, CAMERA_FAR );
}

void Camera::updateOrthographic( float width, float height )
{
	projectionMatrix = glm::ortho( 0.0f, width, height, 0.0f );
}

void Camera::setPosition( const glm::vec3& p )
{
	position = p;

	dirtyViewMatrix = true;
	dirtyFrustum = true;
}

void Camera::setDirection( const glm::vec3& d )
{
	direction = glm::normalize( d );

	verticalAngle = glm::asin( d.y );

	float acva = glm::acos( verticalAngle );

	if( acva < EPSILON || acva > EPSILON )
		horizontalAngle = glm::acos( d.z / glm::cos( verticalAngle ) );
	else
		horizontalAngle = glm::acos( 0.0f );

	// calculate up vector
	right = glm::vec3
	(
		glm::sin( horizontalAngle - PI * 0.5f ),
		0.0f,
		glm::cos( horizontalAngle - PI * 0.5f )
	);

	up = glm::cross( right, direction );

	dirtyViewMatrix = true;
	dirtyFrustum = true;
}

void Camera::setHorizontalAngle( float angle )
{
	horizontalAngle = angle;

	// clamp angles
	if( horizontalAngle > 2*PI )
		horizontalAngle -= 2*PI;
	else if( horizontalAngle < -2*PI )
		horizontalAngle += 2*PI;

	// calculate new direction
	direction = glm::vec3(
		glm::cos( verticalAngle ) * glm::sin( horizontalAngle ),
		glm::sin( verticalAngle ),
		glm::cos( verticalAngle ) * glm::cos( horizontalAngle )
	);

	// calculate up vector
	right = glm::vec3(
		glm::sin( horizontalAngle - 3.14f * 0.5f ),
		0,
		glm::cos( horizontalAngle - 3.14f * 0.5f )
	);

	up = glm::cross( right, direction );

	dirtyViewMatrix = true;
	dirtyFrustum = true;
}

void Camera::setVerticalAngle( float angle )
{
	verticalAngle = angle;

	// clamp angles
	if( fabs( verticalAngle ) > PI*0.5f )
		verticalAngle = PI*0.5f;

	// calculate new direction
	direction = glm::vec3(
		glm::cos( verticalAngle ) * glm::sin( horizontalAngle ),
		glm::sin( verticalAngle ),
		glm::cos( verticalAngle ) * glm::cos( horizontalAngle )
	);

	// calculate up vector
	right = glm::vec3(
		glm::sin( horizontalAngle - 3.14f * 0.5f ),
		0,
		glm::cos( horizontalAngle - 3.14f * 0.5f )
	);

	up = glm::cross( right, direction );

	dirtyViewMatrix = true;
	dirtyFrustum = true;
}

const glm::mat4& Camera::getViewMatrix() const
{
	return viewMatrix;
}

const glm::mat4& Camera::getProjectionMatrix() const
{
	return projectionMatrix;
}

const glm::vec3& Camera::getPosition() const
{
	return position;
}

const glm::vec3& Camera::getDirection() const
{
	return direction;
}

const glm::vec3& Camera::getForward() const
{
	return direction;
}

const glm::vec3& Camera::getRight() const
{
	return right;
}

const glm::vec3& Camera::getUp() const
{
	return up;
}

const glm::vec3& Camera::getFinalPosition() const
{
	return finalPosition;
}

const glm::vec3& Camera::getFinalDirection() const
{
	return finalDirection;
}

const glm::vec3& Camera::getFinalForward() const
{
	return finalDirection;
}

const glm::vec3& Camera::getFinalRight() const
{
	return finalRight;
}

const glm::vec3& Camera::getFinalUp() const
{
	return finalUp;
}