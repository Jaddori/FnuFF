#pragma once

#include "common.h"

namespace Physics
{
	class Ray2
	{
	public:
		Ray2();
		Ray2( const glm::vec3& start, const glm::vec3& direction, float length );
		~Ray2();

	private:
		glm::vec3 start, direction;
		float length;
	};
}