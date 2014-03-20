using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PxSharp.ApiGen {
    public class Enum {
        public Regex Regex;
        public Match Match;
        public Func<Enum, string> ManagedNameParser;
        public Func<Enum, IEnumerable<EnumValue>> ValuesParser;
        public Action<Enum, OutputFile> AttributeWriter;

        public string ManagedName {
            get { return ManagedNameParser(this); }
        }

        public Enum Clone () {
            return (Enum) MemberwiseClone();
        }
    }
}
