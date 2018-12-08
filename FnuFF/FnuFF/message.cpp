#include "message.h"
using namespace Network;

Message::Message()
	: size( 0 ), offset( 0 ), hash( 0 )
{
}

Message::Message( char* buf, int len )
	: size( len ), offset( 0 ), hash( 0 )
{
	memcpy( buffer, buf, len );
}

Message::Message( const Message& ref )
	: size( ref.size ), offset( ref.offset ), hash( ref.hash )
{
	memcpy( buffer, ref.buffer, size );
}

Message::~Message()
{
}

void Message::clear()
{
	size = 0;
	offset = 0;
}

void Message::setSize( int value )
{
	size = value;
}

void Message::setOffset( int value )
{
	offset = value;
}

void Message::setBuffer( char* buf, int len )
{
	memcpy( buffer, buf, len );
	size = len;
}

void Message::setHash( uint64_t h )
{
	hash = h;
}

int Message::getSize() const
{
	return size;
}

int Message::getOffset() const
{
	return offset;
}

char* Message::getBuffer()
{
	return buffer;
}

uint64_t Message::getHash() const
{
	return hash;
}