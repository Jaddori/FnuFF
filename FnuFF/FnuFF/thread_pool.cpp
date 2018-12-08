#include "thread_pool.h"

ThreadPool::ThreadPool()
	: curTask( 0 )
{
	LOG_INFO( "Constructing." );
}

ThreadPool::~ThreadPool()
{
	LOG_INFO( "Destructing." );
}

void ThreadPool::load()
{
	LOG_INFO( "Creating threads." );

	for( int i=0; i<THREAD_POOL_MAX_THREADS; i++ )
	{
		data[i].id = i;
		data[i].alive = true;
		data[i].done = true;
		data[i].signal = SDL_CreateSemaphore( 0 );
		threads[i] = SDL_CreateThread( threadWork, nullptr, &data[i] );
	}
}

void ThreadPool::unload()
{
	LOG_INFO( "Waiting for threads to finish." );

	for( int i=0; i<THREAD_POOL_MAX_THREADS; i++ )
		data[i].alive = false;

	for( int i=0; i<THREAD_POOL_MAX_THREADS; i++ )
	{
		SDL_WaitThread( threads[i], nullptr );
	}
}

void ThreadPool::queueWork( const Job& job)
{
	LOG_ASSERT( job.task != nullptr, "Job task is nullptr." );

	jobs.enqueue( job );
}

void ThreadPool::schedule()
{
	for( int i=0; i<THREAD_POOL_MAX_THREADS && jobs.getSize() > 0; i++ )
	{
		if( data[i].done )
		{
			data[i].done = false;
			data[i].job = jobs.dequeue();

			SDL_SemPost( data[i].signal );
		}
	}
}

int ThreadPool::threadWork( void* args )
{
	ThreadData* data = (ThreadData*)args;

	LOG_INFO( "Thread #%d entering task loop.", data->id );

	while( data->alive )
	{
		// wait for something to do
		int waitResult = SDL_SemWaitTimeout( data->signal, THREAD_POOL_TIMEOUT );
		if( waitResult == 0 )
		{
			data->job.task( data->job.args );

			data->done = true;
		}
		else if( waitResult == -1 )
		{
			LOG_ERROR( "Error encountered when waiting on semaphore on thread #%d.", data->id );
			data->alive = false;
		}
	}

	LOG_INFO( "Thread #%d exiting task loop.", data->id );

	return 0;
}
