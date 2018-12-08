#pragma once

#include "common.h"
#include "message.h"

#define CLIENT_TICK_RATE 20
#define CLIENT_TICK_TIME ( 1000.0f / CLIENT_TICK_RATE )
//#define CLIENT_DEFAULT_PORT 12345
//#define CLIENT_HANDSHAKE_TIMEOUT_MS 1000
//#define CLIENT_HANDSHAKE_MAX_RETRIES 3

namespace Network
{
	class Client
	{
	public:
		Client();
		~Client();

		void start();
		void stop();

		//void processHandshake();
		void processTick();
		void setConnection( const char* ip, int port );

		template<typename T>
		void queue( T value )
		{
			SDL_LockMutex( mutex );
			sendMessage.write( value );
			SDL_UnlockMutex( mutex );
		}

		template<typename T>
		void queue( T* value, int maxCount )
		{
			SDL_LockMutex( mutex );
			sendMessage.write( value, maxCount );
			SDL_UnlockMutex( mutex );
		}

		Array<Message>& getMessages();

		bool getValid() const;

	private:
		SOCKET mainSocket;
		WSADATA wsaData;
		struct sockaddr_in remoteAddress;
		int remoteAddressSize;

		char buffer[MESSAGE_SIZE];
		bool hasSocket, valid;
		SDL_mutex* mutex;
		Message sendMessage;
		Array<Message> recvMessages;
		
		bool hasConnection;
	};
}