// File generated by ApiGen on 2013-12-14 08:40:25

#include "..\Common.h"

EXPORT_API bool Px_FilterObjectIsTrigger(PxFilterObjectAttributes attr) {
    return PxFilterObjectIsTrigger(attr);
}
EXPORT_API bool Px_FilterObjectIsKinematic(PxFilterObjectAttributes attr) {
    return PxFilterObjectIsKinematic(attr);
}
EXPORT_API PxFilterObjectType::Enum Px_GetFilterObjectType(PxFilterObjectAttributes attr) {
    return PxGetFilterObjectType(attr);
}
EXPORT_API PxU32 Px_GetValue(PxCookingValue::Enum cookValue) {
    return PxGetValue(cookValue);
}
EXPORT_API PxControllerManager* Px_CreateControllerManager(PxScene* scene) {
    return PxCreateControllerManager(*(scene));
}
EXPORT_API PxU32 Px_GetGaussMapVertexLimitForPlatform(PxPlatform::Enum targetPlatform) {
    return PxGetGaussMapVertexLimitForPlatform(targetPlatform);
}
EXPORT_API PxD6Joint* Px_D6JointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
    return PxD6JointCreate(*(physics), actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}
EXPORT_API void Px_GetFilterOps(PxFilterOp::Enum* op0, PxFilterOp::Enum* op1, PxFilterOp::Enum* op2) {
    PxGetFilterOps(*(op0), *(op1), *(op2));
}
EXPORT_API void Px_SetFilterOps(PxFilterOp::Enum* op0, PxFilterOp::Enum* op1, PxFilterOp::Enum* op2) {
    PxSetFilterOps(*(op0), *(op1), *(op2));
}
EXPORT_API bool Px_GetFilterBool() {
    return PxGetFilterBool();
}
EXPORT_API void Px_SetFilterBool(const bool enable) {
    PxSetFilterBool(enable);
}
EXPORT_API void Px_GetFilterConstants(PxGroupsMask_Managed* c0, PxGroupsMask_Managed* c1) {
    PxGroupsMask c0_tmp;
    PxGroupsMask c1_tmp;
    PxGetFilterConstants(c0_tmp, c1_tmp);
    *c0 = PxGroupsMask_OUT(c0_tmp);
    *c1 = PxGroupsMask_OUT(c1_tmp);
}
EXPORT_API void Px_SetFilterConstants(PxGroupsMask_Managed c0, PxGroupsMask_Managed c1) {
    PxSetFilterConstants(PxGroupsMask_IN(c0), PxGroupsMask_IN(c1));
}
EXPORT_API void Px_SetGroup(const PxRigidActor* actor, PxU16 collisionGroup) {
    PxSetGroup(*(actor), collisionGroup);
}
EXPORT_API PxU16 Px_GetGroup(const PxRigidActor* actor) {
    return PxGetGroup(*(actor));
}
EXPORT_API bool Px_GetGroupCollisionFlag(PxU16 group1, PxU16 group2) {
    return PxGetGroupCollisionFlag(group1, group2);
}
EXPORT_API void Px_SetGroupCollisionFlag(PxU16 group1, PxU16 group2, const bool enable) {
    PxSetGroupCollisionFlag(group1, group2, enable);
}
EXPORT_API PxDistanceJoint* Px_DistanceJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
    return PxDistanceJointCreate(*(physics), actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}
EXPORT_API bool Px_InitExtensions(PxPhysics* physics) {
    return PxInitExtensions(*(physics));
}
EXPORT_API void Px_CloseExtensions() {
    PxCloseExtensions();
}
EXPORT_API PxFixedJoint* Px_FixedJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
    return PxFixedJointCreate(*(physics), actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}
EXPORT_API PxPrismaticJoint* Px_PrismaticJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
    return PxPrismaticJointCreate(*(physics), actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}
EXPORT_API PxRevoluteJoint* Px_RevoluteJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
    return PxRevoluteJointCreate(*(physics), actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}
EXPORT_API PxRigidStatic* Px_CreatePlane(PxPhysics* sdk, const PxPlane* plane, PxMaterial* material) {
    return PxCreatePlane(*(sdk), *(plane), *(material));
}
EXPORT_API PxRigidDynamic* Px_CreateDynamic(PxPhysics* sdk, PxTransform_Managed transform, PxShape* shape, PxReal density) {
    return PxCreateDynamic(*(sdk), PxTransform_IN(transform), *(shape), density);
}
EXPORT_API PxRigidDynamic* Px_CloneDynamic(PxPhysics* physicsSDK, PxTransform_Managed transform, const PxRigidDynamic* body) {
    return PxCloneDynamic(*(physicsSDK), PxTransform_IN(transform), *(body));
}
EXPORT_API PxRigidStatic* Px_CreateStatic(PxPhysics* sdk, PxTransform_Managed transform, PxShape* shape) {
    return PxCreateStatic(*(sdk), PxTransform_IN(transform), *(shape));
}
EXPORT_API PxRigidStatic* Px_CloneStatic(PxPhysics* physicsSDK, PxTransform_Managed transform, const PxRigidActor* actor) {
    return PxCloneStatic(*(physicsSDK), PxTransform_IN(transform), *(actor));
}
EXPORT_API PxSphericalJoint* Px_SphericalJointCreate(PxPhysics* physics, PxRigidActor* actor0, PxTransform_Managed localFrame0, PxRigidActor* actor1, PxTransform_Managed localFrame1) {
    return PxSphericalJointCreate(*(physics), actor0, PxTransform_IN(localFrame0), actor1, PxTransform_IN(localFrame1));
}
EXPORT_API PxFoundation* Px_GetFoundation() {
    return &(PxGetFoundation());
}
EXPORT_API void Px_CloseVehicleSDK(PxSerializationRegistry* serializationRegistry) {
    PxCloseVehicleSDK(serializationRegistry);
}