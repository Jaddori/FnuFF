#include "model.h"
using namespace Rendering;

Model::Model()
	: mesh( NULL ), texture( NULL ),
	vertexArray( 0 ), vertexBuffer( 0 ), indexBuffer( 0 )
{
}

Model::~Model()
{
}

void Model::load( const Mesh* m, const Texture* t )
{
	mesh = m;
	texture = t;
}

void Model::unload()
{
	mesh = NULL;
	texture = NULL;
}

void Model::render()
{
	texture->bind();
	mesh->bind();
	mesh->render();
}

const Mesh* Model::getMesh() const
{
	return mesh;
}

const Texture* Model::getTexture() const
{
	return texture;
}