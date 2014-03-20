using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PxSharp {
    using PxU8 = System.Byte;
    using PxI8 = System.SByte;
    using PxU16 = System.UInt16;
    using PxI16 = System.Int16;
    using PxU32 = System.UInt32;
    using PxI32 = System.Int32;
    using PxU64 = System.UInt64;
    using PxI64 = System.Int64;
    using PxF32 = System.Single;
    using PxF64 = System.Double;
    using PxReal = System.Single;
    using PxExtended = System.Double;

    [StructLayout(LayoutKind.Sequential)]
    public struct PxDominanceGroup {
        public PxU8 Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxClientID {
        public PxU8 Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxMaterialTableIndex {
        public PxU16 Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxTriangleID {
        public PxU32 Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObstacleHandle {
        public PxU32 Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxFilterObjectAttributes {
        public PxU32 Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PxControllerStats {
        public PxU16 NbIterations;
        public PxU16 NbFullUpdates;
        public PxU16 NbPartialUpdates;
    };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PxHullPolygon {
        public fixed PxReal Plane[4];
        public PxU16 NbVerts;
        public PxU16 IndexBase;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxBroadPhaseCaps {
        public PxU32 MaxNbRegions;
        public PxU32 MaxNbObjects;
        public bool NeedsPredefinedBounds;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxsScratchBlock {
        public IntPtr Ptr;
        public PxU32 Size;
    };

    public partial struct PxContactPair {
        public PxU32 ExtractContacts (PxsContactPairPointBuffer buffer) {
            return Pxs.ExtractContacts(ref this, buffer);
        }
    }

    public partial struct PxVec3 {
        public static PxVec3 Zero { get { return new PxVec3() { X = 0, Y = 0, Z = 0 }; } }
        public static PxVec3 One { get { return new PxVec3() { X = 1, Y = 1, Z = 1 }; } }
    }

    public partial struct PxQuat {
        public static PxQuat Identity { get { return new PxQuat() { X = 0, Y = 0, Z = 0, W = 1 }; } }
        public static PxQuat Zero { get { return new PxQuat() { X = 0, Y = 0, Z = 0, W = 0 }; } }
    }

    public partial struct PxPlane {
        public PxVec3 Normal {
            get { return N; }
            set { N = value; }
        }

        public PxReal OriginDistance {
            get { return D; }
            set { D = value; }
        }
    }

    public partial struct PxTransform {
        public static PxTransform Identity { get { return new PxTransform() { P = PxVec3.Zero, Q = PxQuat.Identity }; } }

        public PxVec3 Position {
            get { return P; }
            set { P = value; }
        }

        public PxQuat Rotation {
            get { return Q; }
            set { Q = value; }
        }
    }
}
