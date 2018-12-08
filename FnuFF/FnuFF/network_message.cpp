#include "network_message.h"
using namespace Network;

NetworkMessage::NetworkMessage( uint64_t _id, uint64_t _ack, uint32_t _ackflags )
	: id( _id ), ack( _ack ), ackflags( _ackflags )
{
	write( id );
	write( ack );
	write( ackflags );
}

NetworkMessage::NetworkMessage( char* buf, int len )
	: Message( buf, len )
{
	id = read<uint64_t>();
	ack = read<uint64_t>();
	ackflags = read<uint64_t>();
}

NetworkMessage::~NetworkMessage()
{
}

void NetworkMessage::clear()
{
	Message::clear();

	id = 0;
	ack = 0;
	ackflags = 0;
}