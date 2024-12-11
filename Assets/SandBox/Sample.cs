using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Networking;

namespace SandBox
{
    public class Sample : MonoBehaviour
    {
        [SerializeField] private Transform _spriteTransform;
        [SerializeField] private RectTransform _textTransform;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TMP_Text _textRenderer;
        [SerializeField] private Ease _ease;
        [SerializeField] private float _duration;
        [SerializeField] private float _spritePosition0;
        [SerializeField] private float _spritePosition1;
        [SerializeField] private float _textPosition0;
        [SerializeField] private float _textPosition1;
        [SerializeField] private float _changeTiming;

        [SerializeField] private string _squareSpritePath;
        [SerializeField] private string _circleSpritePath;

        [SerializeField] private TableReference _tableReference;
        [SerializeField] private Locale _japaneseLocale;
        [SerializeField] private Locale _englishLocale;

        [SerializeField] private float _backgroundChangeTiming;
        [SerializeField] private float _backgroundChangeDuration;
        [SerializeField] private Ease _backgroundChangeEase;
        private readonly CompositeMotionHandle _motionHandles = new();
        private bool _state;

        private async void Start()
        {
            await LocalizationSettings.InitializationOperation.Task;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) Change().Forget();
        }

        private void OnDestroy()
        {
            _motionHandles.Cancel();
        }

        private async UniTaskVoid Change()
        {
            _motionHandles.Cancel();
            _motionHandles.Clear();
            _state = !_state;
            await UniTask.WhenAll(
                ChangeSprite(),
                ChangeLocale(),
                ChangeBackground()
            );
        }

        private async UniTask ChangeBackground()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_changeTiming));
            var motionHandle = LMotion
                .Create(Camera.main!.backgroundColor, _state ? Color.white : Color.black, _backgroundChangeDuration)
                .WithEase(_backgroundChangeEase)
                .Bind(c => Camera.main!.backgroundColor = c)
                .AddTo(this);

            _motionHandles.Add(motionHandle);
        }

        private async UniTask ChangeSprite()
        {
            Sprite sprite;
            if (_state)
            {
                var request =
                    UnityWebRequestTexture.GetTexture($"{Application.streamingAssetsPath}/{_squareSpritePath}");
                await request.SendWebRequest();
                var handler = (DownloadHandlerTexture)request.downloadHandler;
                sprite = Sprite.Create(handler.texture,
                    new Rect(0, 0, handler.texture.width, handler.texture.height), Vector2.one / 2f);
            }
            else
            {
                sprite = await Addressables.LoadAssetAsync<Sprite>(_circleSpritePath);
            }

            var motionHandle = LMotion
                .Create(_spriteTransform.position.y, _state ? _spritePosition0 : _spritePosition1, _duration)
                .WithEase(_ease)
                .BindToPositionY(_spriteTransform)
                .AddTo(this);
            _motionHandles.Add(motionHandle);

            await UniTask.Delay(TimeSpan.FromSeconds(_changeTiming));

            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color = _state ? Color.black : Color.white;
        }

        private async UniTask ChangeLocale()
        {
            var motionHandle = LMotion
                .Create(_textTransform.anchoredPosition.y, _state ? _textPosition0 : _textPosition1, _duration)
                .WithEase(_ease)
                .BindToAnchoredPositionY(_textTransform)
                .AddTo(this);
            _motionHandles.Add(motionHandle);

            await UniTask.Delay(TimeSpan.FromSeconds(_changeTiming));

            var localizedString = new LocalizedString(_tableReference, "greet");
            localizedString.LocaleOverride = _state ? _japaneseLocale : _englishLocale;
            var text = localizedString.GetLocalizedString();
            _textRenderer.text = text;
            _textRenderer.color = _state ? Color.black : Color.white;
        }
    }
}
