#include "Common.h"

EXPORT_API PxTransform_Managed PxShapeExt_GetGlobalPose(PxShape* shape, PxRigidActor* actor) {
	return PxTransform_OUT(PxShapeExt::getGlobalPose(*shape, *actor));
}

EXPORT_API PxBounds3_Managed PxShapeExt_GetWorldBounds(PxShape* shape, PxRigidActor* actor, float inflation) {
	return PxBounds3_OUT(PxShapeExt::getWorldBounds(*shape, *actor, inflation));
}

EXPORT_API bool PxShapeExt_Overlap(PxShape* shape, PxRigidActor* actor, PxGeometry* otherGeometry, PxTransform_Managed otherGeometryPose) {
	return PxShapeExt::overlap(*shape, *actor, *otherGeometry, PxTransform_IN(otherGeometryPose));
}

EXPORT_API bool PxShapeExt_Sweep(PxShape* shape, PxRigidActor* actor, PxVec3_Managed unitDir, PxReal distance, PxGeometry* otherGeometry, PxTransform_Managed otherGeometryPose, PxSweepHit_Managed* hit, PxHitFlag::Enum hitFlags) {
	PxSweepHit _hit;

	if (PxShapeExt::sweep(*shape, *actor, PxVec3_IN(unitDir), distance, *otherGeometry, PxTransform_IN(otherGeometryPose), _hit, hitFlags)) {
		*hit = PxSweepHit_OUT(_hit);
		return true;
	}

	return false;
}

/*
EXPORT_API PxU32 PxShapeExt_Raycast(PxShape* shape, PxRigidActor* actor, PxVec3_Managed rayOrigin, PxVec3_Managed rayDir, PxReal distance, PxHitFlag::Enum hitFlags, PxU32 maxHits) {
	PxU32 hitCount = PxShapeExt::raycast(*shape, *actor, PxVec3_IN(rayOrigin), PxVec3_IN(rayDir), distance, hitFlags, maxHits, )
	return hitCount;
}
*/
