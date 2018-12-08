#include "entity.h"

CoreData* Entity::coreData = NULL;
uint64_t Entity::entities = 0;

Entity::Entity()
{
	entityID = entities++;
}

Entity::~Entity()
{
}

void Entity::setCoreData( CoreData* cd )
{
	coreData = cd;
}