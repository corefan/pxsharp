using System;
using System.Text.RegularExpressions;

namespace PxSharp.ApiGen {
    public class Parameter : Base<Parameter> {
        public Type Type;
        public int Index;
        public bool HideInManaged;
        public Function Function;
        public Func<Parameter, string> TypeTextResolver;
        public Func<Parameter, string> ManagedToPInvokeExpressionResolver = p => p.NativeName;
        public Class Class { get { return Function.Class; } }

        public string ManagedToPInvokeExpression {
            get { return ManagedToPInvokeExpressionResolver(this); }
        }

        public string MatchValue {
            get { return Match.Value.Trim(); }
        }

        public string TypeText {
            get { return TypeTextResolver(this); }
        }
    }
}
