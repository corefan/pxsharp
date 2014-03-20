#include "Common.h"

PxsErrorCallback::PxsErrorCallback(PxsErrorDelegate errorDelegate) : mErrorDelegate(errorDelegate) { 
}

void PxsErrorCallback::reportError(PxErrorCode::Enum code, const char* message, const char* file, int line) {
	mErrorDelegate(code, message);
}

PXS_DEFINE_CTOR1(PxsErrorCallback, PxsErrorDelegate);
PXS_DEFINE_DTOR(PxsErrorCallback);