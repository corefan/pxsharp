#include "Common.h"

EXPORT_API PxVisualDebuggerConnectionFlag::Enum PxVisualDebuggerExt_GetAllConnectionFlags() {
	return (PxVisualDebuggerConnectionFlag::Enum) (PxU32)PxVisualDebuggerExt::getAllConnectionFlags();
}

EXPORT_API PxVisualDebuggerConnectionFlag::Enum PxVisualDebuggerExt_GetDefaultConnectionFlags() {
	return (PxVisualDebuggerConnectionFlag::Enum) (PxU32)PxVisualDebuggerExt::getDefaultConnectionFlags();
}

EXPORT_API PxVisualDebuggerConnection* PxVisualDebuggerExt_CreateConnection0(PxVisualDebuggerConnectionManager* manager, const char* hostIp, PxI32 port, PxU32 timeout, PxVisualDebuggerConnectionFlag::Enum flags) {
	return PxVisualDebuggerExt::createConnection(manager, hostIp, port, timeout, flags);
}

EXPORT_API PxVisualDebuggerConnection* PxVisualDebuggerExt_CreateConnection1(PxVisualDebuggerConnectionManager* manager, const char* filename, PxVisualDebuggerConnectionFlag::Enum flags) {
	return PxVisualDebuggerExt::createConnection(manager, filename, flags);
}

EXPORT_API void PxVisualDebuggerConnection_AddRef(PxVisualDebuggerConnection* self) {
	self->addRef();
}

EXPORT_API void PxVisualDebuggerConnection_Release(PxVisualDebuggerConnection* self) {
	self->release();
}

EXPORT_API void PxVisualDebuggerConnection_Disconnect(PxVisualDebuggerConnection* self) {
	self->disconnect();
}

EXPORT_API void PxVisualDebuggerConnection_Flush(PxVisualDebuggerConnection* self) {
	self->flush();
}

EXPORT_API bool PxVisualDebuggerConnection_IsConnected(PxVisualDebuggerConnection* self) {
	return self->isConnected();
}

