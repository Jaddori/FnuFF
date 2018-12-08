#pragma once

#include <stdint.h>
#include <algorithm>
#include <cassert>

#define MESSAGE_SIZE 512

namespace Network
{
	class Message
	{
	public:
		Message();
		Message( char* buffer, int length );
		Message( const Message& ref );
		~Message();

		virtual void clear();

		template<typename T>
		void write( T value )
		{
			assert( offset + sizeof(T) < MESSAGE_SIZE );

			*(T*)(buffer+offset) = value;
			offset += sizeof(T);
			size = offset;
		}
		
		template<typename T>
		void write( T* value, int count )
		{
			write<int32_t>( count );

			assert( offset + sizeof(T)*count < MESSAGE_SIZE );

			memcpy( buffer+offset, value, count*sizeof(T) );
			offset += sizeof(T)*count;
			size = offset;
		}

		template<typename T>
		T read()
		{
			assert( offset + sizeof(T) <= size );

			T result = *(T*)(buffer+offset);
			offset += sizeof(T);

			return result;
		}

		template<typename T>
		int read( T* destination, int maxCount )
		{
			int count = read<int32_t>();

			assert( offset + sizeof(T)*count <= size );
			assert( count <= maxCount );

			memcpy( destination, buffer+offset, count*sizeof(T) );
			offset += sizeof(T)*count;

			return count;
		}

		void setSize( int size );
		void setOffset( int offset );
		void setBuffer( char* buffer, int length );
		void setHash( uint64_t hash );

		int getSize() const;
		int getOffset() const;
		char* getBuffer();
		uint64_t getHash() const;

	private:
		uint64_t hash;
		int size;
		int offset;
		char buffer[MESSAGE_SIZE];
	};
}