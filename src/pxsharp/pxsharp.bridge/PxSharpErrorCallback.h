#include "Common.h"

typedef void(_stdcall *PxSharpErrorDelegate) (PxErrorCode::Enum code, const char* message);

class PxSharpErrorCallback : public PxErrorCallback {
private:
	PxSharpErrorDelegate mErrorDelegate;

public:
	PxSharpErrorCallback(PxSharpErrorDelegate errorDelegate) : mErrorDelegate(errorDelegate) { }
	virtual void reportError(PxErrorCode::Enum code, const char* message, const char* file, int line);
};