#include "solid.h"
#include "vertex.h"
using namespace Physics;
using namespace Rendering;

Solid::Solid()
	: planes( NULL ), faceCount( 0 ), vaos( NULL ), vbos( NULL ), faces( NULL )
{
}

Solid::~Solid()
{
}

void Solid::read( Assets* assets, const name_t* textureNames, FILE* file, void* transientMemory )
{
	planeCount = 0;
	fread( &planeCount, sizeof(planeCount), 1, file );

	planes = new Plane[planeCount];

	for( int i=0; i<planeCount; i++ )
	{
		fread( &planes[i], sizeof(planes[i]), 1, file );
	}

	faceCount = 0;
	fread( &faceCount, sizeof(faceCount), 1, file );

	faces = new SolidFace[faceCount];
	vaos = new GLuint[faceCount];
	vbos = new GLuint[faceCount];

	for( int i=0; i<faceCount; i++ )
	{
		int textureIndex = 0;
		fread( &textureIndex, sizeof(textureIndex), 1, file );

		faces[i].textureIndex = assets->getTextureIndex( textureNames[textureIndex] );

		uint32_t faceVertexCount = 0;
		fread( &faceVertexCount, sizeof(faceVertexCount), 1, file );

		faces[i].vertexCount = faceVertexCount;
		faces[i].vertices = new SolidVertex[faceVertexCount];
		fread( faces[i].vertices, sizeof(SolidVertex), faceVertexCount, file );
	}
}

void Solid::unload()
{
	glDeleteVertexArrays( faceCount, vaos );
	glDeleteBuffers( faceCount, vbos );
}

void Solid::upload()
{
	glGenVertexArrays( faceCount, vaos );
	glGenBuffers( faceCount, vbos );

	for( int i=0; i<faceCount; i++ )
	{
		SolidFace& face = faces[i];

		glBindVertexArray( vaos[i] );

		glEnableVertexAttribArray( 0 );
		glEnableVertexAttribArray( 1 );

		glBindBuffer( GL_ARRAY_BUFFER, vbos[i] );
		glBufferData( GL_ARRAY_BUFFER, sizeof(SolidVertex)*face.vertexCount, face.vertices, GL_STATIC_DRAW );

		glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, sizeof(SolidVertex), 0 );
		glVertexAttribPointer( 1, 2, GL_FLOAT, GL_FALSE, sizeof(SolidVertex), (void*)(sizeof(glm::vec3)) );

		delete[] face.vertices;
		face.vertices = NULL;
	}

	glBindVertexArray( 0 );
}

GLuint Solid::getVAO( int faceIndex ) const
{
	return vaos[faceIndex];
}

int Solid::getVertexCount( int faceIndex ) const
{
	return faces[faceIndex].vertexCount;
}

int Solid::getTextureIndex( int faceIndex ) const
{
	return faces[faceIndex].textureIndex;
}

const Physics::Plane* Solid::getPlanes() const
{
	return planes;
}

int Solid::getPlaneCount() const
{
	return planeCount;
}

int Solid::getFaceCount() const
{
	return faceCount;
}