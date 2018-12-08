#pragma once

//#include <Windows.h>
//#include <Psapi.h>
//#include "SDL\SDL.h"
//#include "thread_pool.h"
#include "common.h"
#include "thread_pool.h"

class SystemInfo
{
public:
	SystemInfo();
	~SystemInfo();

	void poll();

	void startUpdate();
	void startRender();

	void stopUpdate();
	void stopRender();

	int getCores() const;
	int getThreads() const;
	int getRam() const;
	bool getVsync() const;
	int getUpdateMs() const;
	int getRenderMs() const;
	float getDeltaTime() const;

private:
	int cores;
	int threads;
	int ram;
	bool vsync;

	int updateStartTick;
	int renderStartTick;

	int updateMs;
	int renderMs;
	float deltaTime;
};
