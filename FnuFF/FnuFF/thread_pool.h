#pragma once

#include "common.h"

#define THREAD_POOL_MAX_THREADS 2
#define THREAD_POOL_TIMEOUT 100

typedef void (Task)( void* args );
struct Job
{
	Task* task;
	void* args;
};

class ThreadPool
{
public:
	ThreadPool();
	~ThreadPool();

	void load();
	void unload();

	void queueWork( const Job& job );
	void schedule();

private:
	struct ThreadData
	{
		int id;
		bool alive, done;
		SDL_semaphore* signal;
		Job job;
	};

	static int threadWork( void* args );

	SDL_Thread* threads[THREAD_POOL_MAX_THREADS];
	ThreadData data[THREAD_POOL_MAX_THREADS];

	Queue<Job> jobs;
	int curTask;
};
