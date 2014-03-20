using System.Collections.Generic;
using System.Linq;

namespace PxSharp.ApiGen {
    public static class PInvokeGenerator {
        static void WriteAttributes (Context context, OutputFile file, string entryPoint) {
            file.AppendLineFormat("[SuppressUnmanagedCodeSecurity]");
            file.AppendLineFormat("[DllImport(\"{0}\", EntryPoint = \"{1}\", ExactSpelling = true)]", context.CSharpPInvokeDllName, entryPoint);
        }

        static void GenerateField (Context context, OutputFile file, Field f, Class cls) {
            f.WithClass(cls, field => {
                WriteAttributes(context, file, field.GetterName(context));
                file.BeginLine();
                file.Append("internal static extern ");
                file.Append(field.Type.PInvokeType + " ");
                file.Append(field.GetterName(context));
                file.Append(" (IntPtr " + context.CppSelfParameterName + ");");
                file.EndLine();
                file.AppendLine();

                WriteAttributes(context, file, field.SetterName(context));
                file.BeginLine();
                file.Append("internal static extern void ");
                file.Append(field.SetterName(context));
                file.Append(" (IntPtr " + context.CppSelfParameterName + ", ");
                file.Append(field.Type.PInvokeType + " value);");
                file.EndLine();
                file.AppendLine();
            });
        }

        static void GenerateFunction (Context context, OutputFile file, Function f, Class cls) {
            if (f.EmitPInvoke) {
                f.WithClass(cls, func => {
                    WriteAttributes(context, file, func.ExternName);
                    file.BeginLine();

                    if (func.ReturnType.PInvokeReturnAttributes != "") {
                        file.Append("[return: " + func.ReturnType.PInvokeReturnAttributes + "] ");
                    }

                    file.Append("public static extern ");
                    file.Append(func.ReturnType.PInvokeType + " ");
                    file.Append(func.ExternName + " (");

                    if (func.IsStatic == false) {
                        file.Append("[param: In] IntPtr " + context.CppSelfParameterName);

                        if (func.Parameters.Count > 0) {
                            file.Append(", ");
                        }
                    }

                    file.Append(func.Parameters.Select(p => {
                        var param = p.Type.PInvokeType + " " + p.NativeName;

                        if (p.Type.PInvokeParameterAttributes == "")
                            return param;

                        return "[param: " + p.Type.PInvokeParameterAttributes + "] " + param;
                    }).Join(", "));
                    file.Append(");");
                    file.EndLine();
                    file.AppendLine();
                });
            }
        }

        static void GenerateCastFunction (Context context, OutputFile file, Class from, Class to) {
            WriteAttributes(context, file, from.CastFunctionTo(to));
            file.AppendLineFormat("internal static extern IntPtr {0}(IntPtr p);", from.CastFunctionTo(to));
            file.AppendLine();
        }

        public static void Generate (Context context) {
            using (var file = context.CSharpOutput.CreateFile(context.CSharpPInvokeClassName + ".cs")) {
                context.WriteCSharpUsings(file);
                file.AppendLineFormat("namespace {0} {{ ", context.CSharpNamespace);
                file.Indented(() => {
                    context.WriteCSharpAliases(file);
                    file.AppendLineFormat("public static class {0} {{", context.CSharpPInvokeClassName);
                    file.Indented(() => {
                        foreach (Function func in context.MatchedUserFunction) {
                            GenerateFunction(context, file, func, null);
                        }

                        foreach (Class cls in context.MatchedClasses) {
                            foreach (var field in cls.AllFields) {
                                GenerateField(context, file, field, cls);
                            }

                            foreach (var method in cls.AllMethods) {
                                GenerateFunction(context, file, method, cls);
                            }

                            foreach (var parent in cls.AllParents) {
                                GenerateCastFunction(context, file, cls, parent);
                                GenerateCastFunction(context, file, parent, cls);
                            }
                        }
                    });
                    file.AppendLine("}");
                });
                file.AppendLine("}");
            }
        }
    }
}
