using System;
using System.Text.RegularExpressions;

namespace PxSharp.ApiGen {
    public class Type : Base<Type> {
        public Field Field;
        public Function Function;
        public Parameter Parameter;
        public Func<Type, string> NativeTypeResolver = f => f.Match.Value.Trim();
        public Func<Type, string> ExternTypeResolver = f => f.NativeType;
        public Func<Type, string> PInvokeTypeResolver = f => f.NativeType;
        public Func<Type, string> PInvokeParameterAttributesResolver = f => "";
        public Func<Type, string> PInvokeReturnAttributesResolver = f => "";
        public Func<Type, string> ManagedTypeResolver = f => f.NativeType;
        public Func<Type, string, string> NativeToExternConverter = (f, expr) => expr;
        public Func<Type, string, string> ExternToNativeConverter = (f, expr) => expr;
        public Func<Type, string, string> PInvokeToManagedConverter = (f, expr) => expr;
        public Func<Type, string, string> ManagedToPinvokeConverter = (f, expr) => expr;

        public bool IsReturnType { get { return Function != null && ReferenceEquals(Function.ReturnType, this); } }
        public bool IsParameter { get { return Parameter != null; } }
        
        public string NativeType { get { return NativeTypeResolver(this).Trim(); } }
        public string ExternType { get { return ExternTypeResolver(this).Trim(); } }
        public string PInvokeType { get { return PInvokeTypeResolver(this).Trim(); } }
        public string ManagedType { get { return ManagedTypeResolver(this).Trim(); } }

        public string PInvokeParameterAttributes { get { return PInvokeParameterAttributesResolver(this).Trim(); } }
        public string PInvokeReturnAttributes { get { return PInvokeReturnAttributesResolver(this).Trim(); } }

        public string ConvertFromNativeToExtern (string expr) {
            return NativeToExternConverter(this, expr).Trim();
        }

        public string ConvertFromExternToNative (string expr) {
            return ExternToNativeConverter(this, expr).Trim();
        }

        public string ConvertFromPInvokeToManaged (string expr) {
            return PInvokeToManagedConverter(this, expr).Trim();
        }

        public string ConvertFromManagedToPinvoke (string expr) {
            return ManagedToPinvokeConverter(this, expr).Trim();
        }

        public Type () {
            NativeNameResolver = t => t.NativeType;
        }
    }
}
