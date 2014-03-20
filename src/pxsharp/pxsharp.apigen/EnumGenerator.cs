using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PxSharp.ApiGen {
    public static class EnumGenerator {
        public static void Generate (Context config) {
            using (var file = config.CSharpOutput.CreateFile("Enums.cs")) {
                file.AppendLineFormat("namespace {0} {{", config.CSharpNamespace);
                file.IndentLevel += 1;

                foreach (var include in config.ReadHeaderFiles()) {
                    foreach (var enumeration in config.EnumerationTemplates) {
                        foreach (var match in include.Text.StripComments().MatchAll(enumeration.Regex)) {
                            var clone = enumeration.Clone();
                            clone.Match = match;

                            if (config.CppEnumBlackList.Contains(clone.ManagedName))
                                continue;

                            if (clone.AttributeWriter != null) {
                                clone.AttributeWriter(clone, file);
                            }

                            file.AppendLineFormat("public enum {0} : uint {{", clone.ManagedName);
                            file.IndentLevel += 1;

                            foreach (var value in clone.ValuesParser(clone).OrderBy(x => x.CharacterOffset)) {

                                file.BeginLine();
                                file.Append(value.Name);

                                if (value.Value.HasValue()) {
                                    file.Append(" = unchecked((uint) (");
                                    file.Append(value.Value.Replace("::", "."));
                                    file.Append("))");
                                }

                                file.Append(",");
                                file.EndLine();
                            }

                            file.IndentLevel -= 1;
                            file.AppendLine("}");
                        }
                    }
                }

                file.IndentLevel -= 1;
                file.AppendLine("}");
            }
        }
    }
}
