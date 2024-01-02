using ILRuntime.Mono.Cecil;
using ILRuntime.Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class DelegateGen
{
    [MenuItem("ILRuntime/DelegateGen")]
    public static void Gen()
    {
        var targetPath = PathUtil.EditorDllPath;
        Dictionary<string, MethodReference> map = new Dictionary<string, MethodReference>();

        using (var fs = File.Open(targetPath, FileMode.Open))
        {
            var module = ModuleDefinition.ReadModule(fs);
            foreach (var typeDefinition in module.Types)
            {
                foreach (var methodDefinition in typeDefinition.Methods)
                {
                    if (methodDefinition == null)
                        continue;
                    if (methodDefinition.Body == null)
                        continue;
                    if (methodDefinition.Body.Instructions == null)
                        continue;

                    foreach (var instruction in methodDefinition.Body.Instructions)
                    {
                        if (instruction.OpCode != OpCodes.Newobj || instruction.Previous == null ||
                            instruction.Previous.OpCode != OpCodes.Ldftn) continue;

                        var type = instruction.Operand as MethodReference;
                        if (type == null ||
                            (!type.DeclaringType.Name.Contains("Action") &&
                             !type.DeclaringType.Name.Contains("Func"))
                             ) continue;

                        var typeName = type.DeclaringType.FullName;
                        map[typeName] = type;
                    }
                }
            }
        }

        var ret = "";
        foreach(var kv in map)
        {
            var name = kv.Key;
            var type = kv.Value;

            var args = "";
            foreach(var param in type.DeclaringType.GenericParameters)
            {
                if (string.IsNullOrEmpty(args))
                    args = param.Name;
                else
                    args = param.Name + ",";
            }
            //Debuger.Log(type.DeclaringType, type.DeclaringType.);
            ret += string.Format("app.DelegateManager.RegisterMethodDelegate<{0}>();\r\n", args);
        }
        Debuger.Log("end");
    }
}
