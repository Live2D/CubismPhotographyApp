# CubismPhotographyApp

## 概要

このプロジェクトは現実のカメラ映像と合わせて Live2D Cubism 4 Editor で出力したモデルを一体表示するプロジェクトです。 ポストプロセスと組み合わせて、エフェクトを適用した状態も作成することが可能です。

Cubism SDK for Unity と組み合わせて使用します。

また、Unity Remote 5との連携を前提として作成されています。 ビルドさせての動作につきましては確認しておりませんのであらかじめご了承ください。

Unity Remote 5について詳しくは下記リンク先をご確認ください。
https://docs.unity3d.com/ja/2019.4/Manual/UnityRemote5.html

利用には事前に下記のダウンロードが必要です

* Unity Hub
* Unity（推奨:2019 LTS）
    * Unity Remote 5を利用する端末のOSのプラットフォームモジュール
* SDK for Unity


## Cubism SDK for Unity

モデルを表示、操作するための各種機能を提供します。

本プロジェクトには Live2D Cubism Core for Unity が含まれておりません。

Live2D 公式ホームページの Cubism SDK ダウンロードページよりダウンロードし、プロジェクトへインポートの上ご利用ください。

[Cubism SDK](https://www.live2d.com/download/cubism-sdk/)


## SDKマニュアル・チュートリアル

[Cubism SDK Manual](https://docs.live2d.com/cubism-sdk-manual/top/)

[Cubism SDK Tutorial](https://docs.live2d.com/cubism-sdk-tutorials/top/)

## ライセンス

本 SDK を使用する前に [ライセンス](Assets/Live2D/Cubism/LICENSE.md) をご確認ください。


## 注意事項

本 SDK を使用する前に [注意事項](Assets/Live2D/Cubism/NOTICE.md) をご確認ください。

## ブランチ
Git上には3つのブランチを用意しています

### develop ブランチ
2021年12月18日に開催したCubism SDKワークショップ #2の解答部分を含めたフルプロジェクトのブランチです。
すぐにアプリケーションを実行させたい、動作を体験したいという方向けです。

### for-workshop ブランチ
2021年12月18日に開催したCubism SDKワークショップ #2で実際に利用したプロジェクトのブランチです。
解答部分が削除されているため、 Cubism SDK ワークショップの問題を解きたい、もしくは実装の練習がしたいという方向けです。

### reviced-version ブランチ
develop ブランチを一部改変したフルプロジェクトのブランチです。
移動の操作に当たり判定を必要とせず、2本指で移動する仕様へ変更したバージョンとなります。


## ディレクトリ構成

Assets以下のディレクトリの構成

```
Assets
├─ Live2D           # Live2D Cubism SDK for Unity が含まれるディレクトリ
├─ Scenes           # シーンとシーン用のポストプロセスプロファイルが含まれるディレクトリ
├─ Scripts          # プロジェクト用のスクリプトファイルが含まれるディレクトリ
└─ Textures         # UI用のテクスチャが含まれるディレクトリ
```


## プロジェクトの利用方法

### プラットフォームの切り替え

- `BuildSettings` から Unity Remote 5を利用する端末のプラットフォームへ切り替えてください
    - 切り替え方法については [Unity公式のマニュアルページ](https://docs.unity3d.com/ja/2018.4/Manual/BuildSettings.html) をご確認ください

### モデルの導入方法

1. モデルの組み込みファイルの入ったフォルダをUnityのプロジェクトウィンドウへドラッグアンドドロップする
2. `Assets/Scenes` 以下のシーン `SampleScene.unity` を開く
3.  1.で生成されたプレハブをEventSystemオブジェクトの `Live2DModelManager` コンポーネント内にある `Live2Dモデル` ラベルの場所にアタッチする

### モデルの当たり判定の設定方法

Cubism SDK 公式チュートリアルページの `SDK for Unity/コンポーネントの使い方/当たり判定の設定` の手順を参考にしてください。

[Cubism SDK 公式チュートリアル 当たり判定の設定](https://docs.live2d.com/cubism-sdk-tutorials/hittest/)


## 本プロジェクトを利用したアプリケーション開発について

本プロジェクトには `Cubism SDK for Unity` が使用されております。

したがって、本プロジェクトを利用してアプリケーションをリリースする場合には出版許諾契約等の各種契約が必要となります。

詳しくは下記リンク先をご確認ください。

[Cubism SDK リリースライセンス](https://www.live2d.com/download/cubism-sdk/release-license/)