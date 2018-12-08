#pragma once

#include "common.h"
#include "mesh.h"
#include "texture.h"

namespace Rendering
{
	class Model
	{
	public:
		Model();
		~Model();

		void load( const Mesh* mesh, const Texture* texture );
		void unload();

		void render();

		const Mesh* getMesh() const;
		const Texture* getTexture() const;

	private:
		const Mesh* mesh;
		const Texture* texture;

		GLuint vertexArray;
		union
		{
			GLuint buffers[2];
			struct
			{
				GLuint vertexBuffer;
				GLuint indexBuffer;
			};
		};
	};
}