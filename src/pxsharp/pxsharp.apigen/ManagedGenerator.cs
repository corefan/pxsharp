using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxSharp.ApiGen {
    public static class ManagedGenerator {
        static void GenerateField (Context context, OutputFile file, Field f, Class cls) {
            f.WithClass(cls, field => {
                file.AppendLineFormat("public {0} {1} {{", field.Type.ManagedType, field.ManagedName);

                file.Indented(() => {
                    string getExpr = context.CSharpPInvokeClassName + "." + field.GetterName(context) + "(" + context.CppSelfParameterName + ")";
                    file.AppendLineFormat("get {{ return {0}; }}", field.Type.ConvertFromPInvokeToManaged(getExpr));

                    string setExpr = context.CSharpPInvokeClassName + "." + field.SetterName(context);
                    file.AppendLineFormat("set {{ {0}({1}, {2}); }}", setExpr, context.CppSelfParameterName, field.Type.ConvertFromManagedToPinvoke("value"));
                });

                file.AppendLine("}");
            });
        }

        static void GenerateFunction (Context context, OutputFile file, Function f, Class cls) {
            if (f.EmitManaged) {
                f.WithClass(cls, func => {
                    file.BeginLine();
                    file.Append("public ");

                    if (func.IsStatic) {
                        file.Append("static ");
                    }

                    file.Append(func.ReturnType.ManagedType + " ");
                    file.Append(func.ManagedName + " (");
                    file.Append(func.Parameters.Where(p => !p.HideInManaged).Select(p => p.Type.ManagedType + " " + p.NativeName).Join(", "));
                    file.Append(") { ");
                    file.EndLine();

                    file.Indented(() => {
                        if (func.IsStatic == false) {
                            file.AppendLine("#if DEBUG");
                            file.AppendLine("if (IsNull) throw new PxsNullPointerException();");
                            file.AppendLine("#endif");
                        }

                        if (func.HasFixedStatements) {
                            file.AppendLine("unsafe {");
                            file.IndentLevel += 1;

                            for (int i = 0; i < func.ManagedFixedStatements.Count; ++i) {
                                var stmt = func.ManagedFixedStatements[i];
                                file.BeginLine();
                                file.AppendFormat("fixed(void* {0} = {1})", stmt.Item1, stmt.Item2);

                                if (i + 1 == func.ManagedFixedStatements.Count) {
                                    file.Append(" {");
                                }

                                file.EndLine();
                            }

                            file.IndentLevel += 1;
                        }

                        file.BeginLine();

                        if (func.ReturnsVoid == false) {
                            file.Append("return ");
                        }

                        string callExpr = "";
                        callExpr += context.CSharpPInvokeClassName + ".";
                        callExpr += func.ExternName + "(";

                        if (func.IsStatic == false) {
                            callExpr += context.CppSelfParameterName;

                            if (func.Parameters.Count > 0) {
                                callExpr += ", ";
                            }
                        }

                        callExpr += func.Parameters.Select(p => p.Type.ConvertFromManagedToPinvoke(p.ManagedToPInvokeExpression)).Join(", ");
                        callExpr += ")";

                        file.Append(func.ReturnType.ConvertFromPInvokeToManaged(callExpr));
                        file.Append(";");
                        file.EndLine();

                        if (func.HasFixedStatements) {
                            file.IndentLevel -= 1;
                            file.AppendLine("}");
                            file.IndentLevel -= 1;
                            file.AppendLine("}");
                        }
                    });

                    file.AppendLine("}");
                });
            }
        }

        static void PrintIgnoreList (OutputFile file, string header, List<string> ignoreList) {
            if (ignoreList.Count > 0) {
                file.AppendLine("// " + header);

                foreach (var name in ignoreList) {
                    file.AppendLine("// " + name);
                }

                file.AppendLine();
            }
        }

        static void GenerateCastOperator (Context context, OutputFile file, Class cls, Class from, Class to, bool isImplicit) {
            file.BeginLine();
            file.Append("public static ");
            file.Append(isImplicit ? "implicit " : "explicit ");
            file.Append("operator ");
            file.Append(to.ManagedName);
            file.Append(" (" + from.ManagedName + " value) { return new ");
            file.Append(to.ManagedName + "(");
            file.Append(context.CSharpPInvokeClassName + ".");
            file.Append(from.CastFunctionTo(to) + "(value.");
            file.Append(context.CppSelfParameterName + ")); }");
            file.EndLine();
        }

        public static void Generate (Context context) {
            using (var file = context.CSharpOutput.CreateFile("Delegates.cs")) {
                context.WriteCSharpUsings(file);

                file.AppendLineFormat("namespace {0} {{ ", context.CSharpNamespace);
                file.Indented(() => {
                    context.WriteCSharpAliases(file);

                    foreach (var group in context.MatchedUserDelegates.GroupBy(x => x.NativeName)) {
                        var func = group.First();

                        file.AppendLine("[SuppressUnmanagedCodeSecurity]");
                        file.AppendLine("[UnmanagedFunctionPointer(CallingConvention.StdCall)]");

                        file.BeginLine();
                        file.Append("public delegate ");
                        file.Append(func.ReturnType.ManagedType + " ");
                        file.Append(func.ManagedName + "(");
                        file.Append(func.Parameters.Select(x => x.Type.ManagedType + " " + x.NativeName).Join(", "));
                        file.Append(");");
                        file.EndLine();
                        file.AppendLine();
                    }
                });

                file.AppendLine("}");
            }

            using (var file = context.CSharpOutput.CreateFile("Functions.cs")) {
                context.WriteCSharpUsings(file);

                file.AppendLineFormat("namespace {0} {{ ", context.CSharpNamespace);
                file.Indented(() => {
                    context.WriteCSharpAliases(file);

                    foreach (var grouping in context.MatchedUserFunction.GroupBy(x => x.StaticClassName)) {
                        file.AppendLineFormat("public static partial class {0} {{", grouping.Key);

                        file.Indented(() => {
                            foreach (var func in grouping) {
                                GenerateFunction(context, file, func, null);
                            }
                        });

                        file.AppendLine("}");
                    }
                });

                file.AppendLine("}");
            }

            using (var file = context.CSharpOutput.CreateFile("Classes.cs")) {
                context.WriteCSharpUsings(file);

                file.AppendLineFormat("namespace {0} {{ ", context.CSharpNamespace);
                file.Indented(() => {
                    context.WriteCSharpAliases(file);

                    foreach (var cls in context.MatchedClasses) {
                        file.BeginLine();
                        file.AppendFormat("public partial struct {0} ", cls.ManagedName);

                        if (cls.ReleaseMethod != null) {
                            file.Append(": System.IDisposable ");
                        }

                        file.Append("{");
                        file.EndLine();

                        file.Indented(() => {
                            PrintIgnoreList(file, "Ignored Fields", cls.IgnoredFields);
                            PrintIgnoreList(file, "Ignored Methods", cls.IgnoredMethods);

                            file.AppendLineFormat("internal readonly IntPtr {0};", context.CppSelfParameterName);
                            file.AppendLineFormat("internal {0} (IntPtr _{1}) {{ {1} = _{1}; }}", cls.ManagedName, context.CppSelfParameterName);
                            file.AppendLine();
                            file.AppendLineFormat("public IntPtr NativePtr {{ get {{ return {0}; }} }}", context.CppSelfParameterName);
                            file.AppendLineFormat("public bool IsNull {{ get {{ return {0} == IntPtr.Zero; }} }}", context.CppSelfParameterName);
                            file.AppendLine();
                            file.AppendLineFormat("public static implicit operator IntPtr({0} value) {{ return value.{1}; }}", cls.ManagedName, context.CppSelfParameterName);
                            file.AppendLineFormat("public static explicit operator {0}(IntPtr value) {{ return new {0}(value); }}", cls.ManagedName);
                            file.AppendLineFormat("public static implicit operator Boolean({0} value) {{ return value.{1} != IntPtr.Zero; }}", cls.ManagedName, context.CppSelfParameterName);
                            file.AppendLineFormat("public override string ToString () {{ return IsNull ? \"{{{0}@NULL}}\" : \"{{{0}@\" + NativePtr + \"}}\"; }}", cls.ManagedName);

                            foreach (var field in cls.AllFields) {
                                GenerateField(context, file, field, cls);
                            }

                            foreach (var method in cls.AllMethods) {
                                GenerateFunction(context, file, method, cls);
                            }

                            if (cls.ReleaseMethod != null) {
                                file.AppendLine("public void Dispose() { ");
                                file.Indented(() => {
                                    file.AppendLineFormat("{0}();", cls.ReleaseMethod.ManagedName);
                                });
                                file.AppendLine("}");
                            }

                            foreach (var parent in cls.AllParents) {
                                GenerateCastOperator(context, file, cls, cls, parent, true);
                                GenerateCastOperator(context, file, cls, parent, cls, false);
                            }
                        });


                        file.AppendLine("}");
                    }
                });

                file.AppendLine("}");
            }
        }
    }
}
