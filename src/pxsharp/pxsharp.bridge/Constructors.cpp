#include "Common.h"

EXPORT_API PxPlane* PxPlane_New0() {
	return new PxPlane();
}

EXPORT_API PxPlane* PxPlane_New1(PxVec3_Managed normal, PxReal distance) {
	return new PxPlane(PxVec3_IN(normal), distance);
}

EXPORT_API void PxPlane_Delete(PxPlane* self) {
	delete self;
}