#include "Common.h"


PxsSimulationEventCallback::PxsSimulationEventCallback(
	PxsSimulationWakeDelegate _onWakeDelegate,
	PxsSimulationSleepDelegate _onSleepDelegate,
	PxsSimulationTriggerDelegate _onTriggerDelegate,
	PxsSimulationConstraintBreakDelegate _onConstraintBreakDelegate,
	PxsSimulationContactDelegate _onContactDelegate)
	:
	onWakeDelegate(_onWakeDelegate),
	onSleepDelegate(_onSleepDelegate),
	onTriggerDelegate(_onTriggerDelegate),
	onConstraintBreakDelegate(_onConstraintBreakDelegate),
	onContactDelegate(_onContactDelegate)
{

}

void PxsSimulationEventCallback::onWake(PxActor** actors, PxU32 count) {
	this->onWakeDelegate({ actors, count });
}

void PxsSimulationEventCallback::onSleep(PxActor** actors, PxU32 count) {
	this->onSleepDelegate({ actors, count });
}

void PxsSimulationEventCallback::onConstraintBreak(PxConstraintInfo* _constraints, PxU32 count) {
	PxConstraintInfo_Managed* constraints = new PxConstraintInfo_Managed[count];

	for (int i = 0; i < count; ++i) {
		constraints[i] = PxConstraintInfo_OUT(_constraints[i]);
	}

	this->onConstraintBreakDelegate({ constraints, count });
	delete constraints;
}

void PxsSimulationEventCallback::onTrigger(PxTriggerPair* _pairs, PxU32 count) {
	PxTriggerPair_Managed* pairs = new PxTriggerPair_Managed[count];

	for (int i = 0; i < count; ++i) {
		pairs[i] = PxTriggerPair_OUT(_pairs[i]);
	}

	this->onTriggerDelegate({ pairs, count });
	delete pairs;
}

void PxsSimulationEventCallback::onContact(const PxContactPairHeader& pairHeader, const PxContactPair* _pairs, PxU32 nbPairs) {
	PxContactPair_Managed* pairs = new PxContactPair_Managed[nbPairs];

	for (int i = 0; i < nbPairs; ++i) {
		pairs[i] = PxContactPair_OUT(_pairs[i]);
	}

	this->onContactDelegate(PxContactPairHeader_OUT(pairHeader), { pairs, nbPairs });
	delete pairs;
}

EXPORT_API PxU32 Pxs_ExtractContacts(PxContactPair_Managed* pair, PxsContactPairPointBuffer buffer) {
	PxContactPair _pair = PxContactPair_IN(*pair);
	PxContactPairPoint* _buffer = new PxContactPairPoint[buffer.count];
	PxU32 result = _pair.extractContacts(_buffer, buffer.count);

	for (PxU32 i = 0; i < result; ++i) {
		buffer.points[i] = PxContactPairPoint_OUT(_buffer[i]);
	}

	return result;
}

PXS_DEFINE_CTOR5(PxsSimulationEventCallback,
	PxsSimulationWakeDelegate,
	PxsSimulationSleepDelegate,
	PxsSimulationTriggerDelegate,
	PxsSimulationConstraintBreakDelegate,
	PxsSimulationContactDelegate);

PXS_DEFINE_DTOR(PxsSimulationEventCallback);