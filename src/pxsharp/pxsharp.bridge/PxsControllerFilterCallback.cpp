#include "Common.h"

PxsControllerFilterCallback::PxsControllerFilterCallback(PxsControllerFilterDelegate callback) : mDelegate(callback) { 
}

PxsControllerFilterCallback::~PxsControllerFilterCallback() {
	mDelegate = NULL;
}

bool PxsControllerFilterCallback::filter(const PxController& a, const PxController& b) {
	return mDelegate(&a, &b);
}

PXS_DEFINE_CTOR1(PxsControllerFilterCallback, PxsControllerFilterDelegate);
PXS_DEFINE_DTOR(PxsControllerFilterCallback);
