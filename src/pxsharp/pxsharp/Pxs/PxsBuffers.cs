using System.Runtime.InteropServices;

namespace PxSharp {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct PxsTriggerPairBuffer {
        public PxTriggerPair* Pairs;
        public uint Count;
    };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct PxsConstraintInfoBuffer {
        PxConstraintInfo* Constraints;
        public uint Count;
    };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct PxsContactPairBuffer {
        PxContactPair* Pairs;
        public uint Count;
    };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct PxsActorBuffer {
        public PxActor* Buffer;
        public uint Count;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct PxsContactPairPointBuffer {
        public PxContactPairPoint* Buffer;
        public uint Count;
    }
}
