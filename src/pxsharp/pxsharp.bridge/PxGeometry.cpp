#include "Common.h"

PXS_DEFINE_CTOR1(PxSphereGeometry, PxReal);
PXS_DEFINE_DTOR(PxSphereGeometry);

PXS_DEFINE_CTOR3(PxBoxGeometry, PxReal, PxReal, PxReal);
PXS_DEFINE_DTOR(PxBoxGeometry);

PXS_DEFINE_CTOR2(PxCapsuleGeometry, PxReal, PxReal);
PXS_DEFINE_DTOR(PxCapsuleGeometry);

/*
EXPORT_API void PxGeometry_Delete(PxGeometry* self) {
	switch (self->getType()) {
	case PxGeometryType::eSPHERE:
		PxSphereGeometry_Delete(static_cast<PxSphereGeometry*>(self));
		break;

	case PxGeometryType::eBOX:
		PxBoxGeometry_Delete(static_cast<PxBoxGeometry*>(self));
		break;

	case PxGeometryType::eCAPSULE:
		PxCapsuleGeometry_Delete(static_cast<PxCapsuleGeometry*>(self));
		break;
	}
}
*/