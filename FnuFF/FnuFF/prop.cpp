#include "prop.h"

Prop::Prop()
	: meshIndex( -1 )
{
}

Prop::~Prop()
{
}

bool Prop::load( const char* mesh )
{
	meshIndex = coreData->assets->loadMesh( mesh );

	return ( meshIndex >= 0 );
}

void Prop::render()
{
	coreData->graphics->queueMesh( meshIndex, &transform );
}

Transform& Prop::getTransform()
{
	return transform;
}