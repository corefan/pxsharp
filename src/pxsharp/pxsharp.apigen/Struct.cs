using System;
using System.Collections.Generic;
using System.Linq;

namespace PxSharp.ApiGen {
    public class StructField {
        public Type Type;
        public string Name;
        public bool Convert = true;
        public int Rank = 0;
        public bool Const = false;

        public IEnumerable<string> GetFieldFields (Context context) {
            Struct s = null;

            try {
                s = context.FindStruct(Type.NativeName);
            } catch { }

            if (s != null) {
                foreach (var field in s.AllFields) {
                    if (field.Convert == false)
                        continue;

                    foreach (var fieldField in field.GetFieldFields(context)) {
                        yield return Name + "." + fieldField;
                    }
                }
            } else {
                yield return Name;
            }
        }
    }

    public class Struct : Base<Struct> {
        public bool Unsafe = false;
        public Struct Parent;
        public List<Struct> Children;
        public List<StructField> Fields;
        public string CustomConstructor = "";
        public bool GenerateNativeConverters = true;
        public int SortOrder = 0;
        public Func<Struct, string> ExternNameResolver;
        public Func<Struct, string> ManagedNameResolver;

        public IEnumerable<StructField> AllFields {
            get {
                if (Parent != null) {
                    return Parent.AllFields.Concat(Fields);
                }

                return Fields;
            }
        }

        public string ExternName {
            get { return ExternNameResolver(this); }
        }

        public string ManagedName {
            get { return ManagedNameResolver(this); }
        }

        public override void OnCloned (Struct parent) {
            Fields = new List<StructField>();
            Children = new List<Struct>();
        }
    }
}
