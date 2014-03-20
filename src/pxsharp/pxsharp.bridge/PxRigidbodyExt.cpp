#include "Common.h"

EXPORT_API void PxRigidbodyExt_AddForceAtLocalPos(PxRigidBody* rigidbody, PxVec3_Managed force, PxVec3_Managed pos, PxForceMode::Enum forceMode, bool wakeup) {
	PxRigidBodyExt::addForceAtLocalPos(*rigidbody, PxVec3_IN(force), PxVec3_IN(pos), forceMode, wakeup);
}

EXPORT_API void PxRigidbodyExt_AddForceAtPos(PxRigidBody* rigidbody, PxVec3_Managed force, PxVec3_Managed pos, PxForceMode::Enum forceMode, bool wakeup) {
	PxRigidBodyExt::addForceAtPos(*rigidbody, PxVec3_IN(force), PxVec3_IN(pos), forceMode, wakeup);
}

EXPORT_API void PxRigidbodyExt_AddLocalForceAtLocalPos(PxRigidBody* rigidbody, PxVec3_Managed force, PxVec3_Managed pos, PxForceMode::Enum forceMode, bool wakeup) {
	PxRigidBodyExt::addLocalForceAtLocalPos(*rigidbody, PxVec3_IN(force), PxVec3_IN(pos), forceMode, wakeup);
}

EXPORT_API void PxRigidbodyExt_AddLocalForceAtPos(PxRigidBody* rigidbody, PxVec3_Managed force, PxVec3_Managed pos, PxForceMode::Enum forceMode, bool wakeup) {
	PxRigidBodyExt::addLocalForceAtPos(*rigidbody, PxVec3_IN(force), PxVec3_IN(pos), forceMode, wakeup);
}

EXPORT_API void PxRigidbodyExt_ComputeLinearAngularImpulse(PxRigidBody* rigidbody, PxTransform_Managed globalPose, PxVec3_Managed point, PxVec3_Managed impulse, PxReal invMassScale, PxReal invInertiaScale, PxVec3_Managed* linearImpulse, PxVec3_Managed* angularImpulse) {
	PxVec3 _linearImpulse;
	PxVec3 _angularImpulse;

	PxRigidBodyExt::computeLinearAngularImpulse(*rigidbody, PxTransform_IN(globalPose), PxVec3_IN(point), PxVec3_IN(impulse), invMassScale, invInertiaScale, _linearImpulse, _angularImpulse);

	*linearImpulse = PxVec3_OUT(_linearImpulse);
	*angularImpulse = PxVec3_OUT(_angularImpulse);
}

EXPORT_API void PxRigidbodyExt_ComputeVelocityDeltaFromImpulse(PxRigidBody* rigidbody, PxTransform_Managed globalPose, PxVec3_Managed point, PxVec3_Managed impulse, const PxReal invMassScale, const PxReal invInertiaScale, PxVec3_Managed* deltaLinearVelocity, PxVec3_Managed* deltaAngularVelocity) {
	PxVec3 _deltaLinearVelocity;
	PxVec3 _deltaAngularVelocity;

	PxRigidBodyExt::computeVelocityDeltaFromImpulse(*rigidbody, PxTransform_IN(globalPose), PxVec3_IN(point), PxVec3_IN(impulse), invMassScale, invInertiaScale, _deltaLinearVelocity, _deltaAngularVelocity);

	*deltaLinearVelocity = PxVec3_OUT(_deltaLinearVelocity);
	*deltaAngularVelocity = PxVec3_OUT(_deltaAngularVelocity);
}

EXPORT_API PxVec3_Managed PxRigidbodyExt_GetVelocityAtPos(PxRigidBody* rigidbody, PxVec3_Managed pos) {
	return PxVec3_OUT(PxRigidBodyExt::getVelocityAtPos(*rigidbody, PxVec3_IN(pos)));
}

EXPORT_API PxVec3_Managed PxRigidbodyExt_GetVelocityAtOffset(PxRigidBody* rigidbody, PxVec3_Managed pos) {
	return PxVec3_OUT(PxRigidBodyExt::getVelocityAtOffset(*rigidbody, PxVec3_IN(pos)));
}

EXPORT_API PxVec3_Managed PxRigidbodyExt_GetLocalVelocityAtLocalPos(PxRigidBody* rigidbody, PxVec3_Managed pos) {
	return PxVec3_OUT(PxRigidBodyExt::getLocalVelocityAtLocalPos(*rigidbody, PxVec3_IN(pos)));
}

EXPORT_API bool PxRigidbodyExt_SetMassAndUpdateInertia(PxRigidBody* rigidbody, PxReal mass, PxVec3_Managed massLocalPose) {
	PxVec3 _massLocalPose = PxVec3_IN(massLocalPose);
	return PxRigidBodyExt::setMassAndUpdateInertia(*rigidbody, mass, &_massLocalPose);
}

EXPORT_API bool PxRigidbodyExt_UpdateMassAndInertia(PxRigidBody* rigidbody, PxReal density, PxVec3_Managed massLocalPose) {
	PxVec3 _massLocalPose = PxVec3_IN(massLocalPose);
	return PxRigidBodyExt::updateMassAndInertia(*rigidbody, density, &_massLocalPose);
}
