using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace SandBox
{
    public class AssetDataBaseTest : MonoBehaviour
    {
        private void Start()
        {
            var filePath = Application.streamingAssetsPath + "/SampleText.txt";

            UnityWebRequest webRequest = UnityWebRequest.Get(filePath);
            webRequest.SendWebRequest();
            while (!webRequest.isDone)
            {
            }

            var text = webRequest.downloadHandler.text;
            var textAsset = new TextAsset(text);
            
            AssetDatabase.CreateAsset(textAsset, Application.streamingAssetsPath + "/CreatedSampleText.txt");
        }
    }
}