# StreamingAssetsInjector

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)

WebGL ビルドで StreamingAssets フォルダをアップロードできない環境でも、サーバー不要で StreamingAssets を読み込めます

## Features

-   Addressables、Localization など StreamingAssets を利用するほとんどのライブラリを利用することができます
-   インストールするだけで動作します

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

## Sample

[Sample](https://unityroom.com/games/streamingassetsimportertest)

## License

[LICENSE](https://github.com/KurisuJuha/StreamingAssetsInjector/blob/main/LICENSE)
