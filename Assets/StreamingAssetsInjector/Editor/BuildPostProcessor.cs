using System.IO;
using System.Linq;
using StreamingAssetsInjector.Runtime;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace StreamingAssetsInjector.Editor
{
    public class BuildPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.WebGL)
            {
                var outputPath = report.summary.outputPath;
                var buildPath = Path.Combine(outputPath, "Build");
                var streamingAssetsPath = Path.Combine(outputPath, "StreamingAssets");
                var loaderPath = Directory.GetFiles(buildPath, "*.loader.js").First();
                var data = StreamingAssetsLoader.GetBase64(streamingAssetsPath);
                File.AppendAllText(loaderPath, $"function GetStreamingAssetsData(){{return \"{data}\";}}");
            }

            Debug.Log($"PostProcess {report.summary.platform} {report.summary.outputPath}");
        }
    }
}