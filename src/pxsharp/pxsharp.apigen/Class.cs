using System;
using System.Collections.Generic;
using System.Linq;

namespace PxSharp.ApiGen {
    public class Class : Base<Class> {
        public Class BaseClass;
        public Function ReleaseMethod;
        public List<Field> Fields;
        public List<Class> SubClasses;
        public List<Function> Methods;
        public List<string> IgnoredMethods;
        public List<string> IgnoredFields;
        public Func<Class, string> NativeBodyTextResolver;
        public Func<Class, string> ManagedNameResolver = c => c.NativeName;
        public Func<Class, Class, string> CastFunctionNameResolver = (from, to) => String.Format("CAST_{0}_TO_{1}", from.NativeName, to.NativeName);

        public string NativeBody {
            get { return NativeBodyTextResolver(this); }
        }

        public IEnumerable<Class> AllParents {
            get {
                Class current = this.BaseClass;

                while (current != null) {
                    yield return current;
                    current = current.BaseClass;
                }
            }
        }

        public IEnumerable<Field> AllFields {
            get {
                if (BaseClass != null) {
                    return BaseClass.AllFields.Concat(Fields);
                } else {
                    return Fields;
                }
            }
        }

        public IEnumerable<Function> AllMethods {
            get {
                if (BaseClass != null) {
                    return BaseClass.AllMethods.Concat(Methods).GroupBy(x => x.NativeName).SelectMany(g => {
                        if(g.Count() == 1) return g.ToArray();
                        return g.Where(x => ReferenceEquals(this, x.Class));
                    });
                } else {
                    return Methods;
                }
            }
        }

        public Class RootClass {
            get {
                Class current = this;

                while (current.BaseClass != null) {
                    current = current.BaseClass;
                }

                return current;
            }
        }

        public bool IsSubclass {
            get { return BaseClass != null; }
        }

        public string ManagedName {
            get { return ManagedNameResolver(this); }
        }

        public string CastFunctionTo (Class other) {
            return CastFunctionNameResolver(this, other);
        }

        public Function FindMethod (string name) {
            return Methods.FirstOrDefault(f => f.ManagedName == name);
        }

        public override void OnCloned (Class parent) {
            Fields = new List<Field>();
            Methods = new List<Function>();
            IgnoredMethods = new List<string>();
            IgnoredFields = new List<string>();
            SubClasses = new List<Class>();
        }
    }
}
