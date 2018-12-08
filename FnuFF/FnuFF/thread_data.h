#pragma once

#include "common.h"
#include "core_data.h"

struct ThreadData
{
	CoreData* coreData;
	SDL_semaphore* updateDone;
	SDL_semaphore* renderDone;
};