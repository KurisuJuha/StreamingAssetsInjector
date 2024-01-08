using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;

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
                var targetAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .First(asm => asm.GetName().Name == "UnityEngine.UnityWebRequestModule");
                Process(targetAssembly);
            }
            finally
            {
                // 必ずアンロックされるように try-finally でくくる
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Process(Assembly targetAssembly)
        {
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(targetAssembly.Location);
            var moduleDefinition = assemblyDefinition.MainModule;
            var targetTypeDefinition = moduleDefinition.Types.First(type => type.Name == "UnityWebRequest");
            var methodDefinition = targetTypeDefinition.Methods.First(method => method.Name == "SendWebRequest");
            Debug.Log(methodDefinition.Name);
        }
    }
}