using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using StreamingAssetsInjector.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace StreamingAssetsInjector.IlPostProcess
{
    [InitializeOnLoad]
    public static class PostProcessor
    {
        static PostProcessor()
        {
            // UnityEditor再生中は無視する
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            // 注入中に再読み込みされないようにロックする
            EditorApplication.LockReloadAssemblies();
            try
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    try
                    {
                        var assemblyPath = assembly.Location;
                        var currentDirectory = Directory.GetCurrentDirectory();
                        var relativePath = Path.GetRelativePath(currentDirectory, assemblyPath).Replace("\\", "/");
                        var isScriptAssemblies = relativePath.StartsWith("Library/ScriptAssemblies");
                        if (!isScriptAssemblies) continue;
//                        Debug.Log(assembly.Location);
                        Process(assemblyPath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
            }
            finally
            {
                // 必ずアンロックされるように try-finally でくくる
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Process(string assemblyPath)
        {
            using var assemblyDefinition =
                AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { ReadWrite = true });

            //TODO: あとで消す
            if (assemblyDefinition.Name.Name != "SandBox") return;

            // もしAttributeがすでにあるなら変更しない
            if (assemblyDefinition.CustomAttributes.Any(attribute =>
                    attribute.AttributeType.FullName == typeof(PostProcessedAttribute).FullName)) return;

            Process(assemblyDefinition);

            // Attributeをassemblyに追加
            AddPostProcessedAttribute(assemblyDefinition);

            // IL_0001: ldstr        "InjectCode"
            // IL_0006: call         void [UnityEngine.CoreModule]UnityEngine.Debug::Log(object)
            // IL_000b: nop

            // IL_0061: ldstr        "hoge"
            // IL_0066: call         class [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequestAsyncOperation [StreamingAssetsInjector.Runtime]StreamingAssetsInjector.Runtime.StreamingAssetsLoader::GetWebRequestAsyncOperation(string)
            // IL_006b: pop

            assemblyDefinition.Write();
        }

        private static void AddPostProcessedAttribute(AssemblyDefinition assemblyDefinition)
        {
            var processedAttributeMethodReference =
                assemblyDefinition
                    .MainModule
                    .ImportReference(
                        typeof(PostProcessedAttribute)
                            .GetConstructor(Type.EmptyTypes)
                    );
            assemblyDefinition.CustomAttributes.Add(new CustomAttribute(processedAttributeMethodReference));
        }

        private static void Process(AssemblyDefinition assemblyDefinition)
        {
            foreach (var methodDefinition in assemblyDefinition.MainModule.Types.SelectMany(type => type.Methods))
                Process(methodDefinition.Body, assemblyDefinition);
        }

        private static void Process(MethodBody methodBody, AssemblyDefinition assemblyDefinition)
        {
            // IL_0022: ldloc.0      // 'request'
            // IL_0023: callvirt     instance class [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequestAsyncOperation [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequest::SendWebRequest()
            // IL_0028: stloc.1      // operation
            // ↑これを ↓こうしたい
            // IL_0061: ldloc.0      // 'request'
            // IL_0062: call         class [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequestAsyncOperation [StreamingAssetsInjector.Runtime]StreamingAssetsInjector.Runtime.StreamingAssetsLoader::GetWebRequestAsyncOperation(class [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequest)

            // IL_0001: callvirt     instance class [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequestAsyncOperation [UnityEngine.UnityWebRequestModule]UnityEngine.Networking.UnityWebRequest::SendWebRequest()

            for (var i = 0; i < methodBody.Instructions.Count; i++)
            {
                var instruction = methodBody.Instructions[i];

                if (instruction.OpCode != OpCodes.Callvirt) continue;
                if (instruction.Operand.ToString() !=
                    "UnityEngine.Networking.UnityWebRequestAsyncOperation UnityEngine.Networking.UnityWebRequest::SendWebRequest()")
                    continue;

                var ilProcessor = methodBody.GetILProcessor();

                var getWebRequestAsyncOperationMethodReference =
                    assemblyDefinition.MainModule.ImportReference(
                        typeof(StreamingAssetsLoader).GetMethod("GetWebRequestAsyncOperation",
                            new[] { typeof(UnityWebRequest) }));

                ilProcessor.InsertAfter(instruction,
                    Instruction.Create(OpCodes.Call, getWebRequestAsyncOperationMethodReference));
                ilProcessor.RemoveAt(i);
            }
        }
    }
}