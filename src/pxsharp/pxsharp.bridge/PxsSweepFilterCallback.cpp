#include "Common.h"

PxsSweepFilterCallback::PxsSweepFilterCallback(PxsQueryPreFilterDelegate pre, PxsSweepPostFilterDelegate post) : mPreFilter(pre), mPostFilter(post) {
}

PxsSweepFilterCallback:: ~PxsSweepFilterCallback() {
	mPreFilter = NULL;
	mPostFilter = NULL;
}

PxQueryHitType::Enum PxsSweepFilterCallback::preFilter(const PxFilterData& filterData, const PxShape* shape, const PxRigidActor* actor, PxHitFlags& queryFlags) {
	return this->mPreFilter(PxFilterData_OUT(filterData), shape, actor, &queryFlags);
}

PxQueryHitType::Enum PxsSweepFilterCallback::postFilter(const PxFilterData& filterData, const PxQueryHit& hit) {
	return this->mPostFilter(PxFilterData_OUT(filterData), PxSweepHit_OUT(static_cast<const PxSweepHit&>(hit)));
}

PXS_DEFINE_CTOR2(PxsSweepFilterCallback, PxsQueryPreFilterDelegate, PxsSweepPostFilterDelegate);
PXS_DEFINE_DTOR(PxsSweepFilterCallback);