#include "level.h"
using namespace Physics;

Level::Level()
{
	transform.setActive( true );
}

Level::~Level()
{
}

bool Level::load( const char* filepath )
{
	bool result = false;

	FILE* file = fopen( filepath, "r" );
	if( file )
	{
		fseek( file, 0, SEEK_END );
		int len = ftell( file );
		fseek( file, 0, SEEK_SET );

		fread( coreData->transientMemory, 1, len, file );
		fclose( file );

		coreData->transientMemory[len] = 0;

		meshIndex = coreData->assets->loadMesh( coreData->transientMemory );

		Mesh geometry;
		geometry.load( coreData->transientMemory );

		// process triangles to create collision data
		const int VERTEX_COUNT = geometry.getVertexCount();
		const Vertex* VERTICES = geometry.getVertices();

		triangleCount = VERTEX_COUNT / 3;
		triangles = new Triangle[triangleCount];
		for( int i=0; i<triangleCount; i++)
		{
			triangles[i].v[0] = VERTICES[i*3].position;
			triangles[i].v[1] = VERTICES[i*3+1].position;
			triangles[i].v[2] = VERTICES[i*3+2].position;
		}
	}

	return result;
}

void Level::unload()
{
}

void Level::render()
{
	coreData->graphics->queueMesh( meshIndex, &transform );
}

int Level::raytrace( const Ray& ray, glm::vec3& hitPoint )
{
	int result = -1;
	Hit hit;

	for( int i=0; i<triangleCount && result < 0; i++ )
	{
		if( coreData->collisionSolver->ray( ray, triangles[i], &hit ) )
		{
			printf( "Hit triangle %i\n", i );
			result = i;

			hitPoint = hit.position;
		}
	}

	return result;
}

const Triangle* Level::getTriangle( int index ) const
{
	const Triangle* result = NULL;

	if( index >= 0 && index < triangleCount )
		result = &triangles[index];

	return result;
}