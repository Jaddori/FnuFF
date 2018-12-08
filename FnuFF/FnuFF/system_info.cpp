#include "system_info.h"

SystemInfo::SystemInfo()
{
}

SystemInfo::~SystemInfo()
{
}

void SystemInfo::poll()
{
	cores = SDL_GetCPUCount();
	threads = THREAD_POOL_MAX_THREADS + 2;

	// TODO: Fix this
	//vsync = ( SDL_GL_GetSwapInterval() == 1 );
	vsync = false;

	PROCESS_MEMORY_COUNTERS memInfo;
	GetProcessMemoryInfo( GetCurrentProcess(), &memInfo, sizeof(memInfo) );

	ram = (int)memInfo.PagefileUsage;
}

void SystemInfo::startUpdate()
{
	int prevStartTick = updateStartTick;
	updateStartTick = SDL_GetTicks();

	deltaTime = (float)(updateStartTick - prevStartTick) * 0.001f;
}

void SystemInfo::startRender()
{
	renderStartTick = SDL_GetTicks();
}

void SystemInfo::stopUpdate()
{
	updateMs = SDL_GetTicks() - updateStartTick;
}

void SystemInfo::stopRender()
{
	renderMs = SDL_GetTicks() - renderStartTick;
}

int SystemInfo::getRam() const
{
	return ram;
}

int SystemInfo::getCores() const
{
	return cores;
}

int SystemInfo::getThreads() const
{
	return threads;
}

bool SystemInfo::getVsync() const
{
	return vsync;
}

int SystemInfo::getUpdateMs() const
{
	return updateMs;
}

int SystemInfo::getRenderMs() const
{
	return renderMs;
}

float SystemInfo::getDeltaTime() const
{
	return deltaTime;
}
