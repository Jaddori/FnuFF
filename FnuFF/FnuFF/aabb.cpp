#include "aabb.h"
using namespace Physics;

AABB2::AABB2()
{
}

AABB2::AABB2( const glm::vec3& m1, const glm::vec3& m2 )
	: minPosition( m1 ), maxPosition( m2 )
{
}

AABB2::~AABB2()
{
}