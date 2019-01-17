#pragma once

#include "common.h"
#include "collision_solver.h"
#include "assets.h"

struct SolidVertex
{
	glm::vec3 position;
	glm::vec2 uv;
	glm::vec2 lm;
};

struct SolidFace
{
	SolidVertex* vertices;
	int vertexCount;
	int textureIndex;
};

class Solid
{
public:
	Solid();
	~Solid();

	void unload();
	void upload();

	void read( Rendering::Assets* assets, const name_t* textureNames, FILE* file, void* transientMemory );

	GLuint getVAO( int faceIndex ) const;
	int getVertexCount( int faceIndex ) const;
	int getTextureIndex( int faceIndex ) const;
	const Physics::Plane* getPlanes() const;
	int getPlaneCount() const;
	int getFaceCount() const;

private:
	Physics::Plane* planes;
	int planeCount;
	int faceCount;
	GLuint* vaos;
	GLuint* vbos;
	SolidFace* faces;
};