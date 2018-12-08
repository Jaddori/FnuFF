#pragma once

#include "common.h"
#include "input.h"
#include "system_info.h"
#include "camera.h"
#include "assets.h"
#include "graphics.h"
#include "shapes.h"
#include "collision_solver.h"
#include "client.h"
#include "server.h"

#define CORE_DATA_TRANSIENT_MEMORY_SIZE 1024*1024*10 // 10Mb

struct CoreData
{
	System::Input* input;
	SystemInfo* systemInfo;
	bool* running, *reload;
	char* transientMemory;
	Rendering::Assets* assets;
	Rendering::Graphics* graphics;
	Rendering::DebugShapes* debugShapes;
	Physics::CollisionSolver* collisionSolver;
	Network::Client* client;
	Network::Server* server;
	uint64_t* updateAccumulator;
};