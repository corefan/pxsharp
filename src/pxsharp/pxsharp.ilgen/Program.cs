using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace pxsharp.ilgen {
    class Program {
        static TypeDefinition obsdef;
        static AssemblyDefinition asm;

        static void Main (string[] args) {
            asm = AssemblyDefinition.ReadAssembly("pxsharp.dll");
            var mscor = AssemblyDefinition.ReadAssembly(@"C:\Program Files (x86)\Unity\Editor\Data\MonoBleedingEdge\lib\mono\2.0\mscorlib.dll");
            obsdef = mscor.MainModule.GetType("System.ObsoleteAttribute");
            var geometry = asm.MainModule.GetType("PxSharp.PxGeometry");

        
            AddEmptyConstructor(geometry, asm.MainModule);

            asm.Write("pxsharp2.dll");
        }

        static void AddEmptyConstructor (TypeDefinition type, ModuleDefinition def) {
            
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var method = new MethodDefinition(".ctor", methodAttributes, def.TypeSystem.Void);

            var obsctor = obsdef.Methods[0];
            method.CustomAttributes.Add(new CustomAttribute(asm.MainModule.Import(obsctor)));
            
            var exn = def.GetType("PxSharp.PxsException");
            var exnCtor = exn.Methods[0];
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Newobj, exnCtor));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Throw));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            type.Methods.Add(method);
        }
    }
}
