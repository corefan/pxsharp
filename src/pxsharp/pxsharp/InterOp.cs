using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PxSharp {
    static class InterOp {
        public static int EnumToInt<T> (T val) where T : struct {
            return (int) (ValueType) val;
        }

        public static T IntToEnum<T> (int val) where T : struct {
            return (T) (ValueType) val;
        }
    }
}
