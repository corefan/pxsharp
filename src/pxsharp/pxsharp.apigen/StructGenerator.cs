using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxSharp.ApiGen {
    public static class StructGenerator {

        static void GenerateInOutFunc (Context context, OutputFile cppFile, Struct s, string from, string to, bool isIn) {
            cppFile.AppendLineFormat("#ifndef {0}_{1}", s.NativeName, isIn ? "IN" : "OUT");
            cppFile.AppendLineFormat("__forceinline {1} {2}_{3}(const {0}& val) {{", from, to, s.NativeName, isIn ? "IN" : "OUT");
            cppFile.Indented(() => {
                if (isIn && String.IsNullOrEmpty(s.CustomConstructor) == false) {
                    cppFile.AppendLineFormat("{0} res = {1};", to, s.CustomConstructor);
                } else {
                    cppFile.AppendLineFormat("{0} res;", to);
                }

                foreach (var field in s.AllFields) {
                    if (field.Convert == false)
                        continue;

                    foreach (var fieldField in field.GetFieldFields(context)) {
                        string valExpr = "val." + fieldField;

                        if (fieldField == field.Name) {
                            if (isIn) {
                                valExpr = field.Type.ConvertFromExternToNative(valExpr);
                            } else {
                                valExpr = field.Type.ConvertFromNativeToExtern(valExpr);
                            }
                        }

                        if (field.Rank == 0) {
                            cppFile.AppendLineFormat("res.{0} = {1};", fieldField, valExpr);
                        } else {
                            for (int i = 0; i < field.Rank; ++i) {
                                cppFile.AppendLineFormat("res.{0}[{2}] = {1}[{2}];", fieldField, valExpr, i);
                            }
                        }
                    }
                }

                cppFile.AppendLine("return res;");
            });
            cppFile.AppendLine("};");
            cppFile.AppendLine("#endif");
        }

        internal static void Generate (Context context) {
            using (var cppFile = context.CppOutput.CreateFile("Structs.h")) {
                using (var csFile = context.CSharpOutput.CreateFile("Structs.cs")) {

                    // C++ _Managed structs

                    foreach (var s in context.MatchedStructs.OrderBy(x => x.SortOrder)) {
                        if (s.GenerateNativeConverters == false)
                            continue;

                        // Struct
                        cppFile.AppendLineFormat("struct {0} {{", s.ExternName);
                        cppFile.Indented(() => {
                            foreach (var f in s.AllFields) {
                                cppFile.AppendLineFormat("{3}{0} {1}{2};", f.Type.ExternType, f.Name, f.Rank > 0 ? "[" + f.Rank + "]" : "", f.Const ? "const " : "");
                            }
                        });
                        cppFile.AppendLine("};");

                        // Generate in/out funcs
                        GenerateInOutFunc(context, cppFile, s, s.ExternName, s.NativeName, true);
                        GenerateInOutFunc(context, cppFile, s, s.NativeName, s.ExternName, false);
                    }

                    // C# structs

                    // write out using statements
                    context.WriteCSharpUsings(csFile);

                    // write out namespace
                    csFile.AppendLineFormat("namespace {0} {{", context.CSharpNamespace);

                    csFile.Indented(() => {
                        // aliases to match internal types
                        context.WriteCSharpAliases(csFile);

                        foreach (var s in context.MatchedStructs.OrderBy(x => x.SortOrder)) {
                            csFile.AppendLine("[StructLayout(LayoutKind.Sequential)]");
                            csFile.AppendLineFormat("public {1}partial struct {0} {{", s.ManagedName, s.Unsafe ? "unsafe " : "");

                            csFile.Indented(() => {
                                // Fields
                                foreach (var f in s.AllFields) {
                                    if (f.Rank > 0) {
                                        switch (f.Type.ManagedType) {
                                            case "PxU32":
                                            case "PxU16":
                                            case "PxU8":
                                                csFile.AppendLineFormat("public fixed {0} {1}[{2}];", f.Type.ManagedType, f.Name.UpperCaseFirst(), f.Rank);
                                                break;

                                            default:
                                                for (int i = 0; i < f.Rank; ++i) {
                                                    csFile.AppendLineFormat("{0} {1}_{2};", f.Type.ManagedType, f.Name.UpperCaseFirst(), i);
                                                }
                                                break;
                                        }

                                    } else {
                                        csFile.AppendLineFormat("public {0} {1};", f.Type.ManagedType, f.Name.UpperCaseFirst());
                                    }
                                }

                                // Accessor method for arrays
                                foreach (var f in s.AllFields) {
                                    if (f.Rank > 0) {
                                        switch (f.Type.ManagedType) {
                                            case "PxU32":
                                            case "PxU16":
                                            case "PxU8":
                                                break;

                                            default:
                                                csFile.AppendLineFormat("public {0} {1}(int index) {{ ", f.Type.ManagedType, f.Name.UpperCaseFirst());
                                                csFile.Indented(() => {
                                                    csFile.AppendLine("switch (index) { ");
                                                    csFile.Indented(() => {
                                                        for (int i = 0; i < f.Rank; ++i) {
                                                            csFile.AppendLineFormat("case {0}: return {1}_{0};", i, f.Name.UpperCaseFirst());
                                                        }

                                                        csFile.AppendLine("default: throw new System.IndexOutOfRangeException();");
                                                    });
                                                    csFile.AppendLine("}");
                                                });
                                                csFile.AppendLine("}");
                                                break;
                                        }
                                    }
                                }
                            });

                            csFile.AppendLine("};");
                            csFile.AppendLine("");
                        }
                    });

                    csFile.AppendLine("}");
                }
            }
        }
    }
}
