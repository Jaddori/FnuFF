#pragma once

#include <fstream>
#include "array.h"
#include <cassert>
#include "SDL\SDL.h"

#define LOG_START( file ) Log::instance().start( file )
#define LOG_STOP() Log::instance().stop()
#define LOG_FINALIZE() Log::instance().finalize()

#if _WIN32
#define LOG( verbosity, fmt, ... ) \
	{ \
		char buffer[1024] = {}; \
		snprintf( buffer, 1024, __FUNCTION__ " - " fmt, ## __VA_ARGS__ ); \
		Log::instance().addMessage( verbosity, buffer ); \
	}
#else
#define LOG( verbosity, fmt, ... ) \
	{ \
		char buffer[1024] = {}; \
		snprintf( buffer, 1024, "%s - " fmt, __func__, ## __VA_ARGS__ ); \
		Log::instance().addMessage( verbosity, buffer ); \
	}
#endif
#define LOG_ASSERT( condition, fmt, ... ) \
	{ \
		if( !(condition) ) \
			LOG( VERBOSITY_ERROR, fmt, ## __VA_ARGS__ ); \
		assert( (condition) ); \
	}
#define LOG_THRESHOLD( threshold ) Log::instance().setThreshold( threshold )
#define LOG_INFORMATIONS() LOG_THRESHOLD( VERBOSITY_INFORMATION )
#define LOG_WARNINGS() LOG_THRESHOLD( VERBOSITY_WARNING )
#define LOG_ERRORS() LOG_THRESHOLD( VERBOSITY_ERROR )

#define LOG_INFO( fmt, ... ) LOG( VERBOSITY_INFORMATION, fmt, ## __VA_ARGS__ )
#define LOG_WARNING( fmt, ... ) LOG( VERBOSITY_WARNING, fmt, ## __VA_ARGS__ )
#define LOG_ERROR( fmt, ... ) LOG( VERBOSITY_ERROR, fmt, ## __VA_ARGS__ )
#define LOG_DEBUG( fmt, ... ) LOG( VERBOSITY_DEBUG, fmt, ## __VA_ARGS__ )

enum
{
	VERBOSITY_INFORMATION,
	VERBOSITY_WARNING,
	VERBOSITY_ERROR,
	VERBOSITY_DEBUG
};

struct LogMessage
{
	char message[1024];
	int verbosity;
};

class Log
{
public:
	static Log& instance()
	{
		static Log log;
		return log;
	}
	~Log(){}

	bool start( const char* logfile );
	void stop();

	void addMessage( int verbosity, const char* message );
	void copyMessages( Array<LogMessage>& destination );

	void setThreshold( int threshold );

	const Array<LogMessage>& getMessages() const;
	int getThreshold() const;

private:
	Log() : file( nullptr ), threshold( VERBOSITY_WARNING ) {}

	FILE* file;
	Array<LogMessage> messages;
	int threshold;

	SDL_mutex* mutex;
};
