#pragma once

#include "common.h"

namespace Physics
{
	class AABB2
	{
	public:
		AABB2();
		AABB2( const glm::vec3& minPosition, const glm::vec3& maxPosition );
		~AABB2();

	private:
		glm::vec3 minPosition, maxPosition;
	};
}