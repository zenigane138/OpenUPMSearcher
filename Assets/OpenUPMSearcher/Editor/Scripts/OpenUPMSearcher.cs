using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.IO;
using System;
using Newtonsoft.Json.Linq;

namespace OkaneGames.OpenUPMSearcher.Editor
{
    public class OpenUPMSearcher : EditorWindow
    {
        private static readonly string WindowTitle = "OpenUPM Searcher";
        private static readonly Vector2 WindowMinSize = new Vector2(350, 200);

        private static readonly string CacheFilePath = "Library/com.okanegames.openupmsearcher/api_cache.txt";
        private static readonly int DisplayMax = 100;

        private Vector2 _scroll;
        private string _searchText = "";
        private List<string> _packageNameList = null;

        private enum Message
        {
            None,
            GitHubAPI,
            ManifestJson,
        }

#if UNITY_2017_1_OR_NEWER
        [MenuItem("Window/OkaneGames/", priority = Int32.MaxValue)]
#endif
        [MenuItem("Window/OkaneGames/OpenUPM Searcher")]
        [MenuItem("OkaneGames/OpenUPM Searcher")]
        private static void CreateWindow()
        {
            var window = CreateInstance<OpenUPMSearcher>();
            window.titleContent = new GUIContent(WindowTitle);
            window.minSize = WindowMinSize;
            window.Show();
        }

        private void OnGUI()
        {
            // API関連
            GUILayout.BeginHorizontal();
            {
                var tempBGColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Get PackageList by GitHub API"))
                {
                    if (_packageNameList == null) CreatePackageListCacheFromGitHubAPI();
                    else
                    {
                        if (EditorUtility.DisplayDialog("Confirm", GetDialogMessage(Message.GitHubAPI), "Call API", "Cancel"))
                        {
                            CreatePackageListCacheFromGitHubAPI();
                        }
                    }
                }
                GUI.backgroundColor = tempBGColor;

                if (File.Exists(CacheFilePath)) GUILayout.Label(new GUIContent("API cache file: exists", "Last updated:" + File.GetLastWriteTime(CacheFilePath)));
                else GUILayout.Label("API cache file: not exist");
            }
            GUILayout.EndHorizontal();

            // MenuItem関連
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("Open ProjectSettings", "Please check 'Package Manager' for your registered data.")))
                {
                    if (!EditorApplication.ExecuteMenuItem("Edit/Project Settings...")) Debug.Log("Failed Open ProjectSettings");
                }
                if (GUILayout.Button(new GUIContent("Open PackageManager", "Please check 'Packages:My Registries' for your registered data.")))
                {
                    if (!EditorApplication.ExecuteMenuItem("Window/Package Manager")) Debug.Log("Failed Open PackageManager");
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Open manifest.json"))
            {
                var path = Path.Combine(Application.dataPath.Replace("/Assets", ""), "Packages/manifest.json");
                System.Diagnostics.Process.Start(path);
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Filter", GUILayout.Width(35));
                _searchText = GUILayout.TextField(_searchText);
                if (GUILayout.Button("Clear", GUILayout.Width(45)))
                {
                    _searchText = "";
                }
            }
            EditorGUILayout.EndScrollView();

            DrawPackageList();

            DrawZeniganeLink();
        }

        private void DrawPackageList()
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

            // キャッシュファイルがなくて、_packageNameList も作っていなければ API 実行を促す
            if (!File.Exists(CacheFilePath) && _packageNameList == null)
            {
                GUILayout.Label("Press 'Get PackageList by GitHub API' button to get the package list.");
                return;
            }
            else
            {
                // キャッシュファイルはあるけど、リストが空
                if (_packageNameList == null || _packageNameList.Count == 0)
                {
                    if (File.Exists(CacheFilePath))
                    {
                        _packageNameList = new List<string>();
                        var packageNames = File.ReadAllLines(CacheFilePath);
                        // 空行などは追加したくないので foreach で作る
                        foreach (var packageName in packageNames)
                        {
                            if (!string.IsNullOrWhiteSpace(packageName)) _packageNameList.Add(packageName);
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(_searchText))
            {
                GUILayout.Label(_packageNameList.Count + " packages exist. \nLast updated: " + File.GetLastWriteTime(CacheFilePath));
                EditorGUILayout.Space();
                GUILayout.Label("Enter a filter string in the 'Filter' field.");
            }

            bool isOpenManifestJson = false;
            var count = 0;
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            {
                if (_searchText.Length > 0 && _packageNameList != null)
                {
                    foreach (var packageName in _packageNameList)
                    {
                        if (!packageName.Contains(_searchText)) continue;

                        // 一定件数を超えたら表示省略
                        if (count++ >= DisplayMax) continue;

                        DrawPackage(packageName, ref isOpenManifestJson);
                    }
                }

            }
            EditorGUILayout.EndScrollView();

            if (count > DisplayMax) GUILayout.Label(count - DisplayMax + " packages have been omitted from display.");
            GUILayout.Label("Matched packages " + count + " / " + _packageNameList.Count + ".");

            if (isOpenManifestJson)
            {
                OpenManifestJson();
            }
        }

        private void DrawPackage(string packageName, ref bool isOpenManifestJson)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("Web", "Open in Web Browser\n" + "https://openupm.com/packages/" + packageName + "/"), GUILayout.Width(36)))
                {
                    Application.OpenURL("https://openupm.com/packages/" + packageName + "/");
                }

