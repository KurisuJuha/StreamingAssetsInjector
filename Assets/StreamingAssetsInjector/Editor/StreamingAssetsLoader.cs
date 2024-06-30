using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace StreamingAssetsInjector.Editor
{
    public struct SerializedStreamingAsset
    {
        public string Base64;
        public string RelativePath;
    }

    public static class StreamingAssetsLoader
    {
        public static SerializedStreamingAsset[] GetAssets(string folderPath)
        {
            return Directory
                .GetFiles(folderPath, "*", SearchOption.AllDirectories)
                .Select(filePath => GetFileBase64(folderPath, filePath))
                .ToArray();
        }

        private static SerializedStreamingAsset GetFileBase64(string folderPath, string filePath)
        {
            var fileBytes = File.ReadAllBytes(filePath);
            var relativeFilePath = filePath.Remove(0, folderPath.Length + 1);
            relativeFilePath = relativeFilePath.Replace(@"\", "/");
            Debug.Log($"{relativeFilePath}, {string.Join(',', fileBytes)}");

            return new SerializedStreamingAsset
            {
                Base64 = Convert.ToBase64String(fileBytes),
                RelativePath = relativeFilePath
            };
        }
    }
}
