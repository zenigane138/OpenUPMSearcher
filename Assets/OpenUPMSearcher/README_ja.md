OpenUPM Searcher
===
The English version of README.md is available [here](/README.md).

概要
---
OpenUPM Searcher は UnityEditor 上で OpenUPM のパッケージ検索機能と ProjecetSettings(manifest.json) への登録機能を提供するエディタ拡張ウィンドウです。  
Node.js や openupm-cli のインストールは不要です。  

![](https://img.shields.io/badge/Unity-2021.2%20or%20later-lightgrey)
[![](https://img.shields.io/badge/license-MIT-orange)](https://github.com/zenigane138/AnythingBookmark/blob/main/LICENSE.md)
[![](https://img.shields.io/badge/readme-English-red)](/README_ja.md)
[![](https://img.shields.io/badge/Follow-FFFFFF.svg?logo=twitter&style=flat)](https://twitter.com/intent/follow?screen_name=zenigane138)

![image](https://user-images.githubusercontent.com/36072156/233836921-1c8cb572-5666-4cf9-9082-0d24895702cb.png)

機能
---
- OpenUPM のパッケージ名検索
- ProjecetSettings(manifest.json) への登録

動作環境
---
- 必須
  - Unity 2021.2 以降

依存関係
---
- Json.NET ( Newtonsoft.Json ) に依存しています  
  - package.json では Unity レジストリの "com.unity.nuget.newtonsoft-json" を指定しています  

インストール方法
---
依存関係を手動で解決したい方は unitypackage を選び、自動で解決して欲しい方は PackageManager を選んで下さい。  
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
![image](https://user-images.githubusercontent.com/36072156/233836921-1c8cb572-5666-4cf9-9082-0d24895702cb.png)
- 任意のパッケージを "Register" ボタンを押して登録。
  - 登録に成功すればダイアログが表示されるの "OK" をクリック。  
![image](https://user-images.githubusercontent.com/36072156/233837007-1fafcb7e-84fc-4f35-ab1e-b9175505ea23.png)
  - manifest.json が開かれるので UnityEditor をアクティブにする。
  - UnityEditor が manifest.json の更新を検知して ProjectSettings に反映する。
  - "Importing a scoped registry" というダイアログが表示されたら "Close" する。  
![image](https://user-images.githubusercontent.com/36072156/233837103-c788eab0-779a-4bda-bccd-da6a7f977ad1.png)
- ProjectSettings ウィンドウ内の Package Manager で登録内容を確認する。  
![image](https://user-images.githubusercontent.com/36072156/233837167-e91bf218-5ce3-486d-a06e-4db5e53a2d24.png)
- PackageManager ウィンドウを開き "Packages:My Registries" に切り替え、登録されたパッケージをインストール。  
![image](https://user-images.githubusercontent.com/36072156/233837264-a8c49243-24c9-4348-9a44-258762687b77.png)

License
---
[LICENSE.md](/LICENSE.md) をご確認下さい。
