using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace StreamingAssetsInjector.Runtime
{
    public static class StreamingAssetsLoader
    {
        public static UnityWebRequestAsyncOperation GetWebRequestAsyncOperation(UnityWebRequest request)
        {
            Debug.Log("CustomWebRequestAsyncOperation");
            return new UnityWebRequestAsyncOperation();
        }

        public static string GetBase64(string folderPath)
        {
            var bytes = new List<byte>();

            foreach (var file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                var fileBytes = File.ReadAllBytes(file);
                var relativeFilePath = file.Remove(0, folderPath.Length + 1);
                var relativeFilePathBytes = Encoding.UTF8.GetBytes(relativeFilePath);
                Debug.Log($"{relativeFilePath}, {string.Join(',', fileBytes)}");
                // ファイルの相対パスのサイズを入れる
                bytes.AddRange(BitConverter.GetBytes(relativeFilePathBytes.Length));
                // ファイルの相対パスを入れる
                bytes.AddRange(relativeFilePathBytes);
                // ファイルの中身のサイズを入れる
                bytes.AddRange(BitConverter.GetBytes(fileBytes.Length));
                // ファイルの中身を入れる
                bytes.AddRange(fileBytes);
            }

            return Convert.ToBase64String(bytes.ToArray());
        }

        public static Dictionary<string, byte[]> GetData(string data)
        {
            var bytes = Convert.FromBase64String(data);
            var dic = new Dictionary<string, byte[]>();

            for (var i = 0; i < bytes.Length;)
            {
                // ファイルの相対パスのサイズを取得
                var relativeFilePathLength = BitConverter.ToInt32(bytes, i);
                i += 4;
                // ファイルの相対パスを取得
                var relativeFilePath = Encoding.UTF8.GetString(bytes, i, relativeFilePathLength);
                i += relativeFilePathLength;
                // ファイルの中身のサイズを取得
                var fileLength = BitConverter.ToInt32(bytes, i);
                i += 4;
                // ファイルの中身を取得
                var fileData = new byte[fileLength];
                Array.Copy(bytes, i, fileData, 0, fileLength);
                i += fileLength;

                dic[relativeFilePath] = fileData;
            }

            return dic;
        }

        [DllImport("__Internal")]
        public static extern string LoadStreamingAssetsData();
    }
}