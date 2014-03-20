#include "Common.h"

static PxDefaultAllocator defaultAllocator;

EXPORT_API PxFoundation* Px_CreateFoundation(PxErrorCallback* callback) {
	return PxCreateFoundation(PX_PHYSICS_VERSION, defaultAllocator, *callback);
}

EXPORT_API PxFoundation* Px_GetFoundation() {
	return &PxGetFoundation();
}

EXPORT_API bool Px_InitExtensions(PxPhysics* physics) {
	return PxInitExtensions(*physics);
}

EXPORT_API void Px_RegisterArticulations(PxPhysics* physics){
	PxRegisterArticulations(*physics);
}

EXPORT_API void Px_RegisterCloth(PxPhysics* physics){
	PxRegisterCloth(*physics);
}

EXPORT_API void Px_RegisterHeightFields(PxPhysics* physics){
	PxRegisterHeightFields(*physics);
}

EXPORT_API void Px_RegisterParticles(PxPhysics* physics){
	PxRegisterParticles(*physics);
}

EXPORT_API void Px_RegisterUnifiedHeightFields(PxPhysics* physics){
	PxRegisterUnifiedHeightFields(*physics);
}

EXPORT_API void Px_CloseExtensions() {
	PxCloseExtensions();
}

EXPORT_API PxCooking* Px_CreateCooking(PxFoundation* foundation, PxCookingParams_Managed cookingParams) {
	return PxCreateCooking(PX_PHYSICS_VERSION, *foundation, PxCookingParams_IN(cookingParams));
}

EXPORT_API PxPhysics* Px_CreatePhysics(PxFoundation* foundation, PxTolerancesScale_Managed ts) {
	return PxCreatePhysics(PX_PHYSICS_VERSION, *foundation, PxTolerancesScale_IN(ts));
}

EXPORT_API PxPhysics* Px_CreateBasePhysics(PxFoundation* foundation, PxTolerancesScale_Managed ts) {
	return PxCreateBasePhysics(PX_PHYSICS_VERSION, *foundation, PxTolerancesScale_IN(ts));
}

EXPORT_API PxRigidStatic* Px_CreatePlane(PxPhysics* physics, PxPlane* plane, PxMaterial* material) {
	return PxCreatePlane(*physics, *plane, *material);
}

EXPORT_API PxRigidDynamic* Px_CreateDynamic(PxPhysics* physics, PxTransform_Managed transform, PxGeometry* geometry, PxMaterial* material, PxReal density, PxTransform_Managed offset) {
	return PxCreateDynamic(*physics, PxTransform_IN(transform), *geometry, *material, density, PxTransform_IN(offset));
}

EXPORT_API PxRigidDynamic* Px_CloneDynamic(PxPhysics* physics, PxTransform_Managed transform, PxRigidDynamic* dynamic) {
	return PxCloneDynamic(*physics, PxTransform_IN(transform), *dynamic);
}

EXPORT_API PxRigidStatic* Px_CreateStatic(PxPhysics* physics, PxTransform_Managed transform, PxGeometry* geometry, PxMaterial* material, PxTransform_Managed offset) {
	return PxCreateStatic(*physics, PxTransform_IN(transform), *geometry, *material, PxTransform_IN(offset));
}

EXPORT_API PxRigidStatic* Px_CloneStatic(PxPhysics* physics, PxTransform_Managed transform, PxRigidStatic* dynamic) {
	return PxCloneStatic(*physics, PxTransform_IN(transform), *dynamic);
}

EXPORT_API PxFixedJoint* Px_FixedJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
	return PxFixedJointCreate(*physics, actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}

EXPORT_API PxSphericalJoint* Px_SphericalJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
	return PxSphericalJointCreate(*physics, actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}

EXPORT_API PxDistanceJoint* Px_DistanceJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
	return PxDistanceJointCreate(*physics, actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}

EXPORT_API PxPrismaticJoint* Px_PrismaticJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
	return PxPrismaticJointCreate(*physics, actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}

EXPORT_API PxRevoluteJoint* Px_RevoluteJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
	return PxRevoluteJointCreate(*physics, actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}

