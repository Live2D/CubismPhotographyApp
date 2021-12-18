# CubismPhotographyApp

## 概要

このプロジェクトは現実のカメラ映像と合わせて任意のモデルを一体表示するプロジェクトです。
ポストプロセスと組み合わせて、エフェクトを適用した状態も作成することが可能です。

なお、このプロジェクトはUnity Remote 5との連携を前提として作成されています。ビルドさせての動作につきましては確認しておりませんのであらかじめご了承ください。
Unity Remote 5について詳しくは下記リンク先をご確認ください。
https://docs.unity3d.com/ja/2019.4/Manual/UnityRemote5.html

このプロジェクトを利用するには `Cubism Core for Unity` が必要となります。
Live2D 公式ホームページの Cubism SDK ダウンロードページより `Cubism SDK for Unity` をダウンロードし、プロジェクトへインポートの上ご利用ください。

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

[Cubism SDK 公式チュートリアル 当たり判定の設定](https://docs.live2d.com/cubism-sdk-tutorials/hittest/#)

## 当プロジェクトを利用したアプリケーション開発について

当プロジェクトには `Cubism SDK for Unity` が使用されております。
したがって、当プロジェクトを利用してアプリケーションをリリースする場合には出版許諾契約等の各種契約が必要となります。
詳しくは下記リンク先をご確認ください。

[Cubism SDK リリースライセンス](https://www.live2d.com/download/cubism-sdk/release-license/)