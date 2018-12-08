#pragma once

#include <cassert>

#define QUEUE_DEFAULT_CAPACITY 10

template<typename T>
class Queue
{
public:
	Queue()
		: first( -1 ), last( -1 ), capacity( QUEUE_DEFAULT_CAPACITY )
	{
		data = new T[capacity];
	}
	Queue( int initialCapacity )
		: first( -1 ), last( -1 ), capacity( initialCapacity )
	{
		data = new T[capacity];
	}
	~Queue()
	{
		delete[] data;
	}

	void enqueue( const T& item )
	{
		/*if( size >= capacity )
		expand( capacity * 2 );*/

		last++;

		if( last >= capacity )
			last = 0;

		if( getSize() >= capacity )
			expand( capacity * 2 );

		data[last] = item;

		if( first < 0 )
			first = 0;
	}

	T dequeue()
	{
		assert( getSize() > 0 && first >= 0 );

		T& item = data[first];
		first++;

		if( first == last+1 )
			first = last = -1;
		if( first >= capacity )
			first = 0;

		return item;
	}

	T& front()
	{
		return data[first];
	}

	T& back()
	{
		return data[last];
	}

	inline int getSize()
	{
		if( first < 0 )
			return 0;
		else if( first < last )
			return last-first;
		return ( capacity - first + last );
	}

private:
	void expand( int newCapacity )
	{
		T* temp = new T[newCapacity];

		for( int i=0; i<capacity; i++ )
			temp[i] = data[i];

		delete[] data;
		data = temp;

		first = 0;
		last = capacity;
		capacity = newCapacity;
	}

	T* data;
	int capacity;

	int first, last;
};