                if (GUILayout.Button(new GUIContent("Register", "Register to manifest.json.\n'" + packageName + "'"), GUILayout.Width(60)))
                {
                    var result = RegisterScope(packageName);
                    if (result)
                    {
                        if (EditorUtility.DisplayDialog("Result", GetDialogMessage(Message.ManifestJson), "OK"))
                        {
                            // ここで直接 System.Diagnostics.Process.Start() を実行するとレイアウト系エラーが発生するので後から実行
                            isOpenManifestJson = true;
                        }
                    }
                }

                // パッケージ名の検索文字列部分の強調表示
                var style = new GUIStyle(EditorStyles.wordWrappedLabel);
                style.richText = true;
                var displayName = packageName.Replace(_searchText, "<color=lime>" + _searchText + "</color>");
                // 自分のパッケージを見つけやすいようにこれぐらいは許してほしい
                displayName = packageName.Contains("com.okanegames") ? "★" + displayName : displayName;
                EditorGUILayout.LabelField(displayName, style);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OpenManifestJson()
        {
            var path = Path.Combine(Application.dataPath.Replace("/Assets", ""), "Packages/manifest.json");
            System.Diagnostics.Process.Start(path);
        }

        private void DrawZeniganeLink()
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("(C) 2023 OkaneGames / zenigane");
                if (GUILayout.Button(new GUIContent("GitHub", ""), GUILayout.Width(50)))
                {
                    Application.OpenURL("https://github.com/zenigane138");
                }
                if (GUILayout.Button(new GUIContent("Blog", ""), GUILayout.Width(35)))
                {
                    Application.OpenURL("https://zenigane138.hateblo.jp/?from=openupmsearcher");
                }
                if (GUILayout.Button(new GUIContent("Twitter", ""), GUILayout.Width(55)))
                {
                    Application.OpenURL("https://twitter.com/zenigane138");
                }
            }
            GUILayout.EndHorizontal();
        }

        // 引数は com.okanegames.anythingbookmark などの完全に一意のパッケージを示す形式
        // ProjectSettings の scope 自体には com.okanegames を登録して配下のパッケージを表示する事も出来る
        // しかし PackageManager もしくは OpenUPM に問題があるのか全てのパッケージが表示されるわけではない
        // ※ なんなら com だけも可能、また com.cysharp を登録すると UniTask がいないので明らかに不足している
        // 表示されるかそうでないかは実際に使うまでわからず使い勝手が非常に悪いので完全に一意なパッケージのみを登録する形になった
        public static bool RegisterScope(string scope)
        {
            const string RegistryName = "OpenUPM";
            const string RegistryURL = "https://package.openupm.com/";

            string projectPath = Application.dataPath.Replace("/Assets", "");
            string manifestPath = Path.Combine(projectPath, "Packages/manifest.json");

            if (!File.Exists(manifestPath))
            {
                Debug.LogError("manifest.json not found in the Packages folder.");
                return false;
            }

            string manifestContent = File.ReadAllText(manifestPath);
            JObject manifestJson = JObject.Parse(manifestContent);

            JArray scopedRegistries = manifestJson["scopedRegistries"] as JArray;
            if (scopedRegistries == null)
            {
                scopedRegistries = new JArray();
                manifestJson["scopedRegistries"] = scopedRegistries;
            }

            JObject openUPMRegistry = null;
            foreach (JObject registry in scopedRegistries)
            {
                // 名前が同じ物は別レジストリとして登録してしまうとエラーになるので、一致する物があれば追加対象レジストリ
                if (registry["name"] != null && registry["name"].ToString() == RegistryName)
                {
                    openUPMRegistry = registry;
                    break;
                }
            }

            // 同一URL間で同一スコープが存在してはいけないかと思ったらURL関係なしに同一スコープが存在してはいけないだった
            List<string> existingScopeList = new List<string>();
            foreach (JObject registry in scopedRegistries)
            {
                // 同一URLなら
                //if (registry["url"] != null && registry["url"].ToString() == RegistryURL) // バグの可能性もあるのでコメントアウトに留めておく
                {
                    JArray scopes = registry["scopes"] as JArray;
                    foreach (string s in scopes)
                    {
                        existingScopeList.Add(s);
                    }
                }
            }

            bool scopeExists = false;
            foreach (string existingScope in existingScopeList)
            {
                if (existingScope == scope)
                {
                    scopeExists = true;
                    Debug.LogError("'" + scope + "' is already registered scope.");
                    return false;
                }
            }

            // 新規登録
            if (openUPMRegistry == null)
            {
                openUPMRegistry = new JObject
                {
                    ["name"] = RegistryName,
                    ["url"] = RegistryURL,
                    ["scopes"] = new JArray(scope)
                };
                if (!scopeExists)
                {
                    scopedRegistries.Add(openUPMRegistry);
                }
            }
            // 追加
            else
            {
                if (!scopeExists)
                {
                    JArray scopes = openUPMRegistry["scopes"] as JArray;
                    scopes.Add(scope);
                }
            }

            File.WriteAllText(manifestPath, manifestJson.ToString());
            AssetDatabase.Refresh();
            return true;
        }

        private async void CreatePackageListCacheFromGitHubAPI()
        {
            string owner = "openupm";
            string repo = "openupm";
            string branch = "master";

            // data/packages フォルダの SHA を取得
            string dirSha = await GetGitHubDirectorySha(owner, repo, branch);

            // SHA から対象フォルダの全ファイルリストを取得
            if (!string.IsNullOrEmpty(dirSha))
            {
                await GetPackageFileListFromTree(owner, repo, dirSha);
            }
        }

        private async Task<string> GetGitHubDirectorySha(string owner, string repo, string branch)
        {
            string url = $"https://api.github.com/repos/{owner}/{repo}/branches/{branch}";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("User-Agent", "Unity");
                var asyncOperation = www.SendWebRequest();

                while (!asyncOperation.isDone)
                {
                    await Task.Delay(5);
                }

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + www.error);
                    return null;
                }
                else
                {
                    JObject jsonObject = JObject.Parse(www.downloadHandler.text);
                    string treeSha = jsonObject["commit"]["commit"]["tree"]["sha"].ToString();
                    return treeSha;
                }
            }
        }

        private async Task GetPackageFileListFromTree(string owner, string repo, string treeSha)
        {
            // 現状サブディレクトリは存在しないので recursive=0
            string url = $"https://api.github.com/repos/{owner}/{repo}/git/trees/{treeSha}?recursive=0";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("User-Agent", "Unity");
                var asyncOperation = www.SendWebRequest();

                while (!asyncOperation.isDone)
                {
                    await Task.Delay(5);
                }

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + www.error);
                }
                else
                {
                    JObject jsonObject = JObject.Parse(www.downloadHandler.text);
                    JArray treeArray = jsonObject["tree"] as JArray;
                    _packageNameList = new List<string>();

                    foreach (JObject item in treeArray)
                    {
                        string path = item["path"].ToString();
                        if (path.StartsWith("data/packages/") && path.EndsWith(".yml"))
                        {
                            string packageName = Path.GetFileNameWithoutExtension(path);
                            _packageNameList.Add(packageName);
                        }
                    }


                    Debug.Log("Found " + _packageNameList.Count + " packages.");

                    var outputText = "";
                    foreach (var packageName in _packageNameList)
                    {
                        outputText += packageName + System.Environment.NewLine;
                    }

                    var dirPath = Path.GetDirectoryName(CacheFilePath);
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    File.WriteAllText(CacheFilePath, outputText);

                    Debug.Log("Create cache file. path:" + CacheFilePath);
                }
            }
        }

        private string GetDialogMessage(Message msg)
        {
            var str = "";
            if (msg == Message.None) { }
            else if (msg == Message.GitHubAPI)
            {
                str = @"
This process calls the GitHub API to retrieve the list of package files in the master branch of the OpenUPM repository and create a cache file.
The GitHub API has a limit on the number of requests per minute and per hour.
Once the cache file is created at the first startup, there is no need to regenerate it for the time being.
We recommend that you do not run it more than necessary.

この処理はGitHub APIを呼び出してOpenUPMリポジトリのmasterブランチのパッケージファイル一覧を取得してキャッシュファイルを作成しています。
GitHub APIには1分と1時間あたりのリクエスト回数制限機能があります。
初回起動時に一度キャッシュファイルを作成すれば当分再生成の必要はありません。
必要以上に実行しないことをおすすめします。
";
            }
            else if (msg == Message.ManifestJson)
            {
                str = @"
Registration succeeded.
Open manifest.json to reflect this in the UnityEditor.
To be precise, deactivate Unity once and it will be reflected.
If it is reflected in ProjectSettings, you can install it from PackageManager (Packages:Unity Registry).

登録に成功しました。
Unityエディタに反映するためmanifest.jsonを開きます。
正確にはUnityを一度非アクティブにすれば反映されます。
ProjectSettingsへ反映されていたらPackageManager(Packages:Unity Registry)からインストールが可能になります。
";
            }

            return str;
        }

    }
}