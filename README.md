# OpenUPM Searcher
日本語版の README.md は[こちら](/README_ja.md)。  

## Overview
OpenUPM Searcher is an editor extension window for Unity that provides package search functionality for OpenUPM and registration to ProjectSettings (manifest.json).  
Node.js or openupm-cli installation is not required.

![](https://img.shields.io/badge/Unity-2021.2%20or%20later-lightgrey)
[![](https://img.shields.io/badge/license-MIT-orange)](https://github.com/zenigane138/AnythingBookmark/blob/main/LICENSE.md)
[![](https://img.shields.io/badge/readme-%E6%97%A5%E6%9C%AC%E8%AA%9E%E7%89%88-red)](/README_ja.md)
[![openupm](https://img.shields.io/npm/v/com.okanegames.openupmsearcher?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.okanegames.openupmsearcher/)
[![](https://img.shields.io/badge/Follow-FFFFFF.svg?logo=twitter&style=flat)](https://twitter.com/intent/follow?screen_name=zenigane138)

![image](https://user-images.githubusercontent.com/36072156/233836921-1c8cb572-5666-4cf9-9082-0d24895702cb.png)

## Features
- Package name search for OpenUPM
- Registration to ProjectSettings (manifest.json)

## Requirements
- Unity 2021.2 or later

## Dependencies
- Depends on Json.NET ( Newtonsoft.Json )  
  - package.json specifies "com.unity.nuget.newtonsoft-json" in the Unity registry  

## Installation
- unitypackage
  - Download the unitypackage from the Releases page:
  - https://github.com/zenigane138/OpenUPMSearcher/releases
- PackageManager
  - Open the PackageManager window from Window -> PackageManager
  - Select "Add package from git URL..." from the +▼ button and set the following URL:
  - https://github.com/zenigane138/OpenUPMSearcher.git?path=Assets/OpenUPMSearcher

## How to Use
- Open the window from Unity's menu:
  - OkaneGames -> OpenUPM Searcher
  - Window -> OkaneGames -> OpenUPM Searcher
- Click the "Get PackageList by GitHub API" button to get the package list.
- Enter the package name you want to search for in the "Filter" field.
  - Example: com.okanegames
  - Example: unitask
  - ![image](https://user-images.githubusercontent.com/36072156/233836921-1c8cb572-5666-4cf9-9082-0d24895702cb.png)
- Click the "Register" button for any package you want to register.
  - If registration is successful, a dialog box will be displayed. Click "OK".
  - The manifest.json file will be opened and Unity Editor will be activated.
  - Unity Editor will detect the update in manifest.json and reflect it in ProjectSettings.
  - If a dialog box saying "Importing a scoped registry" is displayed, click "Close".
  - ![image](https://user-images.githubusercontent.com/36072156/233837007-1fafcb7e-84fc-4f35-ab1e-b9175505ea23.png)
- Confirm the registered content in the Package Manager window in ProjectSettings.
  - ![image](https://user-images.githubusercontent.com/36072156/233837167-e91bf218-5ce3-486d-a06e-4db5e53a2d24.png)
- Open the PackageManager window and switch to "Packages: My Registries", then install the registered packages.
  - ![image](https://user-images.githubusercontent.com/36072156/233837264-a8c49243-24c9-4348-9a44-258762687b77.png)

## License
Please see [LICENSE.md](/LICENSE.md).
