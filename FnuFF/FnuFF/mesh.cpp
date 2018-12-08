#include "mesh.h"
using namespace Rendering;

Mesh::Mesh()
	: vertexCount( 0 ),
	vertices( NULL ),
	uploaded( false )
{
}

Mesh::~Mesh()
{
}

bool Mesh::load( const char* path )
{
	bool result = false;

	LOG_INFO( "Loading mesh from file: %s", path );

	FILE* file = fopen( path, "rb" );
	if( file )
	{
		fread( &vertexCount, sizeof(int), 1, file );

		if( vertexCount > 0 )
		{
			vertices = new Vertex[vertexCount];

			fread( vertices, sizeof(Vertex), vertexCount, file );

			fclose( file );

			result = true;

			// calculate bounding box
			glm::vec3 minPosition(9999.0f, 9999.0f, 9999.0f), maxPosition(-9999.0f, -9999.0f, -9999.0f);
			for( int i=0; i<vertexCount; i++ )
			{
				const glm::vec3& position = vertices[i].position;
				if( position.x < minPosition.x )
					minPosition.x = position.x;
				if( position.y < minPosition.y )
					minPosition.y = position.y;
				if( position.z < minPosition.z )
					minPosition.z = position.z;

				if( position.x > maxPosition.x )
					maxPosition.x = position.x;
				if( position.y > maxPosition.y )
					maxPosition.y = position.y;
				if( position.z > maxPosition.z )
					maxPosition.z = position.z;
			}

			boundingBox.minPosition = minPosition;
			boundingBox.maxPosition = maxPosition;

			uploaded = false;
		}
	}
	else
		LOG_ERROR( "Failed to open file: %s", path );

	return result;
}

void Mesh::unload()
{
	vertexCount = 0;

	if( vertices )
	{
		delete[] vertices;
		vertices = NULL;
	}

	if( vertexArray )
		glDeleteVertexArrays( 1, &vertexArray );
	if( vertexBuffer )
		glDeleteBuffers( 1, &vertexBuffer );

	vertexArray = 0;
	vertexBuffer = 0;
}

void Mesh::upload()
{
	if( !uploaded )
	{
		LOG_ASSERT( vertices, "Trying to upload a mesh without valid vertex data." );

		if( vertexArray == 0 )
		{
			glGenVertexArrays( 1, &vertexArray );
			glGenBuffers( 1, &vertexBuffer );
		}

		glBindVertexArray( vertexArray );

		glEnableVertexAttribArray( 0 );
		glEnableVertexAttribArray( 1 );
		glEnableVertexAttribArray( 2 );
		glEnableVertexAttribArray( 3 );
		glEnableVertexAttribArray( 4 );

		glBindBuffer( GL_ARRAY_BUFFER, vertexBuffer );

		glBufferData( GL_ARRAY_BUFFER, sizeof(Vertex)*vertexCount, vertices, GL_STATIC_DRAW );

		glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), 0 );
		glVertexAttribPointer( 1, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)(sizeof(glm::vec3)) );
		glVertexAttribPointer( 2, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)(sizeof(glm::vec3)+sizeof(glm::vec2)));
		glVertexAttribPointer( 3, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)(sizeof(glm::vec3)*2+sizeof(glm::vec2)));
		glVertexAttribPointer( 4, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)(sizeof(glm::vec3)*3+sizeof(glm::vec2)));

		glBindVertexArray( 0 );

		if( vertices )
		{
			delete[] vertices;
			vertices = NULL;
		}

		uploaded = true;
	}
}

void Mesh::bind() const
{
	glBindVertexArray( vertexArray );
}

void Mesh::render() const
{
	//glDrawElements( GL_TRIANGLES, indexCount, GL_UNSIGNED_INT, 0 );
	glDrawArrays( GL_TRIANGLES, 0, vertexCount );
}

int Mesh::getVertexCount() const 
{
	return vertexCount;
}

bool Mesh::getUploaded() const
{
	return uploaded;
}

const Vertex* Mesh::getVertices() const
{
	return vertices;
}

const Physics::AABB* Mesh::getBoundingBox() const
{
	return &boundingBox;
}