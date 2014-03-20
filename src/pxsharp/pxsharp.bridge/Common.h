#ifndef PXSHARP_COMMON
#define PXSHARP_COMMON

#include "PxPhysicsAPI.h"

#if _MSC_VER
#define PXSHARP_WIN 1
#else
#define PXSHARP_OSX 1
#endif

#if PXSHARP_WIN
#include <type_traits>
#if PXS_APIGEN
#define EXPORT_API EXPORT_FUNC
#else
#define EXPORT_API extern "C" __declspec(dllexport)
#endif
#else
#if PXS_APIGEN
#define EXPORT_API EXPORT_FUNC
#else
#define EXPORT_API extern "C" 
#endif
#endif

#define PXS_STATIC_ASSERT(assert) static_assert(assert, #assert)

using namespace physx;

/*
#define PxVec2_IN(expr) PxVec2((expr).x, (expr).y)
#define PxVec3_IN(expr) PxVec3((expr).x, (expr).y, (expr).z)
#define PxVec4_IN(expr) PxVec4((expr).x, (expr).y, (expr).z, (expr).w)
#define PxQuat_IN(expr) PxQuat((expr).x, (expr).y, (expr).z, (expr).w)
#define PxTransform_IN(expr) PxTransform(PxVec3((expr).p.x, (expr).p.y, (expr).p.z), PxQuat((expr).q.x, (expr).q.y, (expr).q.z, (expr).q.w))
*/

#define PXS_DEFINE_CTOR0(type) \
EXPORT_API type* type ## _New0() { \
return new type(); \
} \

#define PXS_DEFINE_CTOR1(type, arg0Type) \
	EXPORT_API type* type ## _New1(arg0Type arg0) { \
	return new type(arg0); \
	} \

#define PXS_DEFINE_CTOR2(type, arg0Type, arg1Type) \
	EXPORT_API type* type ## _New2(arg0Type arg0, arg1Type arg1) { \
	return new type(arg0, arg1); \
	} \

#define PXS_DEFINE_CTOR3(type, arg0Type, arg1Type, arg2Type) \
	EXPORT_API type* type ## _New3(arg0Type arg0, arg1Type arg1, arg2Type arg2) { \
	return new type(arg0, arg1, arg2); \
	} \

#define PXS_DEFINE_CTOR4(type, arg0Type, arg1Type, arg2Type, arg3Type) \
	EXPORT_API type* type ## _New4(arg0Type arg0, arg1Type arg1, arg2Type arg2, arg3Type arg3) { \
	return new type(arg0, arg1, arg2, arg3); \
	} \

#define PXS_DEFINE_CTOR5(type, arg0Type, arg1Type, arg2Type, arg3Type, arg4Type) \
	EXPORT_API type* type ## _New5(arg0Type arg0, arg1Type arg1, arg2Type arg2, arg3Type arg3, arg4Type arg4) { \
	return new type(arg0, arg1, arg2, arg3, arg4); \
	} \

#define PXS_DEFINE_DTOR(type) \
	EXPORT_API void type ## _Delete(type* self) { \
	delete self; \
	} \

#if PXS_APIGEN
#define DELEGATE DELEGATE_FUNC
#else
#define DELEGATE typedef
#endif

#include "Generated/Structs.h"

struct PxsScratchBlock {
	void* ptr;
	PxU32 size;
};

struct PxsActorBuffer {
	PxActor** actors;
	PxU32 count;
};

struct PxsTriggerPairBuffer {
	PxTriggerPair_Managed* pairs;
	PxU32 count;
};

struct PxsConstraintInfoBuffer {
	PxConstraintInfo_Managed* constraints;
	PxU32 count;
};

struct PxsContactPairBuffer {
	PxContactPair_Managed* pairs;
	PxU32 count;
};

struct PxsContactPairPointBuffer {
	PxContactPairPoint_Managed* points;
	PxU32 count;
};

