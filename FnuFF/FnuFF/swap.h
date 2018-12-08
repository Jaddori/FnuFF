#pragma once

#define SWAP_WRITE 0
#define SWAP_READ 1

template<typename T>
class Swap
{
public:
	Swap() {}
	Swap( const Swap& ref )
	{
		data[SWAP_WRITE] = ref.data[SWAP_WRITE];
		data[SWAP_READ] = ref.data[SWAP_READ];
	}
	Swap( const T& ref )
	{
		data[SWAP_WRITE] = ref;
	}
	virtual ~Swap(){}

	operator const T&() const
	{
		return data[SWAP_READ];
	}

	Swap& operator=( const Swap& ref )
	{
		data[SWAP_WRITE] = ref.data[SWAP_WRITE];
		data[SWAP_READ] = ref.data[SWAP_READ];
		return *this;
	}

	Swap& operator=( const T& ref )
	{
		data[SWAP_WRITE] = ref;
		return *this;
	}

	void operator+=( const T& ref )
	{
		data[SWAP_WRITE] += ref;
	}

	void operator-=( const T& ref )
	{
		data[SWAP_WRITE] -= ref;
	}

	void operator*=( const T& ref )
	{
		data[SWAP_WRITE] *= ref;
	}

	void operator/=( const T& ref )
	{
		data[SWAP_WRITE] /= ref;
	}

	T& operator[]( int index )
	{
		assert( index >= 0 && index < 2 );
		return data[index];
	}

	virtual void swap()
	{
		data[SWAP_READ] = data[SWAP_WRITE];
	}

	virtual T& getRead()
	{
		return data[SWAP_READ];
	}

	virtual const T& getRead() const
	{
		return data[SWAP_READ];
	}

	virtual T& getWrite()
	{
		return data[SWAP_WRITE];
	}

	virtual const T& getWrite() const
	{
		return data[SWAP_WRITE];
	}

protected:
	T data[2];
};