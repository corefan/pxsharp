using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxSharp.ApiGen {
    public class StructDef {
        public string Name = "";
        public bool SkipIn = false;
        public bool SkipOut = false;
        public bool SkipMacro = false;
        public string CppCtor = "";
        public string ExternSuffix = "_Managed";
        public string[] Fields = new string[0];
        public string[] CopyFields = new string[0];
        public string[] SpecialStatementsIn = new string[0];
        public string[] SpecialStatementsOut = new string[0];
    }
}
