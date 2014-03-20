using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxSharp.ApiGen {
    public static class StructDefGenerator {

        static void GenerateInOutFunc (Context context, OutputFile cppFile, StructDef def, string from, string to, bool isIn) {
            cppFile.AppendLineFormat("__forceinline {1} {2}_{3}(const {0}& in) {{", from, to, def.Name, isIn ? "IN" : "OUT");
            cppFile.Indented(() => {
                cppFile.AppendLineFormat("{0} out{1};", to, isIn ? def.CppCtor : "");
                if (def.CopyFields.Length == 0) {
                    for (int i = 0; i < def.Fields.Length; i += 2) {
                        cppFile.AppendLineFormat("out.{0} = in.{0};", def.Fields[i + 1]);
                    }
                } else {
                    for (int i = 0; i < def.CopyFields.Length; i += 1) {
                        cppFile.AppendLineFormat("out.{0} = in.{0};", def.CopyFields[i]);
                    }
                }

                if (isIn) {
                    foreach (var stmt in def.SpecialStatementsIn) {
                        cppFile.AppendLine(stmt);
                    }
                } else {
                    foreach (var stmt in def.SpecialStatementsOut) {
                        cppFile.AppendLine(stmt);
                    }
                }

                cppFile.AppendLine("return out;");
            });
            cppFile.AppendLine("};");
        }

        public static void Generate (Context context) {

            using (var cppFile = context.CppOutput.CreateFile("Structs.h")) {
                using (var csFile = context.CSharpOutput.CreateFile("Structs.cs")) {

                    context.WriteCSharpUsings(csFile);
                    csFile.AppendLineFormat("namespace {0} {{", context.CSharpNamespace);

                    csFile.Indented(() => {
                        context.WriteCSharpAliases(csFile);

                        foreach (var def in context.StructDefinitions) {

                            // C++

                            cppFile.AppendLineFormat("struct {0}{1} {{", def.Name, def.ExternSuffix);
                            cppFile.Indented(() => {
                                for (int i = 0; i < def.Fields.Length; i += 2) {
                                    cppFile.AppendLineFormat("{0} {1};", context.MatchType(def.Fields[i + 0]).ExternType, def.Fields[i + 1]);
                                }
                            });
                            cppFile.AppendLine("};");

                            // IN / OUT funcs

                            if (def.SkipOut == false)
                                GenerateInOutFunc(context, cppFile, def, def.Name, def.Name + def.ExternSuffix, false);

                            if (def.SkipIn == false)
                                GenerateInOutFunc(context, cppFile, def, def.Name + def.ExternSuffix, def.Name, true);

                            // INLINE macro

                            if (def.SkipMacro == false) {
                                cppFile.BeginLine();
                                cppFile.AppendFormat("#define {0}_INLINE(to, from) ", def.Name);
                            }

                            if (def.CopyFields.Length == 0) {
                                for (int i = 0; i < def.Fields.Length; i += 2) {
                                    cppFile.AppendFormat("to.{0} = from.{0}; ", def.Fields[i + 1]);
                                }
                            } else {
                                for (int i = 0; i < def.CopyFields.Length; i += 1) {
                                    cppFile.AppendFormat("to.{0} = from.{0}; ", def.CopyFields[i]);
                                }
                            }

                            cppFile.EndLine();

                            // C#

                            csFile.AppendLine("[StructLayout(LayoutKind.Sequential)]");
                            csFile.AppendLineFormat("public partial struct {0} {{", def.Name);
                            csFile.Indented(() => {
                                for (int i = 0; i < def.Fields.Length; i += 2) {
                                    csFile.AppendLineFormat("public {0} {1};", context.MatchType(def.Fields[i + 0]).ManagedType, def.Fields[i + 1].UpperCaseFirst());
                                }
                            });
                            csFile.AppendLine("};");
                        }
                    });

                    csFile.AppendLine("}");
                }
            }
        }
    }
}
