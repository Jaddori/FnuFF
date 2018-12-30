#pragma once

#include "common.h"
#include "collision_solver.h"

struct SolidVertex
{
	glm::vec3 position;
	glm::vec3 normal;
	glm::vec3 color;
};

class Solid
{
public:
	Solid();
	~Solid();

	void upload();

	void read( FILE* file, void* transientMemory );

	GLuint getVAO() const;
	int getVertexCount() const;

private:
	Physics::Plane* planes;
	GLuint vao;
	GLuint vbo;
	int vertexCount;
	//glm::vec3* vertices;
	SolidVertex* vertices;
};