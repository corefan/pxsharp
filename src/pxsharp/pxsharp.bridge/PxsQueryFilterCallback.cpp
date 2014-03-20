#include "Common.h"

PxsQueryFilterCallback::PxsQueryFilterCallback(PxsQueryPreFilterDelegate pre, PxsQueryPostFilterDelegate post) : mPreFilter(pre), mPostFilter(post) {
}

PxsQueryFilterCallback::~PxsQueryFilterCallback() {
	mPreFilter = NULL;
	mPostFilter = NULL;
}

PxQueryHitType::Enum PxsQueryFilterCallback::preFilter(const PxFilterData& filterData, const PxShape* shape, const PxRigidActor* actor, PxHitFlags& queryFlags) {
	return this->mPreFilter(PxFilterData_OUT(filterData), shape, actor, &queryFlags);
}

PxQueryHitType::Enum PxsQueryFilterCallback::postFilter(const PxFilterData& filterData, const PxQueryHit& hit) {
	return this->mPostFilter(PxFilterData_OUT(filterData), PxQueryHit_OUT(hit));
}

PXS_DEFINE_CTOR2(PxsQueryFilterCallback, PxsQueryPreFilterDelegate, PxsQueryPostFilterDelegate);
PXS_DEFINE_DTOR(PxsQueryFilterCallback);