EXPORT_API PxD6Joint* Px_D6JointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
	return PxD6JointCreate(*physics, actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}

EXPORT_API bool Px_FilterObjectIsTrigger(PxFilterObjectAttributes attr) {
	return PxFilterObjectIsTrigger(attr);
}

EXPORT_API bool Px_FilterObjectIsKinematic(PxFilterObjectAttributes attr) {
	return PxFilterObjectIsKinematic(attr);
}

EXPORT_API PxFilterObjectType::Enum Px_GetFilterObjectType(PxFilterObjectAttributes attr) {
	return PxGetFilterObjectType(attr);
}

EXPORT_API void Px_GetFilterOps(PxFilterOp::Enum* op0, PxFilterOp::Enum* op1, PxFilterOp::Enum* op2) {
	PxGetFilterOps(*op0, *op1, *op2);
}

EXPORT_API void Px_SetFilterOps(PxFilterOp::Enum op0, PxFilterOp::Enum op1, PxFilterOp::Enum op2) {
	PxSetFilterOps(op0, op1, op2);
}

EXPORT_API bool Px_GetFilterBool() {
	return PxGetFilterBool();
}

EXPORT_API void Px_SetFilterBool(bool value) {
	PxSetFilterBool(value);
}

EXPORT_API void Px_GetFilterConstants(PxGroupsMask_Managed* c0, PxGroupsMask_Managed* c1) {
	PxGroupsMask c0_, c1_;
	PxGetFilterConstants(c0_, c1_);
	*c0 = PxGroupsMask_OUT(c0_);
	*c1 = PxGroupsMask_OUT(c1_);
}

EXPORT_API void Px_SetFilterConstants(PxGroupsMask_Managed c0, PxGroupsMask_Managed c1) {
	PxSetFilterConstants(PxGroupsMask_IN(c0), PxGroupsMask_IN(c1));
}

EXPORT_API void Px_SetGroup(PxRigidActor* actor, PxU16 group) {
	PxSetGroup(*actor, group);
}

EXPORT_API PxU16 Px_GetGroup(PxRigidActor* actor) {
	return PxGetGroup(*actor);
}

EXPORT_API void Px_SetGroupCollision(PxU16 group0, PxU16 group1, bool enable) {
	PxSetGroupCollisionFlag(group0, group1, enable);
}

EXPORT_API bool Px_GetGroupCollision(PxU16 group0, PxU16 group1) {
	return PxGetGroupCollisionFlag(group0, group1);
}

EXPORT_API void Px_SetGroupMask(PxRigidActor* actor, PxGroupsMask_Managed mask) {
	PxSetGroupsMask(*actor, PxGroupsMask_IN(mask));
}

EXPORT_API PxGroupsMask_Managed Px_GetGroupMask(PxRigidActor* actor) {
	return PxGroupsMask_OUT(PxGetGroupsMask(*actor));
}

EXPORT_API PxU32 Px_GetValue(PxCookingValue::Enum value) {
	return PxGetValue(value);
}

EXPORT_API PxU32 Px_GetGaussMapVertexLimitForPlatform(PxPlatform::Enum platform) {
	return PxGetGaussMapVertexLimitForPlatform(platform);
}

EXPORT_API PxCpuDispatcher* Px_DefaultCpuDispatcherCreate(PxU32 numThreads) {
	return static_cast<PxCpuDispatcher*>(PxDefaultCpuDispatcherCreate(numThreads));
}

EXPORT_API PxControllerManager* Px_CreateControllerManager(PxScene* scene) {
	return PxCreateControllerManager(*scene);
}

// Vehicle API

EXPORT_API bool Px_InitVehicleSDK(PxPhysics* physics) {
	return PxInitVehicleSDK(*physics);
}

EXPORT_API void Px_CloseVehicleSDK() {
	PxCloseVehicleSDK();
}

EXPORT_API void Px_VehicleSetBasisVectors(PxVec3_Managed up, PxVec3_Managed forward) {
	PxVehicleSetBasisVectors(PxVec3_IN(up), PxVec3_IN(forward));
}

EXPORT_API void Px_VehicleSetUpdateMode(PxVehicleUpdateMode::Enum mode) {
	PxVehicleSetUpdateMode(mode);
}

