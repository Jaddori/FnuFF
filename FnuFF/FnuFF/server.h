#pragma once

#include "common.h"
#include "message.h"

#define SERVER_TICK_RATE 20
#define SERVER_TICK_TIME ( 1000.0f / SERVER_TICK_RATE )
#define SERVER_DEFAULT_PORT 12345
//#define SERVER_HANDSHAKE_TIMEOUT_MS 1000
//#define SERVER_HANDSHAKE_MAX_RETRIES 3

namespace Network
{
	class Server
	{
	public:
		Server();
		~Server();

		void start( int port = SERVER_DEFAULT_PORT );
		void stop();

		void processTick();

		template<typename T>
		void queue( uint32_t hash, T value )
		{
			int index = addressHashes.find( hash );
			if( index >= 0 )
				sendMessages[index].write( value );
		}

		template<typename T>
		void queue( uint32_t hash, T* value, int maxCount )
		{
			int index = addressHashes.find( hash );
			if( index >= 0 )
				sendMessages[index].write( value, maxCount );
		}

		Array<Message>& getMessages();

		bool getValid() const;
		//uint32_t getNetworkID( uint32_t hash );

	private:
		SOCKET mainSocket;
		WSADATA wsaData;
		struct sockaddr_in localAddress;
		int remoteAddressSize;

		bool hasSocket, valid;
		char buffer[MESSAGE_SIZE];

		Array<Message> sendMessages;
		Array<Message> recvMessages;
		Array<struct sockaddr_in> remoteAddresses;
		Array<uint32_t> addressHashes;
		//Array<uint32_t> salts;
		//Array<uint32_t> networkIDs;

		//Array<struct sockaddr_in> handshakingAddresses;
		//Array<uint32_t> handshakingHashes;
		//Array<int> handshakingTicks;
		//Array<uint32_t> handshakingRetries;
		//Array<uint32_t> handshakingPhases;
		//Array<uint32_t> handshakingSalts;
		//Array<uint32_t> handshakingNetworkIDs;
		//int handshakes;
	};
}