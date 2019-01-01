#include "solid.h"
#include "vertex.h"
using namespace Physics;

Solid::Solid()
{
}

Solid::~Solid()
{
}

void Solid::read( FILE* file, void* transientMemory )
{
	planeCount = 0;
	fread( &planeCount, sizeof(planeCount), 1, file );

	planes = new Plane[planeCount];

	for( int i=0; i<planeCount; i++ )
	{
		fread( &planes[i], sizeof(planes[i]), 1, file );
	}

	glm::vec3 x( 1.0f, 0.0f, 0.0f );
	glm::vec3 y( 0.0f, 1.0f, 0.0f );
	glm::vec3 z( 0.0f, 0.0f, 1.0f );

	vertexCount = 0;
	int offset = 0;
	glm::vec3 color( (float)rand() / (float)RAND_MAX, (float)rand() / (float)RAND_MAX, (float)rand() / (float)RAND_MAX );
	color = glm::normalize( color );

	for( int i=0; i<planeCount; i++ )
	{
		uint32_t faceVertexCount = 0;
		fread( &faceVertexCount, sizeof(faceVertexCount), 1, file );

		//fread( (void*)((char*)transientMemory + offset), sizeof(glm::vec3), faceVertexCount, file );
		//offset += sizeof(glm::vec3) * faceVertexCount;
		//vertexCount += faceVertexCount;

		glm::vec3 faceColor( color.r, color.g, color.b );
		if( glm::dot( planes[i].normal, x ) < 0 ||
			glm::dot( planes[i].normal, y ) < 0 ||
			glm::dot( planes[i].normal, z ) < 0 )
		{
			faceColor.r -= 0.25f;
			faceColor.g -= 0.25f;
			faceColor.b -= 0.25f;
		}

		for( int j=0; j<faceVertexCount; j++ )
		{
			fread( (void*)((char*)transientMemory + offset), sizeof(glm::vec3), 1, file );
			offset += sizeof(glm::vec3);

			//fread( (void*)((char*)transientMemory + offset), sizeof(glm::vec2), 1, file );
			glm::vec2 uv;
			fread( &uv, sizeof(uv), 1, file );
			memcpy( (void*)((char*)transientMemory + offset), &uv, sizeof(uv));
			offset += sizeof(glm::vec2);

			/*memcpy( (void*)((char*)transientMemory + offset), &planes[i].normal, sizeof(glm::vec3) );
			offset += sizeof(glm::vec3);

			memcpy( (void*)((char*)transientMemory + offset), &faceColor, sizeof(glm::vec3) );
			offset += sizeof(glm::vec3);*/
		}

		vertexCount += faceVertexCount;
	}

	//vertices = new glm::vec3[vertexCount*2];
	vertices = new SolidVertex[vertexCount];
	memcpy( vertices, transientMemory, offset );
}

void Solid::upload()
{
	glGenVertexArrays( 1, &vao );
	glBindVertexArray( vao );

	glEnableVertexAttribArray( 0 );
	glEnableVertexAttribArray( 1 );
	//glEnableVertexAttribArray( 2 );

	glGenBuffers( 1, &vbo );
	glBindBuffer( GL_ARRAY_BUFFER, vbo );
	glBufferData( GL_ARRAY_BUFFER, sizeof(SolidVertex)*vertexCount, vertices, GL_STATIC_DRAW );

	glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, sizeof(SolidVertex), 0 );
	glVertexAttribPointer( 1, 2, GL_FLOAT, GL_FALSE, sizeof(SolidVertex), (void*)(sizeof(glm::vec3)) );
	//glVertexAttribPointer( 2, 3, GL_FLOAT, GL_FALSE, sizeof(SolidVertex), (void*)(sizeof(glm::vec3)*2) );

	glBindVertexArray( 0 );

	delete[] vertices;
}

GLuint Solid::getVAO() const
{
	return vao;
}

int Solid::getVertexCount() const
{
	return vertexCount;
}

const Physics::Plane* Solid::getPlanes() const
{
	return planes;
}

int Solid::getPlaneCount() const
{
	return planeCount;
}