#pragma once

#include "common.h"

namespace Rendering
{
	struct Billboard
	{
		glm::vec3 position;
		glm::vec4 uv;
		glm::vec2 size;
		float spherical;
		glm::vec3 scroll;
	};
}