#include "ray.h"
using namespace Physics;

Ray2::Ray2()
{
}

Ray2::Ray2( const glm::vec3& s, const glm::vec3& d, float len )
	: start( s ), length( len )
{
	direction = glm::normalize( d );
}

Ray2::~Ray2()
{
}