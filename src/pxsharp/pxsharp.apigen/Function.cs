using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PxSharp.ApiGen {
    public class Function : Base<Function> {
        public Class Class;
        public Type ReturnType;
        public bool EmitExtern = true;
        public bool EmitPInvoke = true;
        public bool EmitManaged = true;
        public bool IsStatic = false;
        public string StaticClassName = "";
        public bool CaptureReturnValue = false;
        public List<Parameter> Parameters;
        public List<Type> ExternOutAssigns;
        public List<Tuple<string, string>> ExternTempVars;
        public List<Tuple<string, string>> ManagedFixedStatements;
        public Func<Function, string> ExternNameResolver;
        public Func<Function, string> ManagedNameResolver;
        public Func<Function, string> ReturnTypeTextResolver;
        public Func<Function, string> ArgumentListTextResolver;

        public bool HasParameters {
            get { return Parameters.Count > 0; }
        }

        public bool HasFixedStatements {
            get { return ManagedFixedStatements.Count > 0; }
        }

        public bool ReturnsVoid {
            get { return ReturnType.NativeType == "void"; }
        }

        public string ExternName {
            get { return ExternNameResolver(this); }
        }

        public string ManagedName {
            get { return ManagedNameResolver(this); }
        }

        public string ReturnTypeText {
            get { return ReturnTypeTextResolver(this); }
        }

        public string ArgumentListText {
            get { return ArgumentListTextResolver(this); }
        }

        public void WithClass (Class otherClass, Action<Function> action) {
            var orgClass = Class;

            try {
                Class = otherClass;
                action(this);
            } finally {
                Class = orgClass;
            }
        }

        public override void OnCloned (Function parent) {
            Parameters = new List<Parameter>();
            ExternTempVars = new List<Tuple<string, string>>();
            ExternOutAssigns = new List<Type>();
            ManagedFixedStatements = new List<Tuple<string, string>>();
        }
    }
}
