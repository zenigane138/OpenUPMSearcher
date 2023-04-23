OpenUPM Searcher
===
The English version of README.md is available [here](/README.md).

概要
---
OpenUPM Searcher は UnityEditor 上で OpenUPM のパッケージ検索機能と ProjecetSettings(manifest.json) への登録機能を提供するエディタ拡張ウィンドウです。  
Node.js や openupm-cli のインストールは不要です。  

![](https://img.shields.io/badge/Unity-2020.1%20or%20later-lightgrey)
[![](https://img.shields.io/badge/license-MIT-orange)](https://github.com/zenigane138/AnythingBookmark/blob/main/LICENSE.md)
[![](https://img.shields.io/badge/readme-English-red)](/README_ja.md)
[![](https://img.shields.io/badge/Follow-FFFFFF.svg?logo=twitter&style=flat)](https://twitter.com/intent/follow?screen_name=zenigane138)

![image](https://user-images.githubusercontent.com/36072156/232230899-52835490-8a8b-4ad8-8c1d-b2d1a5d78a67.png)

機能
---
- OpenUPM のパッケージ名検索
- ProjecetSettings(manifest.json) への登録

動作環境
---
- 必須
  - Unity 2020.1 以降

インストール方法
---
- unitypackage
  - Releases ページから unitypackage をダウンロード
  - https://github.com/zenigane138/OpenUPMSearcher/releases
- PackageManager
  - Window -> PackageManager でウィンドウを開く
  - +▼ボタンから "Add package from git URL..." を選択し下記URLを設定
  - https://github.com/zenigane138/OpenUPMSearcher.git?path=Assets/OpenUPMSearcher

使い方
---
- ウィンドウを Unity のメニューから開いて下さい。
  - OkaneGames -> OpenUPM Searcher
  - Window -> OkaneGames -> OpenUPM Searcher
- "Get PackageList by GitHub API" ボタンを押してパッケージリストの取得。
- "Filter" に検索したいパッケージのパッケージ名を入力。
  - 例: com.okanegames
  - 例: unitask
- 任意のパッケージを "Register" ボタンを押して登録。
  - 登録に成功すればダイアログが表示されるの OK をクリック。
  - manifest.json が開かれるので UnityEditor をアクティブにする。
  - UnityEditor が manifest.json の更新を検知して ProjectSettings に反映する。
- ProjectSettings ウィンドウ内の Package Manager で登録内容を確認する。
- PackageManager ウィンドウを開き "Packages:My Registries" に切り替え、登録されたパッケージをインストール。

License
---
[LICENSE.md](/LICENSE.md) をご確認下さい。
