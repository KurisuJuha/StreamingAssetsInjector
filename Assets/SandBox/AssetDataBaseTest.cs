using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

    public class C
    {
        public int test;
    }

    public class AssetDataBaseTest : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            // if (Application.platform != RuntimePlatform.WebGLPlayer) return;
            //
            // var data = StreamingAssetsLoader.LoadStreamingAssetsData();
            // Debug.Log(data);
            // var parsedData = StreamingAssetsUtil.GetData(data);
            //
            // foreach (var kvp in parsedData) Debug.Log($"{kvp.Key}, {string.Join(',', kvp.Value)}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(RequestCoroutine());
            if (Input.GetKeyDown(KeyCode.Return)) AddressablesTest().Forget();
        }

        private async UniTaskVoid AddressablesTest()
        {
            var spriteHandle = Addressables.LoadAssetAsync<Sprite>("Assets/SampleSquare.png");
            var sprite = await spriteHandle;

            _spriteRenderer.sprite = sprite;
        }

        private IEnumerator RequestCoroutine()
        {
            var request = UnityWebRequestTexture.GetTexture(Application.streamingAssetsPath + "/Square.png");
            Debug.Log(request.url);
            var operation = request.SendWebRequest();

            yield return operation;

            Debug.Log(operation.isDone);
            Debug.Log(operation.webRequest.downloadHandler.error);
            Debug.Log(operation.webRequest.downloadHandler.text);

            var data = operation.webRequest.downloadHandler.data;
            Debug.Log(string.Join(", ", data));

            var texture = ((DownloadHandlerTexture)operation.webRequest.downloadHandler).texture;
            _spriteRenderer.sprite =
                Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        private void Test()
        {
            Debug.Log("InjectCode");
        }
    }
}
