#include "communicator.h"
using namespace Network;

Communicator::Communicator()
{
}

Communicator::~Communicator()
{
}

void Communicator::receive( char* data, int len )
{
	NetworkMessage msg( data, len );

	int channel = msg.read<int32_t>();
	if( channel == CHANNEL_RELIABLE )
	{

	}
}