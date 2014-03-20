using System;

namespace PxSharp {
    public struct PxFlags16<T> where T : struct {
        UInt16 bits;

        public static implicit operator UInt16 (PxFlags16<T> flags) {
            return flags.bits;
        }

        public static implicit operator Boolean (PxFlags16<T> flags) {
            return flags.bits != 0;
        }

        public static implicit operator PxFlags16<T> (UInt16 bits) {
            PxFlags16<T> flags;
            flags.bits = bits;
            return flags;
        }

        public static implicit operator PxFlags16<T> (Int32 bits) {
            PxFlags16<T> flags;
            flags.bits = (UInt16) bits;
            return flags;
        }

        public static bool operator == (PxFlags16<T> a, PxFlags16<T> b) {
            return a.bits == b.bits;
        }

        public static bool operator != (PxFlags16<T> a, PxFlags16<T> b) {
            return a.bits != b.bits;
        }

        public static bool operator > (PxFlags16<T> a, PxFlags16<T> b) {
            return a.bits > b.bits;
        }

        public static bool operator < (PxFlags16<T> a, PxFlags16<T> b) {
            return a.bits < b.bits;
        }

        public static bool operator >= (PxFlags16<T> a, PxFlags16<T> b) {
            return a.bits >= b.bits;
        }

        public static bool operator <= (PxFlags16<T> a, PxFlags16<T> b) {
            return a.bits <= b.bits;
        }

        public static PxFlags16<T> operator & (PxFlags16<T> a, PxFlags16<T> b) {
            return a.bits & b.bits;
        }

        public static PxFlags16<T> operator & (PxFlags16<T> a, T b) {
            return a.bits & InterOp.EnumToInt(b);
        }

        public static PxFlags16<T> operator & (T b, PxFlags16<T> a) {
            return a.bits & InterOp.EnumToInt(b);
        }

        public static PxFlags16<T> operator | (PxFlags16<T> a, PxFlags16<T> b) {
            return a.bits | b.bits;
        }

        public static PxFlags16<T> operator | (PxFlags16<T> a, T b) {
            return a.bits | InterOp.EnumToInt(b);
        }

        public static PxFlags16<T> operator | (T b, PxFlags16<T> a) {
            return a.bits | InterOp.EnumToInt(b);
        }

        public static PxFlags16<T> operator ^ (PxFlags16<T> a, PxFlags16<T> b) {
            return a.bits ^ b.bits;
        }

        public static PxFlags16<T> operator ^ (PxFlags16<T> a, T b) {
            return a.bits ^ InterOp.EnumToInt(b);
        }

        public static PxFlags16<T> operator ^ (T b, PxFlags16<T> a) {
            return a.bits ^ InterOp.EnumToInt(b);
        }

        public static PxFlags16<T> operator >> (PxFlags16<T> a, int shift) {
            return a.bits >> shift;
        }

        public static PxFlags16<T> operator << (PxFlags16<T> a, int shift) {
            return a.bits << shift;
        }

        public static PxFlags16<T> operator ~ (PxFlags16<T> a) {
            return ~a.bits;
        }

        public override bool Equals (object obj) {
            if (obj is PxFlags16<T>) {
                return this == (PxFlags16<T>) obj;
            }

            return false;
        }

        public override int GetHashCode () {
            return bits;
        }
    }
}
