#include "Common.h"

PxsBroadPhaseCallback::PxsBroadPhaseCallback(
	PxsBroadPhaseCallbackShapeOutOfBoundsDelegate shapeOutOfBounds,
	PxsBroadPhaseCallbackAggregateOutOfBoundsDelegate aggregateOutOfBounds) :
	mShapeOutOfBounds(shapeOutOfBounds),
	mAggregateOutOfBounds(aggregateOutOfBounds) {
}

PxsBroadPhaseCallback::~PxsBroadPhaseCallback() {
	mShapeOutOfBounds = NULL;
	mAggregateOutOfBounds = NULL;
}

void PxsBroadPhaseCallback::onObjectOutOfBounds(PxShape& shape, PxActor& actor) {
	mShapeOutOfBounds(&shape, &actor);
}

void PxsBroadPhaseCallback::onObjectOutOfBounds(PxAggregate& aggregate) {
	mAggregateOutOfBounds(&aggregate);
}

PXS_DEFINE_CTOR2(PxsBroadPhaseCallback, PxsBroadPhaseCallbackShapeOutOfBoundsDelegate, PxsBroadPhaseCallbackAggregateOutOfBoundsDelegate);
PXS_DEFINE_DTOR(PxsBroadPhaseCallback);
