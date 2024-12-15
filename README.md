# StreamingAssetsInjector

WebGLBuild で StreamingAssets フォルダがアップロードできない環境でも StreamingAssets の読み込みを可能にします

## Installation

PackageManger の Add package from git URL に以下を入力

```
https://github.com/KurisuJuha/StreamingAssetsInjector.git?path=Assets/StreamingAssetsInjector
```

または manifest.json の dependencies ブロックに以下を追加

```
"jp.juha.streaming-assets-injector": "https://github.com/KurisuJuha/StreamingAssetsInjector.git?path=Assets/StreamingAssetsInjector"
```

## Usage

上の手順からインストールした状態で、WebGL 環境ビルドを行うことで動作します

> [!NOTE]
> unityroom アップロードでのみ動作確認を行いました（mac,windows からのビルドは確認済みです）

> [!WARNING]
> StreamingAssets 上の全てのデータがメモリ上に展開されます
> パフォーマンス目的で StreamingAssets を利用しようとしている場合は悪化する可能性があります
> Addressables や Localization の機能自体の利用を目的としている場合のみ使用してください
