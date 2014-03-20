#include "Common.h"

PxsRaycastFilterCallback::PxsRaycastFilterCallback(PxsQueryPreFilterDelegate pre, PxsRaycastPostFilterDelegate post) : mPreFilter(pre), mPostFilter(post) {
}

PxsRaycastFilterCallback:: ~PxsRaycastFilterCallback() {
	mPreFilter = NULL;
	mPostFilter = NULL;
}

PxQueryHitType::Enum PxsRaycastFilterCallback::preFilter(const PxFilterData& filterData, const PxShape* shape, const PxRigidActor* actor, PxHitFlags& queryFlags) {
	return this->mPreFilter(PxFilterData_OUT(filterData), shape, actor, &queryFlags);
}

PxQueryHitType::Enum PxsRaycastFilterCallback::postFilter(const PxFilterData& filterData, const PxQueryHit& hit) {
	return this->mPostFilter(PxFilterData_OUT(filterData), PxRaycastHit_OUT(static_cast<const PxRaycastHit&>(hit)));
}

PXS_DEFINE_CTOR2(PxsRaycastFilterCallback, PxsQueryPreFilterDelegate, PxsRaycastPostFilterDelegate);
PXS_DEFINE_DTOR(PxsRaycastFilterCallback);