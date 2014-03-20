using System.Linq;

namespace PxSharp.ApiGen {
    public static class ExternGenerator {

        static void GenerateField (Context context, OutputFile file, Field f, Class cls) {
            f.WithClass(cls, field => {
                file.BeginLine();
                file.Append("EXPORT_API ");
                file.Append(field.Type.ExternType + " ");
                file.Append(field.ExternName + context.CppExternFieldGetterSuffix);
                file.Append("(" + cls.NativeName + "* " + context.CppSelfParameterName + ") { return ");
                file.Append(field.Type.ConvertFromNativeToExtern(context.CppSelfParameterName + "->" + field.NativeName));
                file.Append("; }");
                file.EndLine();

                file.BeginLine();
                file.Append("EXPORT_API void ");
                file.Append(field.ExternName + context.CppExternFieldSetterSuffix);
                file.Append("(" + cls.NativeName + "* " + context.CppSelfParameterName + ", ");
                file.Append(field.Type.ExternType + " value) { ");
                file.Append(context.CppSelfParameterName + "->" + field.NativeName + " = " + field.Type.ConvertFromExternToNative("value") + "; }");
                file.EndLine();
            });
        }

        static void GenerateFunction (Context context, OutputFile file, Function f, Class cls) {
            if (f.EmitExtern) {
                f.WithClass(cls, func => {
                    file.BeginLine();
                    file.Append("EXPORT_API ");
                    file.Append(func.ReturnType.ExternType + " ");
                    file.Append(func.ExternName);
                    file.Append("(");

                    if (cls != null && func.IsStatic == false) {
                        file.Append(cls.NativeName + "* " + context.CppSelfParameterName);

                        if (func.HasParameters) {
                            file.Append(", ");
                        }
                    }

                    file.Append(func.Parameters.Select(p => p.Type.ExternType + " " + p.NativeName).Join(", "));
                    file.Append(") {");
                    file.EndLine();

                    // function body

                    file.Indented(() => {
                        foreach (var tup in func.ExternTempVars) {
                            file.AppendLineFormat("{0} {1};", tup.Item1, tup.Item2);
                        }

                        string argsList = func.Parameters.Select(p => p.Type.ConvertFromExternToNative(p.NativeName)).Join(", ");
                        string callExpr = "";

                        if (cls != null && func.IsStatic == false) {
                            callExpr += context.CppSelfParameterName + "->";
                        }

                        callExpr += func.NativeName + "(" + argsList + ")";

                        file.BeginLine();

                        if (func.ReturnType.NativeType == "void") {
                            file.Append(callExpr);
                        } else {

                            if (func.CaptureReturnValue) {
                                file.Append(func.ReturnType.NativeType + " __apigen__returnval = ");
                            } else {
                                file.Append("return ");
                            }

                            file.Append(func.ReturnType.ConvertFromNativeToExtern(callExpr));
                        }

                        file.Append(";");
                        file.EndLine();

                        foreach (var type in func.ExternOutAssigns) {
                            file.AppendLineFormat("*{0} = {1};", type.Parameter.NativeName, type.ConvertFromNativeToExtern(type.Parameter.TempName));
                        }

                        if (func.CaptureReturnValue) {
                            file.AppendLine("return __apigen__returnval;");
                        }
                    });

                    file.AppendLine("}");
                });
            }
        }

        static void GenerateCastFunction (OutputFile file, Class from, Class to) {
            file.AppendLineFormat("EXPORT_API {1}* {2}({0}* p) {{ return static_cast<{1}*>(p); }}", from.NativeName, to.NativeName, from.CastFunctionTo(to));
        }

        static void GenerateIncludes (Context context, OutputFile file) {
            if (context.CppIncludes.Count > 0) {
                foreach (var include in context.CppIncludes) {
                    file.AppendLineFormat("#include \"{0}\"", include);
                }

                file.AppendLine();
            }
        }

        public static void Generate (Context context) {
            using (var file = context.CppOutput.CreateFile("Classes.cpp")) {
                GenerateIncludes(context, file);

                foreach (Class cls in context.MatchedClasses) {
                    file.AppendLine();
                    file.AppendLine("// " + cls.NativeName);

                    foreach (var field in cls.AllFields) {
                        GenerateField(context, file, field, cls);
                    }

                    foreach (var method in cls.AllMethods) {
                        GenerateFunction(context, file, method, cls);
                    }

                    foreach (var parent in cls.AllParents) {
                        GenerateCastFunction(file, cls, parent);
                        GenerateCastFunction(file, parent, cls);
                    }
                }
            }
        }
    }
}
