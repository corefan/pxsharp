using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxSharp.ApiGen {
    public static class CodeParser {
        static void ParseFunction (Function funcClone, Context context) {
            Type typeClone = context.MatchTypeDelayed(funcClone.ReturnTypeText.Strip("physx::"));
            typeClone.Function = funcClone;
            funcClone.ReturnType = typeClone;

            foreach (var parameterTemplate in context.ParameterTemplates) {
                foreach (var parameterMatch in funcClone.ArgumentListText.MatchAll(parameterTemplate.Regex)) {
                    var parameterClone = parameterTemplate.Clone(parameterMatch);
                    parameterClone.Index = funcClone.Parameters.Count;
                    parameterClone.Function = funcClone;
                    parameterClone.Type = context.MatchTypeDelayed(parameterClone.TypeText.Strip("physx::"));
                    parameterClone.Type.Parameter = parameterClone;
                    parameterClone.Type.Function = funcClone;

                    funcClone.Parameters.Add(parameterClone);

                    parameterClone.InvokeOnMatched();
                    parameterClone.Type.InvokeOnMatched();
                }
            }

            funcClone.InvokeOnMatched();
            funcClone.ReturnType.InvokeOnMatched();
        }

        public static void Parse (Context context) {

            foreach (var source in context.ReadHeaderFiles()) {

                // structs
                foreach (var structTemplate in context.StructTemplates) {
                    foreach (var structMatch in source.Text.StripComments().MatchAll(structTemplate.Regex)) {
                        var structClone = structTemplate.Clone(structMatch);

                        if (context.IsStructWhiteListed(structClone.NativeName)) {
                            structClone.InvokeOnMatched();
                            context.MatchedStructs.Add(structClone);
                        }
                    }
                }

                // classes
                foreach (var classTemplate in context.ClassTemplates) {
                    foreach (var classMatch in source.Text.StripComments().MatchAll(classTemplate.Regex)) {
                        var classClone = classTemplate.Clone(classMatch);

                        if (context.IsClassWhiteListed(classClone.NativeName)) {

                            // Fields

                            foreach (var fieldTempate in context.FieldTemplates) {
                                foreach (var fieldMatch in classClone.NativeBody.MatchAll(fieldTempate.Regex)) {
                                    var fieldClone = fieldTempate.Clone(fieldMatch);
                                    fieldClone.Class = classClone;

                                    if (context.IsFieldIgnored(fieldClone) == false) {
                                        fieldClone.Type = context.MatchType(fieldClone.NativeTypeText);
                                        fieldClone.Type.Field = fieldClone;
                                        classClone.Fields.Add(fieldClone);

                                    } else {
                                        classClone.IgnoredFields.Add(fieldClone.NativeName);
                                    }
                                }
                            }

                            // Methods

                            foreach (var methodTemplate in context.MethodTemplates) {
                                foreach (var methodMatch in classClone.NativeBody.MatchAll(methodTemplate.Regex)) {
                                    var methodClone = methodTemplate.Clone(methodMatch);
                                    methodClone.Class = classClone;

                                    if (context.IsMethodIgnored(methodClone) == false) {
                                        ParseFunction(methodClone, context);
                                        classClone.Methods.Add(methodClone);
                                    } else {
                                        classClone.IgnoredMethods.Add(methodClone.NativeName);
                                    }
                                }
                            }

                            context.MatchedClasses.Add(classClone);
                            classClone.InvokeOnMatched();
                        }
                    }
                }
            }

            foreach (var source in context.ReadUserFiles()) {
                // user functions
                foreach (var funcTemplate in context.UserFunctionTemplates) {
                    foreach (var funcMatch in source.Text.StripComments().MatchAll(funcTemplate.Regex)) {
                        var funcClone = funcTemplate.Clone(funcMatch);
                        context.MatchedUserFunction.Add(funcClone);
                        ParseFunction(funcClone, context);
                    }
                }

                // user delegates
                foreach (var delegateTemplate in context.UserDelegateTemplates) {
                    foreach (var delegateMatch in source.Text.StripComments().MatchAll(delegateTemplate.Regex)) {
                        var delegateClone = delegateTemplate.Clone(delegateMatch);
                        context.MatchedUserDelegates.Add(delegateClone);
                        ParseFunction (delegateClone, context);
                    }
                }
            }
        }

        public static void RaiseOnParsingDone (Context context) {
            foreach (var s in context.MatchedStructs) {
                s.InvokeOnParsingDone();

                foreach (var f in s.Fields) {
                    f.Type.InvokeOnParsingDone();
                }
            }

            foreach (var c in context.MatchedClasses) {
                c.InvokeOnParsingDone();

                foreach (var f in c.Fields) {
                    f.InvokeOnParsingDone();
                    f.Type.InvokeOnParsingDone();
                }

                foreach (var m in c.Methods) {
                    m.InvokeOnParsingDone();
                    m.ReturnType.InvokeOnParsingDone();

                    foreach (var p in m.Parameters) {
                        p.InvokeOnParsingDone();
                        p.Type.InvokeOnParsingDone();
                    }
                }
            }

            foreach (var f in context.MatchedUserFunction) {
                f.InvokeOnParsingDone();
                f.ReturnType.InvokeOnParsingDone();

                foreach (var p in f.Parameters) {
                    p.InvokeOnParsingDone();
                    p.Type.InvokeOnParsingDone();
                }
            }
        }
    }
}
