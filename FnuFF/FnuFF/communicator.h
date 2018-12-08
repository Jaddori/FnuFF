#pragma once

#include "network_message.h"

namespace Network
{
	enum
	{
		CHANNEL_NONE = 0,
		CHANNEL_RELIABLE,
	};

	class Communicator
	{
	public:
		Communicator();
		~Communicator();

		void receive( char* data, int len );

	private:
	};
}