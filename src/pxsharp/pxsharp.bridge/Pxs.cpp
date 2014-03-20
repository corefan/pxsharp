#include "Common.h"

EXPORT_API PxsScratchBlock Pxs_AllocScratchBlock(PxU32 size) {
	PxsScratchBlock block;
	block.ptr = _aligned_malloc(size, 16);
	block.size = size;
	return block;
}

EXPORT_API void Pxs_ReleaseScratchBlock(PxsScratchBlock block) {
	_aligned_free(block.ptr);
}