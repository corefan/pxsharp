using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxSharp.ApiGen {
    class Program {

        const RegexOptions OPTS = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled;

        const string ENUMERATION_REGEX = @"struct\s+(?<typeName>[a-z0-9_]+)\s*\{\s*enum\s*(Enum|Type)\s*{\s*(?<values>[^}]+)\s*};\s*};";
        const string ENUMERATION_VALUE_REGEX = @"(Px_DEPRECATED\s+)?(?<name>[a-z0-9_]+)(\s*=\s*(?<value>[^,}]+))?";
        const RegexOptions ENUMERATION_REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.Singleline;
        const RegexOptions ENUMERATION_VALUE_REGEX_OPTIONS = RegexOptions.IgnoreCase;

        const string FIELD_REGEX = @"[\r\n]+[\s\t]*(?<fieldType>(const\s+)?[a-z0-9:_*]+)(?<!return)\s+(?<fieldName>[a-z0-9_]+)\s*;";
        const string METHOD_REGEX = @"[\r\n]+[\s\t]*(PX_CUDA_CALLABLE\s+)?(PX_PHYSX_COMMON_API\s+)?(PX_INLINE\s+)?(virtual|PX_INLINE|PX_FORCE_INLINE)\s+(?<returnType>(const\s+)?[^\s]+)\s+(?<funcName>[a-z0-9_]+)\s*\((void|(?<argsList>[^)]*))\)\s*";
        const RegexOptions FIELD_REGEX_OPTIONS = RegexOptions.IgnoreCase;
        const RegexOptions METHOD_REGEX_OPTIONS = RegexOptions.IgnoreCase;

        const string TYPE_REGEX_START = @"^(?<const>const\s+)?(?<typeName>[a-z_][a-z0-9_]+)";
        const RegexOptions TYPE_REGEX_OPTIONS = RegexOptions.IgnoreCase;

        static void Main (string[] args) {
            Context context = new Context();

            // C++ config
            context.CppHeaderDirs.Add(@"C:\physx-3.3\Include");
            context.CppExternDirs.Add(@"..\..\..\pxsharp.bridge");
            context.CppHeaderFileFilter = "*.h";
            context.CppExternFileFilter = "*.cpp";
            context.CppOutput.Directory = @"..\..\..\pxsharp.bridge\Generated";
            context.CppIncludes.Add(@"..\Common.h");
            context.CppSelfParameterName = "self";

            // C# config
            context.CSharpOutput.Directory = @"..\..\..\pxsharp\Generated";
            context.CSharpNamespace = "PxSharp";
            context.CSharpPInvokeDllName = "pxsharp.bridge";

            context.CSharpUsings.Add("System");
            context.CSharpUsings.Add("System.Runtime.InteropServices");
            context.CSharpUsings.Add("System.Security");

            // C# aliases to match PhysX types
            context.CSharpAliases.Add(Tuple.Create("PxU8", "System.Byte"));
            context.CSharpAliases.Add(Tuple.Create("PxI8", "System.SByte"));
            context.CSharpAliases.Add(Tuple.Create("PxU16", "System.UInt16"));
            context.CSharpAliases.Add(Tuple.Create("PxI16", "System.Int16"));
            context.CSharpAliases.Add(Tuple.Create("PxU32", "System.UInt32"));
            context.CSharpAliases.Add(Tuple.Create("PxI32", "System.Int32"));
            context.CSharpAliases.Add(Tuple.Create("PxU64", "System.UInt64"));
            context.CSharpAliases.Add(Tuple.Create("PxI64", "System.Int64"));
            context.CSharpAliases.Add(Tuple.Create("PxF32", "System.Single"));
            context.CSharpAliases.Add(Tuple.Create("PxF64", "System.Double"));
            context.CSharpAliases.Add(Tuple.Create("PxReal", "System.Single"));
            context.CSharpAliases.Add(Tuple.Create("PxExtended", "System.Double"));

            // Simple POD types which are simply blit:ed
            context.AddPODTypeTemplate("float");
            context.AddPODTypeTemplate("double");
            context.AddPODTypeTemplate("PxU16");
            context.AddPODTypeTemplate("PxU32");
            context.AddPODTypeTemplate("PxI32");
            context.AddPODTypeTemplate("PxF32");
            context.AddPODTypeTemplate("PxReal");
            context.AddPODTypeTemplate("PxExtended");
            context.AddPODTypeTemplate("PxDominanceGroup");
            context.AddPODTypeTemplate("PxTriangleID");
            context.AddPODTypeTemplate("PxClientID");
            context.AddPODTypeTemplate("PxMaterialTableIndex");
            context.AddPODTypeTemplate("ObstacleHandle");
            context.AddPODTypeTemplate("PxFilterObjectAttributes");
            context.AddPODTypeTemplate("PxBroadPhaseCaps");
            context.AddPODTypeTemplate("PxControllerStats");
            context.AddPODTypeTemplate("PxHullPolygon");
            context.AddPODTypeTemplate("PxsScratchBlock");
            context.AddPODTypeTemplate("PxsActorBuffer");
            context.AddPODTypeTemplate("PxsTriggerPairBuffer");
            context.AddPODTypeTemplate("PxsConstraintInfoBuffer");
            context.AddPODTypeTemplate("PxsContactPairBuffer");
            context.AddPODTypeTemplate("PxsContactPairPointBuffer");

            // Parsing C++ structs into _Managed and C# versions
            context.StructTemplates.Add(new Struct {
                Regex = new Regex(@"
                        # match keyword
                        (struct|class)

                        # match type name
                        \s+(?<typeName>[a-z0-9]+)

                        # match inheritance
                        (\s+:\s+(public\s+)?(?<baseType>[a-z0-9]+))?

                        # match open brace
                        \s+\{

                        # match body
                        (?<typeBody>.+?)

                        # match closing brace
                        \};", OPTS),
                NativeNameResolver = s => s["typeName"],
                ExternNameResolver = s => s.NativeName + "_Managed",
                ManagedNameResolver = s => s.NativeName,
                OnMatched = s => {
                    var body =  s["typeBody"];
                    var fields = body.Strip(@"\{.*?\}", RegexOptions.Singleline).MatchAll(@"
                        #match type
                        (^|[\r\n])\s*(?<isConst>const\s)?\s*(?<fieldType>([a-z0-9_*]+(?:::Enum)?|const\s+void\*))

                        #make sure we dont match 'return var;'
                        (?<!return)\s+

                        # match field name or group of names
                        (
	                        ((?<fieldName>[a-z0-9]+)(\s*\[(?<fieldRank>\d+)\])?)
	                        |
	                        (?<fieldNames>[a-z0-9,\s]+)
                        )\s*;", OPTS).ToArray();

                    foreach (var field in fields) {
                        if (field.HasGroup("fieldNames")) {
                            foreach (var f in field.GroupValue("fieldNames").Split(',')) {
                                s.Fields.Add(new StructField {
                                    Name = f.Trim(),
                                    Type = context.MatchType(field.GroupValue("fieldType"))
                                });
                            }
                        } else {
                            s.Fields.Add(new StructField {
                                Name = field.GroupValue("fieldName"),
                                Type = context.MatchType(field.GroupValue("fieldType")),
                                Rank = field.HasGroup("fieldRank") ? int.Parse(field.GroupValue("fieldRank")) : 0,
                                Const = field.HasGroup("isConst")
                            });

                            if (s.Fields[s.Fields.Count - 1].Rank > 0) {
                                s.Unsafe = true;
                            }
                        }
                    }
                },
                OnParsingDone = s => {
                    if (s.Has("baseType")) {
                        var baseType = s["baseType"];
                        s.Parent = context.FindStruct(baseType);
                        s.Parent.Children.Add(s);
                    }

                    switch (s.NativeName) {
                        case "PxVec3":
                        case "PxQuat":
                            s.SortOrder = -100;
                            break;

                        case "PxBounds3":
                        case "PxTransform":
                            s.SortOrder = -1;
                            break;

                        case "PxCookingParams":
                            s.CustomConstructor = s.NativeName + "(PxTolerancesScale())";
                            break;

                        case "PxSpring":
                        case "PxDominanceGroupPair":
                            s.CustomConstructor = s.NativeName + "(0, 0)";
                            break;

                        case "PxJointLinearLimit":
                            s.CustomConstructor = s.NativeName + "(0, PxSpring(0, 0))";
                            break;

                        case "PxJointLimitCone":
                        case "PxJointLinearLimitPair":
                        case "PxJointAngularLimitPair":
                            s.CustomConstructor = s.NativeName + "(0, 0, PxSpring(0, 0))";
                            break;

                        case "PxJointLimitParameters":
                            s.GenerateNativeConverters = false;
                            break;


                        case "PxActiveTransform":
                            s.Fields.Add(new StructField { Name = "actorType", Type = context.MatchType("PxConcreteType::Enum"), Convert = false });
                            break;
                    }

                    return;
                }
            });

            context.AddWhiteListedStruct("PxVec2");
            context.AddWhiteListedStruct("PxVec3");
            context.AddWhiteListedStruct("PxVec4");
            context.AddWhiteListedStruct("PxQuat");
            context.AddWhiteListedStruct("PxMat33");
            context.AddWhiteListedStruct("PxBounds3");
            context.AddWhiteListedStruct("PxTransform");
            context.AddWhiteListedStruct("PxMeshScale");
            context.AddWhiteListedStruct("PxFilterData");
            context.AddWhiteListedStruct("PxQueryFilterData");
            context.AddWhiteListedStruct("PxSceneLimits");
            context.AddWhiteListedStruct("PxCookingParams");
            context.AddWhiteListedStruct("PxExtendedVec3");
            context.AddWhiteListedStruct("PxTolerancesScale");
            context.AddWhiteListedStruct("PxDominanceGroupPair");
            context.AddWhiteListedStruct("PxActiveTransform", true);
            context.AddWhiteListedStruct("PxControllerState");
            context.AddWhiteListedStruct("PxStridedData");
            context.AddWhiteListedStruct("PxGroupsMask");
            context.AddWhiteListedStruct("PxBroadPhaseRegion");
            context.AddWhiteListedStruct("PxBroadPhaseRegionInfo");
            context.AddWhiteListedStruct("PxTriggerPair");
            context.AddWhiteListedStruct("PxConstraintInfo");
            context.AddWhiteListedStruct("PxContactPairHeader");
            context.AddWhiteListedStruct("PxContactPair");
            context.AddWhiteListedStruct("PxContactPairPoint");

            context.AddWhiteListedStruct("PxJointLimitParameters");
            context.AddWhiteListedStruct("PxJointLimitCone");
            context.AddWhiteListedStruct("PxJointLinearLimit");
            context.AddWhiteListedStruct("PxJointLinearLimitPair");
            context.AddWhiteListedStruct("PxJointAngularLimitPair");
            context.AddWhiteListedStruct("PxSpring");
            context.AddWhiteListedStruct("PxD6JointDrive");

            context.AddWhiteListedStruct("PxActorShape");
            context.AddWhiteListedStruct("PxQueryHit");
            context.AddWhiteListedStruct("PxLocationHit");
            context.AddWhiteListedStruct("PxRaycastHit");
            context.AddWhiteListedStruct("PxOverlapHit");
            context.AddWhiteListedStruct("PxSweepHit");

            context.ClassTemplates.Add(new Class {
                Regex = new Regex(@"
                        # match keyword                    
                        class

                        # match two compiler flags from PhysX
                        (\s+(PX_PHYSX_CHARACTER_API|PX_FOUNDATION_API))?

                        # match the typename itself
                        \s+(?<typeName>[a-z0-9_]+)

                        # match inheritance
                        (\s+:\s+(public\s+)?(?<baseType>[a-z0-9]+))?

                        # opening brace
                        \s*\{

                        # the class body
                        (?<typeBody>.+?)

                        # closing brace
                        (?<!{)\};
                        ", OPTS),
                NativeNameResolver = c => c.Match.GroupValue("typeName"),
                NativeBodyTextResolver = c => c.Match.GroupValue("typeBody").Strip(@"(protected|private):.*?(public:|$)", RegexOptions.Singleline),
                OnParsingDone = c => {
                    try {
                        var baseType = c["baseType"];
                        context.AddInheritance(c.NativeName, baseType);
                    } catch { }

                    return;
                }
            });

            // classes to export APIs for
            context.AddWhiteListedClass("PxShape");
            context.AddWhiteListedClass("PxMaterial");
            context.AddWhiteListedClass("PxActor");
            context.AddWhiteListedClass("PxRigidActor");
            context.AddWhiteListedClass("PxRigidBody");
            context.AddWhiteListedClass("PxRigidStatic");
            context.AddWhiteListedClass("PxRigidDynamic");
            //context.AddWhiteListedClass("PxCooking");
            context.AddWhiteListedClass("PxJoint");
            context.AddWhiteListedClass("PxSphericalJoint");
            context.AddWhiteListedClass("PxFixedJoint");
            context.AddWhiteListedClass("PxPrismaticJoint");
            context.AddWhiteListedClass("PxRevoluteJoint");
            context.AddWhiteListedClass("PxDistanceJoint");
            context.AddWhiteListedClass("PxD6Joint");
            context.AddWhiteListedClass("PxAggregate");
            context.AddWhiteListedClass("PxConstraint");
            context.AddWhiteListedClass("PxArticulation");
            context.AddWhiteListedClass("PxArticulationLink");
            context.AddWhiteListedClass("PxPlane");
            context.AddWhiteListedClass("PxScene");
            context.AddWhiteListedClass("PxSceneDesc");
            context.AddWhiteListedClass("PxPhysics");
            context.AddWhiteListedClass("PxFoundation");
            context.AddWhiteListedClass("PxInputStream");
            context.AddWhiteListedClass("PxOutputStream");
            context.AddWhiteListedClass("PxController");
            context.AddWhiteListedClass("PxControllerManager");
            context.AddWhiteListedClass("PxControllerDesc");
            context.AddWhiteListedClass("PxCapsuleControllerDesc");
            context.AddWhiteListedClass("PxBoxControllerDesc");
            context.AddWhiteListedClass("PxGeometry");
            context.AddWhiteListedClass("PxBoxGeometry");
            context.AddWhiteListedClass("PxSphereGeometry");
            context.AddWhiteListedClass("PxCapsuleGeometry");
            context.AddWhiteListedClass("PxPlaneGeometry");
            context.AddWhiteListedClass("PxConvexMeshGeometry");
            context.AddWhiteListedClass("PxTriangleMeshGeometry");
            context.AddWhiteListedClass("PxHeightFieldGeometry");
            context.AddWhiteListedClass("PxObstacle");
            context.AddWhiteListedClass("PxBoxObstacle");
            context.AddWhiteListedClass("PxCapsuleObstacle");
            context.AddWhiteListedClass("PxObstacleContext");
            context.AddWhiteListedClass("PxConvexMesh");
            context.AddWhiteListedClass("PxHeightField");
            context.AddWhiteListedClass("PxHeightFieldDesc");
            context.AddWhiteListedClass("PxTriangleMesh");

            // additional classes which should exist
            context.CreateClass("PxArticulationDriveCache");
            context.CreateClass("PxsErrorCallback").BaseClass = context.CreateClass("PxErrorCallback");
            context.CreateClass("PxsBroadPhaseCallback").BaseClass = context.CreateClass("PxBroadPhaseCallback");
            context.CreateClass("PxsControllerFilterCallback").BaseClass = context.CreateClass("PxControllerFilterCallback");
            context.CreateClass("PxsSimulationEventCallback").BaseClass = context.CreateClass("PxSimulationEventCallback");
            context.CreateClass("PxsQueryFilterCallback").BaseClass = context.CreateClass("PxQueryFilterCallback");
            context.CreateClass("PxsRaycastFilterCallback").BaseClass = context.FindClass("PxQueryFilterCallback");
            context.CreateClass("PxsSweepFilterCallback").BaseClass = context.FindClass("PxQueryFilterCallback");

            // ignore list for methods

            // these functions are implemented by hand in pxsharp.bridge
            context.MethodBlackList.Add("PxScene::getBroadPhaseRegions");
            context.MethodBlackList.Add("PxScene::getActiveTransforms");

            // these methods are ignored and wont be exposed to C#
            context.MethodBlackList.Add("PxScene::(solve|collide|addCollection|createBatchQuery|getRenderBuffer|getFilterShader|setFilterShader)");
            context.MethodBlackList.Add("PxShape::(getGeometry)");
            context.MethodBlackList.Add("PxController::(move)");
            context.MethodBlackList.Add("PxControllerManager::(getRenderBuffer|computeInteractions)");
            context.MethodBlackList.Add("PxFoundation::(getAllocator)");
            context.MethodBlackList.Add(".+::(simulate|getTaskManager|sweep|raycast|overlap|getSimulationStatistics)");
            context.MethodBlackList.Add("PxConstraint::(setConstraintFunctions)");
            context.MethodBlackList.Add("Px(Triangle|Convex)Mesh::(getTrianglesRemap|getVertices|getIndexBuffer)");
            //context.MethodBlackList.Add(".+::(c|C)allback");
            //context.MethodBlackList.Add("PxArticulation::(.+DriveCache|computeImpulseResponse|applyImpulse)");
            context.MethodBlackList.Add("PxPhysics::(createHeightField|createClothFabric|registerDeletionListener|getProfileZoneManager|registerClass|createUserReference|createCollection|addCollection|createParticleSystem|createParticleFluid|createCloth|getClothFabrics|createConstraint)");

            // ignore list for fields
            //context.FieldBlackList.Add(".+::(c|C)allback");
            context.FieldBlackList.Add("PxSceneDesc::filterShader");
            context.FieldBlackList.Add("PxScene::buf");

            // template for c++ enumerations
            context.EnumerationTemplates.Add(new Enum {
                Regex = new Regex(ENUMERATION_REGEX, ENUMERATION_REGEX_OPTIONS),
                ManagedNameParser = f => f.Match.GroupValue("typeName"),
                ValuesParser = f => f.Match.GroupValue("values").StripComments().Trim().MatchAll(ENUMERATION_VALUE_REGEX, ENUMERATION_VALUE_REGEX_OPTIONS).Select(m => {
                    return new EnumValue { CharacterOffset = m.Index, Name = m.GroupValue("name"), Value = m.GroupValue("value") };
                }),
                AttributeWriter = (f, file) => {
                    if (f.ManagedName.Matches("Flags?")) {
                        file.AppendLine("[System.Flags]");
                    }
                }
            });

            // template for c++ fields
            context.FieldTemplates.Add(new Field {
                Regex = new Regex(FIELD_REGEX, FIELD_REGEX_OPTIONS),
                NativeNameResolver = f => f.Match.GroupValue("fieldName"),
                ExternNameResolver = f => f.Class.ManagedName + "_" + f.NativeName.UpperCaseFirst(),
                NativeTypeTextResolver = f => f.Match.GroupValue("fieldType"),
                ManagedNameResolver = f => f.NativeName[0] == 'm' && Char.IsUpper(f.NativeName[1]) ? f.NativeName.Substring(1).UpperCaseFirst() : f.NativeName.UpperCaseFirst()
            });

            // template for c++ methods
            context.MethodTemplates.Add(new Function {
                Regex = new Regex(METHOD_REGEX, METHOD_REGEX_OPTIONS),
                NativeNameResolver = m => m.Match.GroupValue("funcName"),
                ExternNameResolver = m => {
                    var otherMethods = m.Class.AllMethods.Where(m2 => m2.NativeName == m.NativeName);
                    var methodName = m.Class.ManagedName + "_" + m.NativeName.UpperCaseFirst();

                    if (otherMethods.Count() > 1) {
                        methodName += "_" + otherMethods.OrderBy(x => x.ArgumentListText).ToList().IndexOf(m);
                    }

                    return methodName;
                },
                ReturnTypeTextResolver = m => m.Match.GroupValue("returnType"),
                ArgumentListTextResolver = m => m.Match.GroupValue("argsList"),
                ManagedNameResolver = m => {
                    // Handle conflict with .net GetType method
                    if (m.NativeName == "getType")
                        return "GetTypePx";

                    return m.NativeName.UpperCaseFirst().Strip(@"_\d+$");
                }
            });

            // template for c++ user defined functions to export
            context.UserFunctionTemplates.Add(new Function {
                Regex = new Regex(@"EXPORT_FUNC\s+([^\s]+)\s+([^\s(]+)\(([^)]*)\)", RegexOptions.IgnoreCase | RegexOptions.Singleline),
                NativeNameResolver = f => f[2],
                ExternNameResolver = f => f.NativeName,
                ManagedNameResolver = f => f.ExternName.Strip("^[a-z0-9]+_", RegexOptions.IgnoreCase).Strip(@"_\d+$"),
                ReturnTypeTextResolver = f => f[1],
                ArgumentListTextResolver = f => f[3],
                EmitExtern = false,
                OnMatched = f => {
                    var className = f.NativeName.Grab("^[a-z0-9]+(?=_)");

                    if (className.IsAny("Px", "Pxs", "PxRigidbodyExt", "PxShapeExt", "PxVisualDebuggerExt")) {
                        f.IsStatic = true;
                        f.StaticClassName = className;

                    } else {
                        if (f.IsStatic = f.NativeName.Matches(@"_New\d?$")) {
                            f.ManagedNameResolver = _ => "New";
                        } else {
                            if (f.Parameters.Count > 0 && f.Parameters[0].NativeName == context.CppSelfParameterName)
                                f.Parameters.RemoveAt(0);
                        }

                        if (context.HasClass(className) == false) {
                            context.CreateClass(className);
                        }

                        f.Class = context.FindClass(className);
                        f.Class.Methods.Add(f);

                        context.MatchedUserFunction.Remove(f);

                        if (f.ManagedName == "Delete" || f.ManagedName == "Release") {
                            f.Class.ReleaseMethod = f;
                        }
                    }
                }
            });

            context.UserDelegateTemplates.Add(new Function {
                Regex = new Regex(@"DELEGATE_FUNC\s+(?<returnType>[^(]+)\s*\(\s*_stdcall\s*\*\s*(?<delegateName>[^)]+)\s*\)\s*\((?<argsList>[^)]*)\);"),
                EmitExtern = false,
                EmitPInvoke = false,
                EmitManaged = true,
                NativeNameResolver = f => f["delegateName"],
                ManagedNameResolver = f => f.NativeName,
                ReturnTypeTextResolver = f => f["returnType"],
                ArgumentListTextResolver = f => f["argsList"],
                OnMatched = f => {
                    if (context.MatchedUserDelegates.Count(f2 => f2.NativeName == f.NativeName) == 1)
                        context.AddPODTypeTemplate(f.NativeName);
                }
            });

            // template for c++ parameters for functions/methods
            context.ParameterTemplates.Add(new Parameter {
                Regex = new Regex(@"[^,$]+"),
                NativeNameResolver = ParameterNativeNameResolver,
                TypeTextResolver = ParameterTypeTextResolver,
            });

            // "Flags" types which wraps enums on the C++ side, strip and cast into enums
            context.TypeTemplates.Add(new Type {
                Regex = new Regex("([a-z_][a-z0-9_]*)Flags", RegexOptions.IgnoreCase),
                ExternToNativeConverter = (t, expr) => string.Format("({0}) (PxU32) {1}", t.Match.Value.Trim(), expr),
                NativeToExternConverter = (t, expr) => string.Format("({0}::Enum) (PxU32) {1}", t.Match.Value.Trim().RemoveEnd(1), expr),
                ExternTypeResolver = (t) => t.Match.Value.Trim().RemoveEnd(1) + "::Enum",
                PInvokeTypeResolver = (t) => t.Match.Value.Trim().RemoveEnd(1),
                ManagedTypeResolver = (t) => t.PInvokeType
            });

            // Struct::Enum types which are enums on the C++ side, strip out ::Enum part for managed size
            context.TypeTemplates.Add(new Type {
                Regex = new Regex(TYPE_REGEX_START + @"::Enum$", TYPE_REGEX_OPTIONS),
                PInvokeTypeResolver = t => t.Match.Value.Strip("::Enum"),
                ManagedTypeResolver = t => t.PInvokeType
            });

            // template for ANSI C-strings that are to be mapped to strings on the C# side
            context.TypeTemplates.Add(new Type {
                Regex = new Regex(@"(const\s+)?char\s*\*"),
                ExternTypeResolver = t => t.Match.Value.Trim(),
                PInvokeTypeResolver = t => "string",
                ManagedTypeResolver = t => "string",
                PInvokeParameterAttributesResolver = t => "In, MarshalAs(UnmanagedType.LPStr)",
                PInvokeReturnAttributesResolver = t => "MarshalAs(UnmanagedType.LPStr)",
            });

            // mapping of void* to IntPtr
            context.TypeTemplates.Add(new Type {
                Regex = new Regex(@"(const\s+)?void\s*\*"),
                ExternTypeResolver = t => t.Match.Value.Trim(),
                PInvokeTypeResolver = t => "IntPtr",
                ManagedTypeResolver = t => "IntPtr",
            });

            // mapping of enum pointers
            context.TypeTemplates.Add(new Type {
                Regex = new Regex(@"([a-z0-9_]+)::Enum\s*\*", RegexOptions.IgnoreCase),
                ExternTypeResolver = t => t[0],
                PInvokeTypeResolver = t => "ref " + t[1],
                ManagedTypeResolver = t => "ref " + t[1],
                ManagedToPinvokeConverter = (t, expr) => "ref " + expr
            });

            // mapping of enum refs
            context.TypeTemplates.Add(new Type {
                Regex = new Regex(@"(([a-z0-9_]+)::Enum)\s*&", RegexOptions.IgnoreCase),
                ExternTypeResolver = t => t[1] + "*",
                PInvokeTypeResolver = t => "ref " + t[2],
                ManagedTypeResolver = t => "ref " + t[2],
                ExternToNativeConverter = (t, expr) => "*(" + expr + ")",
                ManagedToPinvokeConverter = (t, expr) => "ref " + expr
            });

            context.TypeTemplates.Add(new Type {
                Regex = new Regex(@"^(?:const\s+)?(void|bool|PxU32|PxSharpErrorDelegate|PxI32|PxF32|PxReal|PxSimulationFilterShader)$", TYPE_REGEX_OPTIONS),
                ExternTypeResolver = t => t[0],
                PInvokeTypeResolver = t => t[1],
                ManagedTypeResolver = t => t[1]
            });

            // generic pointer type, marshal as IntPtr
            context.TypeTemplates.Add(new Type {
                Regex = new Regex(TYPE_REGEX_START + @"\s*\*$", TYPE_REGEX_OPTIONS),
                PInvokeTypeResolver = t => {
                    return "IntPtr";
                },
                ManagedTypeResolver = t => t.PInvokeType,
            });


            CodeParser.Parse(context);
            CodeParser.RaiseOnParsingDone(context);

            EnumGenerator.Generate(context);
            StructGenerator.Generate(context);
            ExternGenerator.Generate(context);
            PInvokeGenerator.Generate(context);
            ManagedGenerator.Generate(context);
        }

        static string ParameterStripDefaultValue (Parameter p) {
            string val = p.MatchValue;

            if (val.Contains("=")) {
                val = val.Substring(0, val.IndexOf('=')).Trim();
            }

            return val;
        }

        static string ParameterNativeNameResolver (Parameter p) {
            string val = ParameterStripDefaultValue(p);

            if (val.ContainsAny(" ", "*", "&") && !val.EndsWith("&")) {
                return Regex.Match(val, "[a-z0-9_]+$", RegexOptions.IgnoreCase).Value.Trim();
            }

            return "arg" + p.Index;
        }

        static string ParameterTypeTextResolver (Parameter p) {
            string val = ParameterStripDefaultValue(p);

            if (val.ContainsAny(" ", "*", "&") && !val.EndsWith("&")) {
                return val.Substring(0, val.Length - p.NativeName.Length).Trim();
            }

            return val;
        }
    }
}
