using UnityEngine;
using UnityEngine.Networking;

namespace SandBox
{
    public class AssetDataBaseTest : MonoBehaviour
    {
        private void Start()
        {
            var request = UnityWebRequest.Get(Application.streamingAssetsPath + "/SampleTest.txt");
            Debug.Log(request.url);
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
            }

            Debug.Log(operation.isDone);
            Debug.Log(operation.webRequest.downloadHandler.text);
        }
    }
}