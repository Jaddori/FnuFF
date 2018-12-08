#include "instanced_model.h"
using namespace Rendering;

InstancedModel::InstancedModel()
	: dirtyMatrices( true ), dirtyBuffer( false )
{
}

InstancedModel::~InstancedModel()
{
}

void InstancedModel::load( Model* m )
{
	model = m;

	glGenBuffers( 1, &uniformBuffer );
}

void InstancedModel::finalize()
{
	if( dirtyMatrices )
	{
		for( int i = 0; i < instances; i++ )
		{
			worldMatrices[i] = glm::scale( glm::translate( IDENT, positions[i] ) * glm::toMat4( orientations[i] ), scales[i] );
		}

		dirtyMatrices = false;
		dirtyBuffer = true;
	}
}

void InstancedModel::render()
{
	if( dirtyBuffer )
	{
		glBindBuffer( GL_UNIFORM_BUFFER, uniformBuffer );
		glBufferData( GL_UNIFORM_BUFFER, sizeof(glm::mat4)*worldMatrices.getSize(), &worldMatrices.getConstData()[0][0], GL_DYNAMIC_DRAW );
		glBindBufferRange( GL_UNIFORM_BUFFER, 0, uniformBuffer, 0, sizeof(glm::mat4)*worldMatrices.getSize() );
		glBindBuffer( GL_UNIFORM_BUFFER, 0 );

		dirtyBuffer = false;
	}

	glBindBuffer( GL_UNIFORM_BUFFER, uniformBuffer );

	model->getTexture()->bind();
	model->getMesh()->bind();

	//glDrawElementsInstanced( GL_TRIANGLES, model->getMesh()->getIndexCount(), GL_UNSIGNED_INT, NULL, worldMatrices.getSize() );

	glBindBuffer( GL_UNIFORM_BUFFER, 0 );
}

int InstancedModel::addInstance()
{
	int index = instances++;

	positions.append();
	orientations.append();
	scales.append() = glm::vec3( 1.0f, 1.0f, 1.0f );
	worldMatrices.append();

	return index;
}

void InstancedModel::setPosition( int index, const glm::vec3& position )
{
	positions[index] = position;
	dirtyMatrices = true;
}

void InstancedModel::setOrientation( int index, const glm::quat& orientation )
{
	orientations[index] = orientation;
	dirtyMatrices = true;
}

void InstancedModel::setScale( int index, const glm::vec3& scale )
{
	scales[index] = scale;
	dirtyMatrices = true;
}