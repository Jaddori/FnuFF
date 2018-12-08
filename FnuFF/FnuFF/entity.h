#pragma once

#include "common.h"
#include "core_data.h"
#include "rendering.h"

class Entity
{
public:
	Entity();
	~Entity();

	static void setCoreData( CoreData* coreData );

protected:
	static CoreData* coreData;
	static uint64_t entities;

	uint64_t entityID;
};