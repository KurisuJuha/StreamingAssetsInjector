using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SandBox
{
    public class A
    {
        public int test;
    }

    public class B : A
    {
        public int test;
    }

    public class AssetDataBaseTest : MonoBehaviour
    {
        private void Start()
        {
            // if (Application.platform != RuntimePlatform.WebGLPlayer) return;
            //
            // var data = StreamingAssetsLoader.LoadStreamingAssetsData();
            // Debug.Log(data);
            // var parsedData = StreamingAssetsUtil.GetData(data);
            //
            // foreach (var kvp in parsedData) Debug.Log($"{kvp.Key}, {string.Join(',', kvp.Value)}");

            var request = UnityWebRequest.Get(Application.streamingAssetsPath + "/SampleTest.txt");
            Debug.Log(request.url);
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
            }

            Debug.Log(operation.isDone);
            Debug.Log(operation.webRequest.downloadHandler.text);

            //request.SendWebRequest();
            Func<UnityWebRequest, UnityWebRequestAsyncOperation> hoge = req => req.SendWebRequest();

            hoge.Invoke(request);
        }

        private void Test()
        {
            Debug.Log("InjectCode");
        }
    }
}