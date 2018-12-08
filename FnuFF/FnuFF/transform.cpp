#include "transform.h"
using namespace Rendering;

Transform::Transform()
	: position( 0.0f, 0.0f, 0.0f ),
	orientation( 1.0f, 0.0f, 0.0f, 0.0f ),
	scale( 1.0f, 1.0f, 1.0f ),
	dirty( true )
{
}

Transform::~Transform()
{
}

void Transform::addPosition( const glm::vec3& p )
{
	position += p;
	dirty = true;
}

void Transform::addOrientation( const glm::quat& o )
{
	orientation *= glm::normalize( o );
	dirty = true;
}

void Transform::addScale( const glm::vec3& s )
{
	scale += s;
	dirty = true;
}

void Transform::addScale( float s )
{
	scale *= s;
}

void Transform::setPosition( const glm::vec3& p )
{
	position = p;
	dirty = true;
}

void Transform::setOrientation( const glm::quat& o )
{
	orientation = glm::normalize( o );
	dirty = true;
}

void Transform::setScale( const glm::vec3& s )
{
	scale = s;
	dirty = true;
}

void Transform::setActive( bool a )
{
	active = a;
}

const glm::vec3& Transform::getPosition() const
{
	return position;
}

const glm::quat& Transform::getOrientation() const
{
	return orientation;
}

const glm::vec3& Transform::getScale() const
{
	return scale;
}

bool Transform::getActive() const
{
	return active;
}

const glm::mat4& Transform::getWorldMatrix()
{
	if( dirty )
	{
		worldMatrix = glm::scale( glm::translate( IDENT, position ) * glm::toMat4( orientation ), scale );
		//worldMatrix = glm::translate( IDENT, position );
		dirty = false;
	}

	return worldMatrix;
}