#pragma once

#include "common.h"
#include "vertex.h"
#include "collision_solver.h"

namespace Rendering
{
	class Mesh
	{
	public:
		Mesh();
		~Mesh();

		bool load( const char* path );
		void unload();
		void upload();

		void bind() const;
		void render() const;

		int getVertexCount() const;
		bool getUploaded() const;

		const Vertex* getVertices() const;
		const Physics::AABB* getBoundingBox() const;

	private:
		int vertexCount;

		Vertex* vertices;

		GLuint vertexArray;
		GLuint vertexBuffer;

		Physics::AABB boundingBox;

		bool uploaded;
	};
}