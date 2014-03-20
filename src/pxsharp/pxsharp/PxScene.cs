using System;

namespace PxSharp {
    partial struct PxScene {
        public uint GetActiveTransforms (PxActiveTransform[] activeTransforms) {
            unsafe {
                fixed (PxActiveTransform* ptr = activeTransforms) {
                    return PInvoke.PxScene_GetActiveTransforms(this, new IntPtr(ptr), (uint) activeTransforms.Length);
                }
            }
        }
    }
}
