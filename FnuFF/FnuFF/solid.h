#pragma once

#include "common.h"
#include "collision_solver.h"

struct SolidVertex
{
	glm::vec3 position;
	glm::vec2 uv;
};

struct SolidFace
{
	SolidVertex* vertices;
	int vertexCount;
	int packIndex, textureIndex;
};

class Solid
{
public:
	Solid();
	~Solid();

	void upload();

	void read( FILE* file, void* transientMemory );

	GLuint getVAO( int faceIndex ) const;
	int getVertexCount( int faceIndex ) const;
	int getTextureIndex( int faceIndex ) const;
	const Physics::Plane* getPlanes() const;
	int getFaceCount() const;

private:
	Physics::Plane* planes;
	int faceCount;
	GLuint* vaos;
	GLuint* vbos;
	SolidFace* faces;
};