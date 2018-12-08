#pragma once

#include "model.h"

namespace Rendering
{
	class InstancedModel
	{
	public:
		InstancedModel();
		~InstancedModel();

		void load( Model* model );
		void finalize();
		void render();

		int addInstance();

		void setPosition( int index, const glm::vec3& position );
		void setOrientation( int index, const glm::quat& orientation );
		void setScale( int index, const glm::vec3& scale );

	private:
		Model* model;
		Array<glm::vec3> positions;
		Array<glm::quat> orientations;
		Array<glm::vec3> scales;
		Array<glm::mat4> worldMatrices;
		int instances;

		GLuint uniformBuffer;
		bool dirtyMatrices;
		bool dirtyBuffer;
	};
}