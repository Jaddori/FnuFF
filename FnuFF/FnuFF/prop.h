#pragma once

#include "entity.h"

class Prop : public Entity
{
public:
	Prop();
	~Prop();

	bool load( const char* mesh );
	void render();

	Transform& getTransform();
	
private:
	int meshIndex;
	Transform transform;
};