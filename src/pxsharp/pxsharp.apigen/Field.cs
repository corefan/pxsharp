using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PxSharp.ApiGen {
    public class Field : Base<Field> {
        public Type Type;
        public Class Class;
        public Func<Field, string> ExternNameResolver = f => f.NativeName;
        public Func<Field, string> ManagedNameResolver = f => f.NativeName;
        public Func<Field, string> NativeTypeTextResolver;

        public string NativeTypeText {
            get { return NativeTypeTextResolver(this); }
        }

        public string ExternName {
            get { return ExternNameResolver(this); }
        }

        public string ManagedName {
            get { return ManagedNameResolver(this); }
        }

        public string GetterName (Context context) {
            return ExternName + context.CppExternFieldGetterSuffix;
        }

        public string SetterName (Context context) {
            return ExternName + context.CppExternFieldSetterSuffix;
        }

        public void WithClass (Class otherClass, Action<Field> action) {
            var orgClass = Class;

            try {
                Class = otherClass;
                action(this);
            } finally {
                Class = orgClass;
            }
        }
    }
}
