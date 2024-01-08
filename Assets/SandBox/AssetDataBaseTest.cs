using StreamingAssetsInjector.Runtime;
using UnityEngine;

namespace SandBox
{
    public class AssetDataBaseTest : MonoBehaviour
    {
        private void Start()
        {
            var data = StreamingAssetsLoader.LoadStreamingAssetsData();
            Debug.Log(data);
            var parsedData = StreamingAssetsUtil.GetData(data);

            foreach (var kvp in parsedData) Debug.Log($"{kvp.Key}, {string.Join(',', kvp.Value)}");

            // var request = UnityWebRequest.Get(Application.streamingAssetsPath + "/SampleTest.txt");
            // Debug.Log(request.url);
            // var operation = request.SendWebRequest();
            // while (!operation.isDone)
            // {
            // }
            //
            // Debug.Log(operation.isDone);
            // Debug.Log(operation.webRequest.downloadHandler.text);
        }
    }
}