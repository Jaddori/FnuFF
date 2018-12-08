#pragma once

#include "swap.h"
#include "array.h"

template<typename T>
class SwapArray : public Swap<Array<T>>
{
public:
	void swap() override
	{
		Swap<Array<T>>::data[SWAP_READ].fastCopy( Swap<Array<T>>::data[SWAP_WRITE] );
	}
};
