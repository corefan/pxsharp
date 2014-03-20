#include "Common.h"

EXPORT_API PxU32 PxScene_GetActiveTransforms(PxScene* self, PxActiveTransform_Managed* buffer, PxU32 bufferSize) {
	PxU32 count;
	const PxActiveTransform* ats = self->getActiveTransforms(count);
	PxU32 realCount = PxMin(count, bufferSize);

	for (PxU32 i = 0; i < realCount; ++i) {
		buffer[i].actor = ats[i].actor;
		buffer[i].actorType = (PxConcreteType::Enum) ats[i].actor->getConcreteType();
		buffer[i].actor2World = PxTransform_OUT(ats[i].actor2World);
		buffer[i].userData = ats[i].userData;
	}

	return realCount;
}

EXPORT_API PxU32 PxScene_GetBroadPhaseRegions(PxScene* self, PxBroadPhaseRegionInfo_Managed* userBuffer, PxU32 bufferSize) {
	PxBroadPhaseRegionInfo* userBuffer_tmp = new PxBroadPhaseRegionInfo[bufferSize];
	PxU32 count = self->getBroadPhaseRegions(userBuffer_tmp, bufferSize);

	for (PxU32 i = 0; i < count; ++i) {
		userBuffer[i] = PxBroadPhaseRegionInfo_OUT(userBuffer_tmp[i]);
	}

	return count;
}

EXPORT_API void PxScene_Simulate_0(PxScene* self, PxReal elapsedTime) {
	self->simulate(elapsedTime);
}

EXPORT_API void PxScene_Simulate_1(PxScene* self, PxReal elapsedTime, PxsScratchBlock block) {
	self->simulate(elapsedTime, NULL, block.ptr, block.size, true);
}

EXPORT_API void PxScene_Simulate_2(PxScene* self, PxReal elapsedTime, PxsScratchBlock block, bool controlSimulation) {
	self->simulate(elapsedTime, NULL, block.ptr, block.size, controlSimulation);
}

EXPORT_API bool PxScene_Raycast(
	PxScene* self,
	PxVec3_Managed origin,
	PxVec3_Managed direction,
	PxReal distance,
	PxRaycastBuffer* buffer,
	PxHitFlag::Enum flags,
	PxQueryFilterData_Managed filterData,
	PxQueryFilterCallback* callback)
{
	return self->raycast(PxVec3_IN(origin), PxVec3_IN(direction), distance, *buffer, flags, PxQueryFilterData_IN(filterData), callback);
}