DELEGATE PxQueryHitType::Enum(_stdcall *PxsQueryPreFilterDelegate) (PxFilterData_Managed filterData, const PxShape* shape, const PxRigidActor* actor, PxHitFlags* queryFlags);
DELEGATE PxQueryHitType::Enum(_stdcall *PxsQueryPostFilterDelegate) (PxFilterData_Managed filterData, PxQueryHit_Managed hit);
DELEGATE PxQueryHitType::Enum(_stdcall *PxsRaycastPostFilterDelegate) (PxFilterData_Managed filterData, PxRaycastHit_Managed hit);
DELEGATE PxQueryHitType::Enum(_stdcall *PxsSweepPostFilterDelegate) (PxFilterData_Managed filterData, PxSweepHit_Managed hit);

DELEGATE void(_stdcall *PxsBroadPhaseCallbackShapeOutOfBoundsDelegate) (PxShape* shape, PxActor* actor);
DELEGATE void(_stdcall *PxsBroadPhaseCallbackAggregateOutOfBoundsDelegate) (PxAggregate* aggregate);
DELEGATE bool(_stdcall *PxsControllerFilterDelegate) (const PxController* a, const PxController* b);
DELEGATE void(_stdcall *PxsErrorDelegate) (PxErrorCode::Enum code, const char* message);

class PxsErrorCallback : public PxErrorCallback {
private:
	PxsErrorDelegate mErrorDelegate;

public:
	PxsErrorCallback(PxsErrorDelegate errorDelegate);
	void reportError(PxErrorCode::Enum code, const char* message, const char* file, int line);
};

class PxsBroadPhaseCallback : public PxBroadPhaseCallback
{
private:
	PxsBroadPhaseCallbackShapeOutOfBoundsDelegate mShapeOutOfBounds;
	PxsBroadPhaseCallbackAggregateOutOfBoundsDelegate mAggregateOutOfBounds;

public:
	PxsBroadPhaseCallback(
		PxsBroadPhaseCallbackShapeOutOfBoundsDelegate shapeOutOfBounds,
		PxsBroadPhaseCallbackAggregateOutOfBoundsDelegate aggregateOutOfBounds);

	virtual	~PxsBroadPhaseCallback() override;
	virtual void onObjectOutOfBounds(PxShape& shape, PxActor& actor) override;
	virtual void onObjectOutOfBounds(PxAggregate& aggregate) override;
};

class PxsControllerFilterCallback : public PxControllerFilterCallback {
private:
	PxsControllerFilterDelegate mDelegate;

public:
	PxsControllerFilterCallback(PxsControllerFilterDelegate callback);
	virtual ~PxsControllerFilterCallback();
	virtual bool filter(const PxController& a, const PxController& b);
};

class PxsQueryFilterCallback : public PxQueryFilterCallback {
private:
	PxsQueryPreFilterDelegate mPreFilter;
	PxsQueryPostFilterDelegate mPostFilter;

public:
	PX_INLINE PxsQueryFilterCallback(PxsQueryPreFilterDelegate pre, PxsQueryPostFilterDelegate post);
	virtual ~PxsQueryFilterCallback() override;
	virtual PxQueryHitType::Enum preFilter(const PxFilterData& filterData, const PxShape* shape, const PxRigidActor* actor, PxHitFlags& queryFlags) override;
	virtual PxQueryHitType::Enum postFilter(const PxFilterData& filterData, const PxQueryHit& hit) override;
};

class PxsRaycastFilterCallback : public PxQueryFilterCallback {
private:
	PxsQueryPreFilterDelegate mPreFilter;
	PxsRaycastPostFilterDelegate mPostFilter;

public:
	PxsRaycastFilterCallback(PxsQueryPreFilterDelegate pre, PxsRaycastPostFilterDelegate post);
	virtual ~PxsRaycastFilterCallback();
	virtual PxQueryHitType::Enum preFilter(const PxFilterData& filterData, const PxShape* shape, const PxRigidActor* actor, PxHitFlags& queryFlags);
	virtual PxQueryHitType::Enum postFilter(const PxFilterData& filterData, const PxQueryHit& hit);
};

class PxsSweepFilterCallback : public PxQueryFilterCallback {
private:
	PxsQueryPreFilterDelegate mPreFilter;
	PxsSweepPostFilterDelegate mPostFilter;

public:
	PxsSweepFilterCallback(PxsQueryPreFilterDelegate pre, PxsSweepPostFilterDelegate post);
	virtual ~PxsSweepFilterCallback();
	virtual PxQueryHitType::Enum preFilter(const PxFilterData& filterData, const PxShape* shape, const PxRigidActor* actor, PxHitFlags& queryFlags);
	virtual PxQueryHitType::Enum postFilter(const PxFilterData& filterData, const PxQueryHit& hit);
};

