#pragma once

#include "message.h"

namespace Network
{
	class NetworkMessage : public Message
	{
	public:
		NetworkMessage( uint64_t id, uint64_t ack, uint32_t ackflags );
		NetworkMessage( char* buffer, int length );
		~NetworkMessage();

		virtual void clear() override;

	private:
		uint64_t id;
		uint64_t ack;
		uint32_t ackflags;
	};
}