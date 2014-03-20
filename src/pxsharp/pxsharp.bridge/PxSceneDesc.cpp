#include "Common.h"

PxFilterFlags FilterShader_Default(
	PxFilterObjectAttributes attributes0, PxFilterData filterData0,
	PxFilterObjectAttributes attributes1, PxFilterData filterData1,
	PxPairFlags& pairFlags, const void* constantBlock, PxU32 constantBlockSize) {

	if (PxFilterObjectIsTrigger(attributes0) || PxFilterObjectIsTrigger(attributes1))
	{
		pairFlags =
			PxPairFlag::eNOTIFY_TOUCH_FOUND |
			PxPairFlag::eNOTIFY_TOUCH_LOST |
			PxPairFlag::eNOTIFY_TOUCH_PERSISTS;
	}
	else
	{
		pairFlags =
			PxPairFlag::eRESOLVE_CONTACTS |
			PxPairFlag::eNOTIFY_CONTACT_POINTS |
			PxPairFlag::eNOTIFY_THRESHOLD_FORCE_FOUND |
			PxPairFlag::eNOTIFY_THRESHOLD_FORCE_LOST |
			PxPairFlag::eNOTIFY_THRESHOLD_FORCE_PERSISTS |
			PxPairFlag::eNOTIFY_TOUCH_FOUND |
			PxPairFlag::eNOTIFY_TOUCH_LOST |
			PxPairFlag::eNOTIFY_TOUCH_PERSISTS;
	}

	return PxFilterFlag::eDEFAULT;
}

EXPORT_API PxSceneDesc* PxSceneDesc_New(PxTolerancesScale_Managed ts) {
	PxSceneDesc* sd = new PxSceneDesc(PxTolerancesScale_IN(ts));
	sd->filterShader = FilterShader_Default;
	return sd;
}

EXPORT_API void PxSceneDesc_Delete(PxSceneDesc* self) {
	delete self;
}