DELEGATE void(_stdcall *PxsSimulationWakeDelegate) (PxsActorBuffer buffer);
DELEGATE void(_stdcall *PxsSimulationSleepDelegate) (PxsActorBuffer buffer);
DELEGATE void(_stdcall *PxsSimulationTriggerDelegate) (PxsTriggerPairBuffer buffer);
DELEGATE void(_stdcall *PxsSimulationConstraintBreakDelegate) (PxsConstraintInfoBuffer buffer);
DELEGATE void(_stdcall *PxsSimulationContactDelegate) (PxContactPairHeader_Managed pairHeader, PxsContactPairBuffer buffer);

class PxsSimulationEventCallback : public PxSimulationEventCallback
{
private:
	PxsSimulationWakeDelegate onWakeDelegate;
	PxsSimulationSleepDelegate onSleepDelegate;
	PxsSimulationTriggerDelegate onTriggerDelegate;
	PxsSimulationConstraintBreakDelegate onConstraintBreakDelegate;
	PxsSimulationContactDelegate onContactDelegate;

public:
	PxsSimulationEventCallback(
		PxsSimulationWakeDelegate _onWakeDelegate,
		PxsSimulationSleepDelegate _onSleepDelegate,
		PxsSimulationTriggerDelegate _onTriggerDelegate,
		PxsSimulationConstraintBreakDelegate _onConstraintBreakDelegate,
		PxsSimulationContactDelegate _onContactDelegate);

	virtual void onConstraintBreak(PxConstraintInfo* constraints, PxU32 count) override;
	virtual void onWake(PxActor** actors, PxU32 count) override;
	virtual void onSleep(PxActor** actors, PxU32 count) override;
	virtual void onContact(const PxContactPairHeader& pairHeader, const PxContactPair* pairs, PxU32 nbPairs) override;
	virtual void onTrigger(PxTriggerPair* pairs, PxU32 count) override;

	virtual ~PxsSimulationEventCallback() {}
};


PXS_STATIC_ASSERT((sizeof(PxU8) == 1));
PXS_STATIC_ASSERT((sizeof(PxI8) == 1));
PXS_STATIC_ASSERT((sizeof(PxU16) == 2));
PXS_STATIC_ASSERT((sizeof(PxI16) == 2));
PXS_STATIC_ASSERT((sizeof(PxU32) == 4));
PXS_STATIC_ASSERT((sizeof(PxI32) == 4));
PXS_STATIC_ASSERT((sizeof(PxU64) == 8));
PXS_STATIC_ASSERT((sizeof(PxI64) == 8));
PXS_STATIC_ASSERT((sizeof(PxVec2) == 8));
PXS_STATIC_ASSERT((sizeof(PxVec3) == 12));
PXS_STATIC_ASSERT((sizeof(PxVec3_Managed) == sizeof(PxVec3)));
PXS_STATIC_ASSERT((sizeof(PxVec4) == 16));
PXS_STATIC_ASSERT((sizeof(PxQuat) == 16));
PXS_STATIC_ASSERT((sizeof(PxQuat_Managed) == sizeof(PxQuat)));
PXS_STATIC_ASSERT((sizeof(PxReal) == 4));
PXS_STATIC_ASSERT((sizeof(PxTransform) == 28));
PXS_STATIC_ASSERT((sizeof(PxTransform_Managed) == sizeof(PxTransform)));
PXS_STATIC_ASSERT((sizeof(PxBounds3) == 24));
PXS_STATIC_ASSERT((sizeof(PxBounds3_Managed) == sizeof(PxBounds3)));
PXS_STATIC_ASSERT((sizeof(PxExtended) == 8));
PXS_STATIC_ASSERT((sizeof(PxExtendedVec3) == 24));
PXS_STATIC_ASSERT((sizeof(PxExtendedVec3_Managed) == sizeof(PxExtendedVec3)));

#endif