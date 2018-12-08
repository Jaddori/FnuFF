#include "server.h"
using namespace Network;

Server::Server()
	//: handshakes( 1 )
{
}

Server::~Server()
{
}

void Server::start( int port )
{
	valid = false;

	if( WSAStartup( MAKEWORD(2,2), &wsaData ) == 0 )
	{
		mainSocket = socket( AF_INET, SOCK_DGRAM, IPPROTO_UDP );
		if( mainSocket != INVALID_SOCKET )
		{
			hasSocket = true;

			localAddress.sin_family = AF_INET;
			localAddress.sin_addr.s_addr = INADDR_ANY;
			localAddress.sin_port = htons( port );

			if( bind( mainSocket, (struct sockaddr*)&localAddress, sizeof(localAddress) ) != SOCKET_ERROR )
			{
				DWORD nonBlocking = 1;
				if( ioctlsocket( mainSocket, FIONBIO, &nonBlocking ) == 0 )
				{
					remoteAddressSize = sizeof(struct sockaddr_in);
					valid = true;
				}
				else
				{
					LOG_ERROR( "Server: Failed to set socket to non-blocking." );
				}
			}
			else
			{
				LOG_ERROR( "Server: Failed to bind socket." );
			}
		}
		else
		{
			LOG_ERROR( "Server: Failed to create socket." );
		}
	}
	else
	{
		LOG_ERROR( "Server: Failed to startup WSA." );
	}
}

void Server::stop()
{
	if( hasSocket )
		closesocket( mainSocket );
	WSACleanup();
}

void Server::processTick()
{
	if( !valid )
		return;

	// RECEIVE
	recvMessages.clear();

	int recvLen = 0;
	do
	{
		memset( buffer, 0, MESSAGE_SIZE );

		struct sockaddr_in remoteAddress;
		recvLen = recvfrom( mainSocket, buffer, MESSAGE_SIZE, 0, (struct sockaddr*)&remoteAddress, &remoteAddressSize );

		if( recvLen > 0 )
		{
			uint32_t hash = remoteAddress.sin_addr.S_un.S_addr + remoteAddress.sin_port;
			if( addressHashes.find( hash ) < 0 )
			{
				addressHashes.add( hash );
				remoteAddresses.add( remoteAddress );
				
				Message& msg = sendMessages.append();
				msg.clear();
				
				LOG_DEBUG( "Adding new remote address with hash: %d", hash );
			}

			Message msg( buffer, recvLen );
			msg.setHash( hash  );
			recvMessages.add( msg );

			/*if( addressHashes.find( hash ) < 0 )
			{
				int index = handshakingHashes.find( hash );
				if( index < 0 )
				{
					handshakingHashes.add( hash );
					handshakingAddresses.add( remoteAddress );
					handshakingTicks.add( 0 );
					handshakingPhases.add( 0 );
					handshakingSalts.add( 0 );
					handshakingRetries.add( 0 );
					handshakingNetworkIDs.add( 0 );

					index = handshakingHashes.getSize()-1;

					LOG_INFO( "Server: Starting handshake with new hash: %d", hash );
				}

				Message msg( buffer, recvLen );

				const int LOCAL_PHASE = handshakingPhases[index];
				switch( LOCAL_PHASE )
				{
					case 0: //
					{
						uint32_t phase = msg.read<uint32_t>();
						if( phase == LOCAL_PHASE )
						{
							LOG_INFO( "Server: Received handshake message #%d", LOCAL_PHASE );

							uint32_t salt = msg.read<uint32_t>();
							handshakingSalts[index] = salt;

							handshakingPhases[index]++;
						}
					} break;

					case 1: //
					{
						uint32_t phase = msg.read<uint32_t>();
						if( phase == LOCAL_PHASE )
						{
							LOG_INFO( "Server: Received handshake message #%d", LOCAL_PHASE );

							uint32_t fullSalt = msg.read<uint32_t>();
							uint32_t checkSalt = hash ^ handshakingSalts[index];

							if( fullSalt == checkSalt )
							{
								addressHashes.add( hash );
								remoteAddresses.add( remoteAddress );
								salts.add( fullSalt );
								networkIDs.add( handshakingNetworkIDs[index] );

								Message& msg = sendMessages.append();
								msg.clear();

								LOG_INFO( "Server: Handshaking complete for hash: %d", handshakingHashes[index] );

								// remove from handshaking lists
								handshakingAddresses.removeAt( index );
								handshakingHashes.removeAt( index );
								handshakingTicks.removeAt( index );
								handshakingPhases.removeAt( index );
								handshakingSalts.removeAt( index );
								handshakingRetries.removeAt( index );
								handshakingNetworkIDs.removeAt( index );
							}
						}
					} break;
				}
			}
			else
			{
				Message msg( buffer, recvLen );
				msg.setHash( hash );
				recvMessages.add( msg );
			}*/
		}
	} while( recvLen > 0 );

	// SEND
	const int REMOTE_ADDRESS_COUNT = remoteAddresses.getSize();
	for( int curAddress = 0; curAddress < REMOTE_ADDRESS_COUNT; curAddress++ )
	{
		Message& message = sendMessages[curAddress];
		if( message.getSize() > 0 )
		{
			int sendLen = sendto( mainSocket, message.getBuffer(), message.getSize(), 0, (addr)&remoteAddresses[curAddress], remoteAddressSize );

			if( sendLen == SOCKET_ERROR )
			{
				LOG_ERROR( "Server: sendto failed with error code: %d", WSAGetLastError() );
				valid = false;
			}

			message.clear();
		}
	}

	// handshaking
	/*const int HANDSHAKING_ADDRESSES = handshakingAddresses.getSize();
	for( int curAddress = 0; curAddress < HANDSHAKING_ADDRESSES; curAddress++ )
	{
		const int LOCAL_PHASE = handshakingPhases[curAddress];
		switch( LOCAL_PHASE )
		{
			case 1:
			{
				bool shouldSend = false;

				handshakingTicks[curAddress] -= TIMESTEP_MS;
				if( handshakingTicks[curAddress] <= 0 )
				{
					if( handshakingRetries[curAddress] >= SERVER_HANDSHAKE_MAX_RETRIES )
					{
						shouldSend = false;
						LOG_WARNING( "Server: Client timed out when trying to connect." );
					}

					shouldSend = true;
				}

				if( shouldSend )
				{
					uint32_t fullSalt = handshakingHashes[curAddress] ^ handshakingSalts[curAddress];

					Message msg;
					msg.write<uint32_t>( LOCAL_PHASE );
					msg.write( fullSalt );
					msg.write( handshakes );
					handshakingNetworkIDs[curAddress] = handshakes;

					handshakes++;

					LOG_INFO( "Server: Sending handshake message #%d", LOCAL_PHASE );
					
					int sendLen = sendto( mainSocket, msg.getBuffer(), msg.getSize(), 0, (addr)&handshakingAddresses[curAddress], remoteAddressSize );
					if( sendLen == SOCKET_ERROR )
					{
						LOG_ERROR( "Server: sendto failed with error code: %d", WSAGetLastError() );
						valid = false;
					}

					handshakingTicks[curAddress] = SERVER_HANDSHAKE_TIMEOUT_MS;
				}
			} break;
		}
	}*/
}

Array<Message>& Server::getMessages()
{
	return recvMessages;
}

bool Server::getValid() const
{
	return valid;
}

/*uint32_t Server::getNetworkID( uint32_t hash )
{
	uint32_t result = 0;

	int index = addressHashes.find( hash );
	if( index >= 0 )
	{
		result = networkIDs[index];
	}

	return result;
}*/