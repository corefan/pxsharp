using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxSharp.ApiGen {
    public class SourceFile {
        public string Path;
        public string Text;
    }

    public class Context {
        static string CompilerPath = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\cl.exe";

        public string CppSelfParameterName = "p";
        public string CppExternFieldSetterSuffix = "_Set";
        public string CppExternFieldGetterSuffix = "_Get";
        public string CSharpNamespace = "";
        public string CSharpPInvokeClassName = "PInvoke";
        public string CSharpPInvokeDllName = "";
        public List<string> CSharpUsings = new List<string>();
        public List<Tuple<string, string>> CSharpAliases = new List<Tuple<string, string>>();
        public List<string> CppIncludes = new List<string>();
        public OutputTarget CSharpOutput = new OutputTarget();
        public OutputTarget CppOutput = new OutputTarget();
        public List<Enum> EnumerationTemplates = new List<Enum>();
        public List<Class> ClassTemplates = new List<Class>();
        public List<Struct> StructTemplates = new List<Struct>();
        public List<Field> FieldTemplates = new List<Field>();
        public List<Function> MethodTemplates = new List<Function>();
        public List<Parameter> ParameterTemplates = new List<Parameter>();
        public List<Type> TypeTemplates = new List<Type>();
        public List<string> MethodBlackList = new List<string>();
        public List<string> FieldBlackList = new List<string>();

        public HashSet<string> CppClassWhiteList = new HashSet<string>();
        public HashSet<string> CppStructWhiteList = new HashSet<string>();
        public HashSet<string> CppEnumBlackList = new HashSet<string>();
        public HashSet<string> CppExternWhiteList = new HashSet<string>();

        public List<string> CppHeaderDirs = new List<string>();
        public List<string> CppExternDirs = new List<string>();
        public string CppHeaderFileFilter = "*.h";
        public string CppExternFileFilter = "*.cpp";
        public List<Function> UserFunctionTemplates = new List<Function>();
        public List<Function> UserDelegateTemplates = new List<Function>();

        public List<Class> MatchedClasses = new List<Class>();
        public List<Struct> MatchedStructs = new List<Struct>();
        public List<Function> MatchedUserFunction = new List<Function>();
        public List<Function> MatchedUserDelegates = new List<Function>();

        public IEnumerable<SourceFile> ReadHeaderFiles () {
            foreach (var dir in CppHeaderDirs) {
                foreach (var file in Directory.GetFiles(dir, CppHeaderFileFilter, SearchOption.AllDirectories)) {
                    yield return new SourceFile { Path = file, Text = File.ReadAllText(file) };
                }
            }
        }

        public IEnumerable<SourceFile> ReadUserFiles () {
            foreach (var dir in CppExternDirs) {
                foreach (var file in Directory.GetFiles(dir, CppExternFileFilter, SearchOption.TopDirectoryOnly)) {
                    CppPreProcessor cppPreProcessor = new CppPreProcessor(CompilerPath, new[] {
                        @"C:\physx-3.3\Include",
                        @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\include"
                    }, new[] { "NDEBUG", "PXS_APIGEN" });

                    yield return new SourceFile { Path = file, Text = cppPreProcessor.Run(file) };
                }
            }
        }

        public Type MatchType (string typeString) {
            typeString = typeString.Trim();

            foreach (Type typeTemplate in TypeTemplates) {
                var typeMatch = typeTemplate.Regex.Match(typeString);

                if (typeMatch.Success) {
                    return typeTemplate.Clone(typeMatch);
                }
            }

            throw new System.Exception("Could not match type " + typeString);
        }

        public Type MatchTypeDelayed (string typeString) {
            try {
                return MatchType(typeString);
            } catch {
                return new Type {
                    NativeTypeResolver = t => MatchType(typeString).NativeType,
                    ExternTypeResolver = t => MatchType(typeString).ExternType,
                    PInvokeTypeResolver = t => MatchType(typeString).PInvokeType,
                    ManagedTypeResolver = t => MatchType(typeString).ManagedType,
                    PInvokeParameterAttributesResolver = t => MatchType(typeString).PInvokeParameterAttributesResolver(t),
                    PInvokeReturnAttributesResolver = t => MatchType(typeString).PInvokeReturnAttributesResolver(t),
                    ManagedToPinvokeConverter = (t, expr) => MatchType(typeString).ManagedToPinvokeConverter(t, expr),
                    PInvokeToManagedConverter = (t, expr) => MatchType(typeString).PInvokeToManagedConverter(t, expr),
                    ExternToNativeConverter = (t, expr) => MatchType(typeString).ExternToNativeConverter(t, expr),
                    NativeToExternConverter = (t, expr) => MatchType(typeString).NativeToExternConverter(t, expr),
                };
            }
        }

        public bool IsFieldIgnored (Field field) {
            foreach (string ignoreRegex in FieldBlackList) {
                if (ignoreRegex.Contains("::")) {
                    var parts = ignoreRegex.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    var classMatches = field.Class.NativeName.Matches(parts[0], RegexOptions.IgnoreCase);
                    var methodMatches = field.NativeName.Matches(parts[1], RegexOptions.IgnoreCase);

                    if (classMatches && methodMatches)
                        return true;
                } else {
                    if (field.NativeName.Matches(ignoreRegex, RegexOptions.IgnoreCase)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsMethodIgnored (Function method) {
            foreach (string ignoreRegex in MethodBlackList) {
                if (ignoreRegex.Contains("::")) {
                    var parts = ignoreRegex.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    var classMatches = method.Class.NativeName.Matches(parts[0], RegexOptions.IgnoreCase);
                    var methodMatches = method.NativeName.Matches(parts[1], RegexOptions.IgnoreCase);

                    if (classMatches && methodMatches)
                        return true;
                } else {
                    if (method.NativeName.Matches(ignoreRegex, RegexOptions.IgnoreCase)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsClassWhiteListed (string typename) {
            return CppClassWhiteList.Contains(typename);
        }

        public bool IsStructWhiteListed (string typename) {
            return CppStructWhiteList.Contains(typename);
        }

        public void WriteCSharpUsings (OutputFile file) {
            if (CSharpUsings.Count > 0) {
                foreach (string ns in CSharpUsings) {
                    file.AppendLineFormat("using {0};", ns);
                }
                file.AppendLine();
            }
        }

        public void WriteCSharpAliases (OutputFile file) {
            if (CSharpAliases.Count > 0) {
                foreach (var tup in CSharpAliases) {
                    file.AppendLineFormat("using {0} = {1};", tup.Item1, tup.Item2);
                }
                file.AppendLine();
            }
        }

        public void AddInheritance (string subType, string baseType) {
            FindClass(subType).BaseClass = FindClass(baseType);
            FindClass(baseType).SubClasses.Add(FindClass(subType));
        }

        public bool HasClass (string name) {
            return MatchedClasses.Any(s => s.NativeName == name);
        }

        public Class FindClass (string name) {
            return MatchedClasses.First(c => c.NativeName == name);
        }

        public Struct FindStruct (string name) {
            return MatchedStructs.First(c => c.NativeName == name);
        }

        public Class CreateClass (string name) {
            AddClassTypeTemplate(name);

            var cls = new Class().Clone(null);
            cls.NativeNameResolver = c => name;
            cls.ManagedNameResolver = c => name;

            MatchedClasses.Add(cls);

            return cls;
        }

        public void AddPODTypeTemplate (string typename) {
            // Foo
            TypeTemplates.Add(new Type {
                Regex = new Regex(@"^(const\s+)?" + typename + @"$"),
                ExternTypeResolver = t => typename,
                PInvokeTypeResolver = t => typename,
                ManagedTypeResolver = t => typename,
                PInvokeParameterAttributesResolver = t => "In",
            });

            // Foo*, const Foo*
            TypeTemplates.Add(new Type {
                Regex = new Regex(@"^(const\s+)?" + typename + @"\s*\*$"),
                PInvokeTypeResolver = t => (t.Has(1) ? "" : "ref ") + typename,
                ManagedTypeResolver = t => (t.Has(1) ? "" : "ref ") + typename,
                ManagedToPinvokeConverter = (t, expr) => (t.Has(1) ? "" : "ref ") + expr,
                PInvokeParameterAttributesResolver = t => t.Has(1) ? "In" : "In, Out",
            });

            // Foo&, const Foo&
            TypeTemplates.Add(new Type {
                Regex = new Regex(@"^(const\s+)?" + typename + @"\s*&$"),
                ExternTypeResolver = t => (t.Has(1) ? typename : typename + "*"),
                PInvokeTypeResolver = t => (t.Has(1) ? "" : "ref ") + typename,
                ManagedTypeResolver = t => (t.Has(1) ? "" : "ref ") + typename,
                ExternToNativeConverter = (t, expr) => (t.Has(1) ? expr : "*" + expr),
                ManagedToPinvokeConverter = (t, expr) => (t.Has(1) ? "" : "ref ") + expr,
                PInvokeParameterAttributesResolver = t => t.Has(1) ? "In" : "In, Out",
            });
        }

        public void AddStructTypeTemplate (string typename) {
            // Foo
            TypeTemplates.Add(new Type {
                Regex = new Regex(@"^" + typename + @"(_Managed)?$"),
                NativeTypeResolver = t => typename,
                ExternTypeResolver = t => typename + "_Managed",
                PInvokeTypeResolver = t => typename,
                ManagedTypeResolver = t => typename,
                ExternToNativeConverter = (t, expr) => typename + "_IN(" + expr + ")",
                NativeToExternConverter = (t, expr) => typename + "_OUT(" + expr + ")",
                PInvokeParameterAttributesResolver = t => "In",
            });

            // Foo&, const Foo&
            TypeTemplates.Add(new Type {
                Regex = new Regex(@"^(const\s+)?" + typename + @"(_Managed)?\s*&$"),
                NativeTypeResolver = t => t[1] + " " + typename + "&",
                ExternTypeResolver = t => {
                    if (t.IsReturnType || t.Has(1))
                        return typename + "_Managed";

                    return typename + "_Managed" + "*";
                },
                PInvokeTypeResolver = t => {
                    if (t.IsReturnType || t.Has(1))
                        return typename;

                    return "ref " + typename;
                },
                ManagedTypeResolver = t => t.PInvokeType,
                ManagedToPinvokeConverter = (t, expr) => t.Has(1) ? expr : ("ref " + expr),
                ExternToNativeConverter = (t, expr) => typename + "_IN(" + (t.Has(1) ? "" : "*") + expr + ")",
                NativeToExternConverter = (t, expr) => typename + "_OUT(" + expr + ")",
                PInvokeParameterAttributesResolver = t => t.Has(1) ? "In" : "In, Out",
                OnMatched = t => {
                    // if we are a non-const function parameter 
                    if (t.Has(1) == false && t.Function != null && t.IsParameter) {
                        t.Function.CaptureReturnValue = t.Function.ReturnsVoid == false;
                        t.Function.ExternTempVars.Add(Tuple.Create(typename, t.Parameter.TempName));
                        t.ExternToNativeConverter = (_, expr) => t.Parameter.TempName;
                        t.Function.ExternOutAssigns.Add(t);
                    }
                }
            });

            // Foo*, const Foo*
            TypeTemplates.Add(new Type {
                Regex = new Regex(@"^(const\s+)?" + typename + @"(_Managed)?\s*\*$"),
                NativeTypeResolver = t => t[1] + typename + "*",
                ExternTypeResolver = t => t[1] + typename + "_Managed" + "*",
                PInvokeTypeResolver = t => "ref " + typename,
                ManagedTypeResolver = t => "ref " + typename,
                ExternToNativeConverter = (t, expr) => typename + "_IN(*" + expr + ")",
                NativeToExternConverter = (t, expr) => { throw new NotImplementedException(); },
                ManagedToPinvokeConverter = (t, expr) => t.Has(1) ? expr : ("ref " + expr),
                PInvokeParameterAttributesResolver = t => t.Has(1) ? "In" : "In, Out",
                OnParsingDone = t => {

                    if (t.IsParameter) {
                        t.Function.ExternTempVars.Add(Tuple.Create(typename, t.Parameter.TempName));
                    }

                    return;
                }
            });
        }

        public void AddClassTypeTemplate (string typename) {
            // Foo*, const Foo*
            TypeTemplates.Add(new Type {
                Regex = new Regex(@"^(const\s+)?" + typename + @"\s*\*$"),
                NativeTypeResolver = t => t[1] + " " + typename + "*",
                ExternTypeResolver = t => t[1] + " " + typename + "*",
                PInvokeTypeResolver = t => typename,
                ManagedTypeResolver = t => typename,
                PInvokeParameterAttributesResolver = t => "In",
            });

            // Foo&, const Foo&
            TypeTemplates.Add(new Type {
                Regex = new Regex(@"^(const\s+)?" + typename + @"\s*&$"),
                NativeTypeResolver = t => t[1] + " " + typename + "&",
                ExternTypeResolver = t => t[1] + " " + typename + "*",
                PInvokeTypeResolver = t => typename,
                ManagedTypeResolver = t => typename,
                ExternToNativeConverter = (t, expr) => "*(" + expr + ")",
                NativeToExternConverter = (t, expr) => "&(" + expr + ")",
                PInvokeParameterAttributesResolver = t => "In",
            });

            // Foo*&
            TypeTemplates.Add(new Type {
                Regex = new Regex(@"^" + typename + @"\s*\*\s*&$"),
                ExternTypeResolver = t => t[1] + " " + typename + "**",
                PInvokeTypeResolver = t => "ref " + typename,
                ManagedTypeResolver = t => "ref " + typename,
                ExternToNativeConverter = (t, expr) => "*(" + expr + ")",
                ManagedToPinvokeConverter = (t, expr) => "ref " + expr,
                PInvokeParameterAttributesResolver = t => "In, Out",
            });
        }

        public void AddBufferTypeTemplate (string typename) {
            // Foo**, Foo* const*, const Foo* const*

            TypeTemplates.Add(new Type {
                Regex = new Regex(@"(const\s+)?" + typename + @"\s*\*\s*(?:const)?\s*\*"),
                PInvokeTypeResolver = t => "IntPtr",
                ManagedTypeResolver = t => typename + "[]",
                OnParsingDone = t => {
                    if (t.IsParameter == false)
                        throw new NotImplementedException();

                    var bufferSizeParameter = t.Function.Parameters[t.Parameter.Index + 1];

                    // fix the buffer for the duration of the this call
                    t.Function.ManagedFixedStatements.Add(Tuple.Create(t.Parameter.TempName, t.Parameter.NativeName));

                    // change the managed->pinvoke expression to wrap the pointer in a IntPtr
                    t.Parameter.ManagedToPInvokeExpressionResolver = p => {
                        return "new IntPtr(" + t.Parameter.TempName + ")";
                    };

                    // hide the bufferSize parameter from the managed caller
                    bufferSizeParameter.HideInManaged = true;

                    // and change the expression to return a casted version of the .Length of the buffer array
                    bufferSizeParameter.ManagedToPInvokeExpressionResolver = p => {
                        return "(" + p.Type.NativeType + ") " + t.Parameter.NativeName + ".Length";
                    };

                    return;
                },
            });
        }

        public void AddWhiteListedClass (string typename) {
            CppClassWhiteList.Add(typename);
            AddClassTypeTemplate(typename);
            AddBufferTypeTemplate(typename);
        }

        public void AddWhiteListedStruct (string typename) {
            AddWhiteListedStruct(typename, false);
        }

        public void AddWhiteListedStruct (string typename, bool skipTemplate) {
            CppStructWhiteList.Add(typename);

            if (skipTemplate == false)
                AddStructTypeTemplate(typename);
        }
    }
}
