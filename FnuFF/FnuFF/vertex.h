#pragma once

#include "common.h"
#include "glm\gtc\epsilon.hpp"

namespace Rendering
{
	struct Vertex
	{
		glm::vec3 position;
		glm::vec2 uv;
		glm::vec3 normal;
		glm::vec3 tangent;
		glm::vec3 bitangent;
	};

	inline bool sameVertex( const Vertex& a, const Vertex& b )
	{
		return
		(
			fabs( a.position.x - b.position.x ) < EPSILON &&
			fabs( a.position.y - b.position.y ) < EPSILON &&
			fabs( a.position.z - b.position.z ) < EPSILON
		);
	}
}