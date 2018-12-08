#pragma once

#include "common.h"

namespace Rendering
{
	class Transform
	{
	public:
		Transform();
		~Transform();

		void addPosition( const glm::vec3& position );
		void addOrientation( const glm::quat& orientation );
		void addScale( const glm::vec3& scale );
		void addScale( float scale );

		void setPosition( const glm::vec3& position );
		void setOrientation( const glm::quat& orientation );
		void setScale( const glm::vec3& scale );
		void setActive( bool active );

		const glm::vec3& getPosition() const;
		const glm::quat& getOrientation() const;
		const glm::vec3& getScale() const;
		bool getActive() const;
		const glm::mat4& getWorldMatrix();

	private:
		glm::vec3 position;
		glm::quat orientation;
		glm::vec3 scale;
		bool active;

		glm::mat4 worldMatrix;
		bool dirty;
	};
}