using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace StreamingAssetsInjector.Editor
{
    public class BuildPreProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log($"PreProcess {report.summary.platform} {report.summary.outputPath}");

            if (report.summary.platform == BuildTarget.WebGL)
            {
                var outputPath = report.summary.outputPath;
                var buildPath = Path.Combine(outputPath, "Build");
                var loaderPath = Directory.GetFiles(buildPath, "*.loader.js").First();
                File.Delete(loaderPath);
            }
        }
    }

    public class BuildPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.WebGL)
            {
                var outputPath = report.summary.outputPath;
                var buildPath = Path.Combine(outputPath, "Build");
                var loaderPath = Directory.GetFiles(buildPath, "*.loader.js").First();
                var streamingAssetsPath = Path.Combine(outputPath, "StreamingAssets");

                var rawText = "OverrideFetch();"
                              + File.ReadAllText(loaderPath)
                              + GetStreamingAssetsTable(StreamingAssetsLoader.GetAssets(streamingAssetsPath))
                              + GetOverrideFetchCode();

                File.WriteAllText(loaderPath, rawText);
            }

            Debug.Log($"PostProcess {report.summary.platform} {report.summary.outputPath}");
        }

        private static string GetStreamingAssetsTable(SerializedStreamingAsset[] assets)
        {
            Debug.Log($@"const streamingAssetsTable = {{{
                string.Join(
                    ",",
                    assets.Select(asset => {
                        Debug.Log(asset.RelativePath);
                        return $"\"{asset.RelativePath}\":\"\"";
                    })
                )
            }}};");
            return $@"const streamingAssetsTable = {{{
                string.Join(
                    ",",
                    assets.Select(asset => {
                        Debug.Log(asset.RelativePath);
                        return $"\"{asset.RelativePath}\":\"{asset.Base64}\"";
                    })
                )
            }}};";
        }

        private static string GetOverrideFetchCode()
        {
            return @"function OverrideFetch() {
    console.log(""OverrideFetch called"");

    const originalFetch = window.fetch;
    window.fetch = function (url, options) {
        if (url.startsWith(window.location.origin + ""/StreamingAssets/"")) {
            var relativeUrl = url.replace(window.location.origin, """").replace(""/StreamingAssets/"", """");

            const binaryData = atob(streamingAssetsTable[relativeUrl]);
            const arrayBuffer = new Uint8Array(binaryData.length);

            for (let i = 0; i < binaryData.length; i++) {
                arrayBuffer[i] = binaryData.charCodeAt(i);
            }

            const blob = new Blob([arrayBuffer]);

            return Promise.resolve(
                new Response(blob, {
                    status: 200,
                })
            );
        }

        return originalFetch(url, options);
    };
}
";
        }
    }
}
