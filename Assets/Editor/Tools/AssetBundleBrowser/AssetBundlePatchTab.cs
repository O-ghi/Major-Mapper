using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using LitJson;

namespace UnityEngine.AssetBundles
{
    [System.Serializable]
    public class AssetBundlePatchTab
    {
        private string m_streamingPath = "Assets/StreamingAssets";
        public string m_outputPath = string.Empty;

        public string m_obboutputPath = string.Empty;

        [SerializeField]
        private Vector2 m_ScrollPosition;

        public AssetBundleBuildTab.ValidBuildTarget m_buildTarget = AssetBundleBuildTab.ValidBuildTarget.Android;

        private AssetBundleBuildTab.ToggleData m_SubPackage;

        [SerializeField]
        private Versions versions;

        const string k_BuildPrefPrefix = "ABBBuild:";
        const string clientConf = "clientConfig.json";
        const string patchesName = "Patches.xml";
        const string packzipName = "pack_zip";
        const string packManiName = "manifest.xml";

        const string packsubName = "pack_sub";

        const string outpathKey = "outpathKey";

        const string obboutpathKey = "obboutpathKey";

        private string m_platformVerPath;
        private string m_versionPath;
        private string m_packversionPath;
        private string m_packzipdirPath;
        private string m_versionJsonPath;
        private string m_patchesXmlPath;

        GUIContent m_TargetContent;

        public static bool isRecording = false;

        private string m_packsubdirPath;
        private string m_useassetdirPath;
        private string m_unuseassetdirPath;

        const string usedAssetDic = "usedasset";
        const string unUseAssetDic = "unUsedasset";

        public AssetBundlePatchTab()
        {
            ReadClientConf();
        }

        public void OnEnable(Rect pos, EditorWindow parent)
        {
            m_SubPackage = new AssetBundleBuildTab.ToggleData(
               false,
               "Build SubPackage",
               "Will build SubPackage.",
               null);
            m_TargetContent = new GUIContent("Build Target", "Choose target platform to build for.");

            m_outputPath = PlayerPrefs.GetString(outpathKey);

            m_obboutputPath = PlayerPrefs.GetString(obboutpathKey);

            AssetLoadManager.isRecording = PlayerPrefs.GetInt("RecordAsset");

			isRecording = AssetLoadManager.isRecording == 1;
        }

        public void OnDisable()
        {
            
        }
        public void OnGUI(Rect pos)
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUILayout.Label(new GUIContent("Create a game resource patch"), centeredStyle);
            EditorGUILayout.Space();

            GUILayout.BeginVertical();

            // build target
            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildTarget))
            {
                AssetBundleBuildTab.ValidBuildTarget tgt = (AssetBundleBuildTab.ValidBuildTarget)EditorGUILayout.EnumPopup(m_TargetContent, m_buildTarget);
                if (tgt != m_buildTarget)
                {
                    m_buildTarget = tgt;
                    
                }
            }

            if (string.IsNullOrEmpty(m_outputPath))
            {
                m_outputPath = "AssetBundles/";
                m_outputPath += m_buildTarget.ToString();
                PlayerPrefs.SetString(outpathKey, m_outputPath);
            }

            ////obboutput path
            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildOutputDirectory))
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                var newPath = EditorGUILayout.TextField("Obb Output Path", m_obboutputPath);
                if ((newPath != m_obboutputPath) &&
                     (newPath != string.Empty))
                {
                    m_obboutputPath = newPath;
                    PlayerPrefs.SetString(obboutpathKey, m_obboutputPath);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
                    BrowseForFolder(obboutpathKey, m_obboutputPath);
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(75f)))
                    ResetPathToDefault(obboutpathKey, m_obboutputPath);

                GUILayout.EndHorizontal();
                EditorGUILayout.Space();

            }

            ////output path
            using (new EditorGUI.DisabledScope(!AssetBundleModel.Model.DataSource.CanSpecifyBuildOutputDirectory))
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                var newPath = EditorGUILayout.TextField("Output Path", m_outputPath);
                if ((newPath != m_outputPath) &&
                     (newPath != string.Empty))
                {
                    m_outputPath = newPath;
                    PlayerPrefs.SetString(outpathKey, m_outputPath);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Browse", GUILayout.MaxWidth(75f)))
                    BrowseForFolder();
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(75f)))
                    ResetPathToDefault();

                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
                
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            string old_game_type = GameInfoManager.GetAttibute("game_type");
            var game_type = EditorGUILayout.TextField("game_type", old_game_type, GUILayout.Width(400));
            if(old_game_type!=game_type)
            {
                GameInfoManager.SetAttibuteForSave("game_type", game_type);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("GameType,例如：HorseWar_cn_cn");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_pf_code = GameInfoManager.GetAttibute("pf_code");
            var pf_code = EditorGUILayout.TextField("pf_code", old_pf_code, GUILayout.Width(400));
            if (old_pf_code != pf_code)
            {
                GameInfoManager.SetAttibuteForSave("pf_code", pf_code);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("channel number, e.g.：hwy");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_sdk_id = GameInfoManager.GetAttibute("sdk_id");
            var sdk_id = EditorGUILayout.TextField("sdk_id", old_sdk_id, GUILayout.Width(400));
            if (old_sdk_id != sdk_id)
            {
                GameInfoManager.SetAttibuteForSave("sdk_id", sdk_id);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("SDKID: For example: uc.xf, ysdk, xf means aggregate SDK, NPxf means non-aggregate SDK");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_use_update = GameInfoManager.GetAttibute("use_update");
            var use_update = EditorGUILayout.TextField("use_update", old_use_update, GUILayout.Width(400));
            if (old_use_update!= use_update)
            {
                GameInfoManager.SetAttibuteForSave("use_update", use_update);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("Whether to enable update, 1 enable 0 disable");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_small_client = GameInfoManager.GetAttibute("small_client");
            var small_client = EditorGUILayout.TextField("small_client", old_small_client, GUILayout.Width(400));
            if (old_small_client != small_client)
            {
                GameInfoManager.SetAttibuteForSave("small_client", small_client);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("Whether it is a packet, 1 is 0 is not");
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string OldVerifyClient = GameInfoManager.GetAttibute("verify_client");
            var VerifyClient = EditorGUILayout.TextField("verify_client", OldVerifyClient, GUILayout.Width(400));

            if (OldVerifyClient != VerifyClient)
            {
                GameInfoManager.SetAttibuteForSave("verify_client", VerifyClient);

                GameInfoManager.SaveLocalClientConfig();
            }

            EditorGUILayout.LabelField("Whether it is an audit package, 1 is 0 is not");

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_review_status = GameInfoManager.GetAttibute("review_status");
            var review_status = EditorGUILayout.TextField("review_status", old_review_status, GUILayout.Width(400));
            if(old_review_status!=review_status)
            {
                GameInfoManager.SetAttibuteForSave("review_status", review_status);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("ReVIEW");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_app_version = GameInfoManager.GetAttibute("app_version");
            var app_version = EditorGUILayout.TextField("app_version", old_app_version ,GUILayout.Width(400));
            PlayerSettings.bundleVersion = app_version;
            if (old_app_version != app_version)
            {
                GameInfoManager.SetAttibuteForSave("app_version", app_version);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("Client game version number Rules: x.x.x");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_isObbPackage = GameInfoManager.GetAttibute("isObbPackage");
            var isObbPackage = EditorGUILayout.TextField("isObbPackage", old_isObbPackage, GUILayout.Width(400));
            if (old_isObbPackage != isObbPackage)
            {
                GameInfoManager.SetAttibuteForSave("isObbPackage", isObbPackage);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("Is it an obb package");
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_mini_app_version = GameInfoManager.GetAttibute("min_app_version");
            var mini_app_version = EditorGUILayout.TextField("min_app_version", old_mini_app_version, GUILayout.Width(400));
            if(old_mini_app_version!=mini_app_version)
            {
                GameInfoManager.SetAttibuteForSave("min_app_version", mini_app_version);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("Client minimum version number");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string versionCodeStr = "1";
            if (m_buildTarget == AssetBundleBuildTab.ValidBuildTarget.Android)
            {
                versionCodeStr = EditorGUILayout.TextField("versionCode", PlayerSettings.Android.bundleVersionCode.ToString(), GUILayout.Width(400));
                int versionCode;
                int.TryParse(versionCodeStr, out versionCode);
                PlayerSettings.Android.bundleVersionCode = versionCode;

                int old_versionCode = GameConvert.IntConvert(GameInfoManager.GetAttibute("versionCode"));

                if (old_versionCode != versionCode)
                {
                    GameInfoManager.SetAttibuteForSave("versionCode", versionCodeStr);
                    GameInfoManager.SaveLocalClientConfig();
                }
                EditorGUILayout.LabelField("The versionCode location of the Android project：PlayerSettings -> OtherSettings -> Bundle Version Code");
            }

            if (m_buildTarget == AssetBundleBuildTab.ValidBuildTarget.iOS)
            {
                versionCodeStr = EditorGUILayout.TextField("build", PlayerSettings.iOS.buildNumber, GUILayout.Width(400));
                int versionCode;
                int.TryParse(versionCodeStr, out versionCode);
                PlayerSettings.iOS.buildNumber = versionCode.ToString();

                int old_versionCode = GameConvert.IntConvert(GameInfoManager.GetAttibute("versionCode"));

                if (old_versionCode != versionCode)
                {
                    GameInfoManager.SetAttibuteForSave("versionCode", versionCodeStr);

                    GameInfoManager.SaveLocalClientConfig();
                }
                EditorGUILayout.LabelField("The build location of the IOS project：PlayerSettings -> OtherSettings -> Build");
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_res_server = GameInfoManager.GetAttibute("res_server");
            var res_server = EditorGUILayout.TextField("res_server", old_res_server, GUILayout.Width(400));
            if (old_res_server != res_server)
            {
                GameInfoManager.SetAttibuteForSave("res_server", res_server);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("The list of resource server domain names is separated by \",\"");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_server_list_version = GameInfoManager.GetAttibute("server_list_version");
            var server_list_version = EditorGUILayout.TextField("server_list_version", old_server_list_version, GUILayout.Width(400));
            if (old_server_list_version != server_list_version)
            {
                GameInfoManager.SetAttibuteForSave("server_list_version", server_list_version);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("server version");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_pack_version = GameInfoManager.GetAttibute("pack_version");
            var pack_version = EditorGUILayout.TextField("pack_version", old_pack_version, GUILayout.Width(400));
            if (old_pack_version != pack_version)
            {
                GameInfoManager.SetAttibuteForSave("pack_version", pack_version);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("Resource patch number(usually fill in 0）");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_dll_version = GameInfoManager.GetAttibute("dll_version");
            var dll_version = EditorGUILayout.TextField("dll_version", old_dll_version, GUILayout.Width(400));
            if (old_dll_version != dll_version)
            {
                GameInfoManager.SetAttibuteForSave("dll_version", dll_version);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("Code version number: (normally fill in 0)");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_patch_version = GameInfoManager.GetAttibute("patch_version");
            var patch_version = EditorGUILayout.TextField("res_version", old_patch_version, GUILayout.Width(400));
            if (old_patch_version != patch_version)
            {
                GameInfoManager.SetAttibuteForSave("patch_version", patch_version);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("Resource version number, for example: 201709251830");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_version_bulletin = GameInfoManager.GetAttibute("version_bulletin");
            var version_bulletin = EditorGUILayout.TextField("version_bulletin", old_version_bulletin, GUILayout.Width(400));
            if (old_version_bulletin != version_bulletin)
            {
                GameInfoManager.SetAttibuteForSave("version_bulletin", version_bulletin);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            string old_apk_url = GameInfoManager.GetAttibute("apk_url");
            var apk_url = EditorGUILayout.TextField("apk_url", old_apk_url, GUILayout.Width(800));
            if (old_apk_url != apk_url)
            {
                GameInfoManager.SetAttibuteForSave("apk_url", apk_url);
                GameInfoManager.SaveLocalClientConfig();
            }
            EditorGUILayout.LabelField("Exchange address");
            EditorGUILayout.EndHorizontal();

			GUILayout.Space (2);
			EditorGUILayout.BeginHorizontal();
			string old_functionIconName = GameInfoManager.GetAttibute("function_icon_name");
			var new_funtionIconName = EditorGUILayout.TextField("function_icon_name", old_functionIconName, GUILayout.Width(800));
			if (old_functionIconName != new_funtionIconName)
			{
				GameInfoManager.SetAttibuteForSave ("function_icon_name", new_funtionIconName);
				GameInfoManager.SaveLocalClientConfig();
			}
			EditorGUILayout.LabelField("Block button configuration");
			EditorGUILayout.EndHorizontal();

            ReadClientConf();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("client_config_url:", GameInfoManager.GetAttibute("client_config_url"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("client_config_local:", GameInfoManager.GetAttibute("client_config_local"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("server_list_local:", GameInfoManager.GetAttibute("server_list_local"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("server_list_url:", GameInfoManager.GetAttibute("server_list_url"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("patch_url:", GameInfoManager.GetAttibute("patch_url"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("patch_cache:", GameInfoManager.GetAttibute("patch_cache"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("patch_local:", GameInfoManager.GetAttibute("patch_cache"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("code_url:", GameInfoManager.GetAttibute("code_url"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("code_local:", GameInfoManager.GetAttibute("code_local"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("bulletin_date:", GameInfoManager.GetAttibute("bulletin_date"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("domain_dyn_conf:", GameInfoManager.GetAttibute("domain_dyn_conf"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("activity_res_url:", GameInfoManager.GetAttibute("activity_res_url"), GUILayout.Width(800));
            EditorGUILayout.EndHorizontal();



            /*  EditorGUILayout.BeginHorizontal();
              var platformSDK = EditorGUILayout.TextField("PlatformSDK", m_gameConf.platformSDK, GUILayout.Width(400));
              if (m_gameConf.platformSDK != platformSDK)
              {
                  m_gameConf.platformSDK = platformSDK;
                  WriteGameConf();
                  PlayerSettings.applicationIdentifier = GetPackageName(GameConvert.IntConvert(platformSDK));
              }
              EditorGUILayout.LabelField("SDK平台选择：0是没有，1是XF,2是UC，3是YSDK");
              EditorGUILayout.EndHorizontal();*/



            EditorGUILayout.Space();
            if (GUILayout.Button("Build"))
            {
                ReadVersionsJson();
                CopyAssetToVersion();
                GameInfoManager.SaveLocalClientConfig();
                BuildVersion();
            }

            if (GUILayout.Button("Manually type differential packages, and do resource repairs when bugs occur"))
            {
                ReadVersionsJson();
                BuildVersion();
            }

            EditorGUILayout.Space();

            GUILayout.Label(new GUIContent("Recording Assets and Production Subcontracting Items"), centeredStyle);

            bool recording = GUILayout.Toggle(isRecording, "Record resource usage");
            if (recording != isRecording)
            {
                isRecording = recording;
				AssetLoadManager.isRecording = isRecording ? 1 : 0;

				PlayerPrefs.SetInt("RecordAsset", isRecording == true ? 1 : 0);
                string log = isRecording == true ? "开启" : "关闭";
                Debug.LogFormat("Resource recording({0})", log);
            }

            if (GUILayout.Button("Export used resources to txt file"))
            {
				if (!isRecording)
				{
					Debug.LogError("Do not turn off recording");
					return;
				}

                List<string> assets = AssetLoadManager.GetRecordAssets();

                m_platformVerPath = m_outputPath + "_version";
                CreateDir(m_platformVerPath);
                m_versionPath = m_platformVerPath + "/" + GameInfoManager.GetAttibute("app_version");
                CreateDir(m_versionPath);
                m_packsubdirPath = m_versionPath + "/" + packsubName;
                CreateDir(m_packsubdirPath);

                assets.Sort(SortComparison);

                File.WriteAllLines(m_packsubdirPath+"/recordAsset.txt", assets.ToArray());
				Debug.LogFormat("Export recording files to：{0}/recordAsset.txt", m_packsubdirPath);
			}

			if (GUILayout.Button("start subcontracting"))
            {
                m_platformVerPath = m_outputPath + "_version";
                CreateDir(m_platformVerPath);
                m_versionPath = m_platformVerPath + "/" + GameInfoManager.GetAttibute("app_version");
                CreateDir(m_versionPath);
                m_packsubdirPath = m_versionPath + "/" + packsubName;
                CreateDir(m_packsubdirPath);

                string[] assets = File.ReadAllLines(m_packsubdirPath + "/recordAsset.txt");
                m_useassetdirPath = m_packsubdirPath + "/" + usedAssetDic;
                m_unuseassetdirPath = m_packsubdirPath + "/" + unUseAssetDic;
                CreateDir(m_useassetdirPath,true);
                CreateDir(m_unuseassetdirPath,true);

                string buildTarget = m_buildTarget.ToString();
                string use_url = m_useassetdirPath +"/"+ buildTarget;
                string unuse_url = m_unuseassetdirPath + "/" + buildTarget;

                string[] defultUseAsset = new string[]
                {
                    buildTarget,
                     "config.bundle",
                     "font.bundle",
                      "lua.bundle",
                      "shaders.bundle",
                };

                List<string> list = new List<string>();
                for (int i = 0; i < defultUseAsset.Length; i++)
                {
                    list.Add(defultUseAsset[i]);
                    list.Add(defultUseAsset[i] + ".manifest");
                }

                for (int i = 0; i < assets.Length;i++)
                {
                    if (!list.Contains(assets[i]))
                        list.Add(assets[i]);
                    if (!list.Contains(assets[i] + ".manifest"))
                        list.Add(assets[i] + ".manifest");
                }

                //Generate configuration information for bundled resources
                var data = ScriptableObject.CreateInstance<RecordAssetData>();
                data.assets = new string[assets.Length+defultUseAsset.Length];
                assets.CopyTo(data.assets, 0);
                defultUseAsset.CopyTo(data.assets, assets.Length);
                data.assetsLength = assets.Length + defultUseAsset.Length;
                AssetDatabase.CreateAsset(data, "Assets/Resources/recordAsset.asset");
                
                CopyAndSubAsset(m_outputPath,use_url,unuse_url, list);

                string zipname = string.Format("{0}/{1}.zip", m_unuseassetdirPath, "subasset");
                ZipTools.ZipFileDirectory(unuse_url, zipname, OnZipProgressHandler, OnZipCompleteHandler);
                //if (Directory.Exists(unuse_url))
                //    Directory.Delete(unuse_url, true);

                FileInfo fileinfo = new FileInfo(zipname);
                RequestFileInfo requestfile = new RequestFileInfo();
                requestfile.name = fileinfo.Name;
                requestfile.size = fileinfo.Length;

				byte[] buffer = new byte[1024 * 1024];
				requestfile.crc = ZipTools.GetFileCRC(zipname, buffer);

                string filejsonName = string.Format("{0}/{1}.json", m_unuseassetdirPath, "SubAssets");
                string filejson = JsonMapper.ToJson(requestfile);
                File.WriteAllText(filejsonName, filejson);

                string destDirName = m_streamingPath + "/" + m_buildTarget.ToString();
                CreateDir(destDirName,true);
                BackAsset(use_url, destDirName);
            }

            if (GUILayout.Button("generate obb file"))
            {
                ExportAndroidProject.ExportObbFile(m_obboutputPath);
            }

            if (GUILayout.Button("Generate subcontracting configuration information to Resources"))
            {
                m_platformVerPath = m_outputPath + "_version";
                CreateDir(m_platformVerPath);
                m_versionPath = m_platformVerPath + "/" + GameInfoManager.GetAttibute("app_version");
                CreateDir(m_versionPath);
                m_packsubdirPath = m_versionPath + "/" + packsubName;
                CreateDir(m_packsubdirPath);
                string[] assets = File.ReadAllLines(m_packsubdirPath + "/recordAsset.txt");

                string buildTarget = m_buildTarget.ToString();

                string[] defultUseAsset = new string[]
                {
                    buildTarget,
                     "config.bundle",
                     "font.bundle",
                      "lua.bundle",
                      "shaders.bundle",
                };

                //Generate configuration information for bundled resources
                var data = ScriptableObject.CreateInstance<RecordAssetData>();
                data.assets = new string[assets.Length + defultUseAsset.Length];
                assets.CopyTo(data.assets, 0);
                defultUseAsset.CopyTo(data.assets, assets.Length);
                data.assetsLength = assets.Length + defultUseAsset.Length;
                AssetDatabase.CreateAsset(data, "Assets/Resources/recordAsset.asset");
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void ReadClientConf()
        {
            string json = File.ReadAllText(m_streamingPath + "/" + clientConf);
            GameInfoManager.LoadClientConfigLocal(json);
        }

        private void ResetPathToDefault(string pathKey, string path)
        {
            path = "AssetBundles/";
            path += m_buildTarget.ToString();
            PlayerPrefs.SetString(pathKey, m_outputPath);
        }


        private string GetPackageName(int currentPlatformSDK)
        {
            string packageName = PlayerSettings.applicationIdentifier;

            if (currentPlatformSDK == 0)
            {
                packageName = "com.leji.fsq";
            }
            else if (currentPlatformSDK == 1)
            {
                packageName = "com.xianfeng.zyj";
            }
            else if (currentPlatformSDK == 2)
            {
                packageName = "con.xianfeng.zyj.aligames";
            }
            else if (currentPlatformSDK == 3)
            {
                packageName = "com.tencent.tmgp.xf.zyj";
            }

            return packageName;
        }

        public void CopyAndSubAsset(string root_url, string use_url, string unuse_url, List<string> uselist)
        {
            //get all files
            root_url = Path.GetFullPath(root_url);
            use_url = Path.GetFullPath(use_url);
            unuse_url = Path.GetFullPath(unuse_url);

            m_FilesList.Clear();

            GetDirectoryFiles(root_url);

            float progress = 0;
            int count = m_FilesList.Count;
            for (int i = 0; i < count; i++)
            {
                FileInfo file = m_FilesList[i];

                string path = "";
                int length = file.FullName.Length - file.Name.Length - root_url.Length - 2;
                if (length > 0)
                    path = file.FullName.Substring(root_url.Length + 1,
                    length);
                else
                    path = "";
                path = path.Replace(@"\", @"/");

                string filepath = string.IsNullOrEmpty(path) ? file.Name : path + "/" + file.Name;

                string dirctory = "";

                if (uselist.Contains(filepath))
                {
                    dirctory = Path.Combine(use_url, path);
                }
                else
                {
                    dirctory = Path.Combine(unuse_url, path);
                }
                
                if (!Directory.Exists(dirctory))
                {
                    Directory.CreateDirectory(dirctory);
                }
                progress = (float)i / (float)count;
                EditorUtility.DisplayProgressBar("Copy Assets", file.Name, progress);
                file.CopyTo(Path.Combine(dirctory, file.Name), true);
            }
            EditorUtility.ClearProgressBar();
        }

        private int SortComparison(string x, string y)
        {
            int xlen = x.Length;
            int ylen = y.Length;
            int len = xlen < ylen ? xlen : ylen;
            for (int i = 0; i < len; i++)
            {
                if (x[i] > y[i])
                    return 1;
                else if (x[i] < y[i])
                    return -1;
            }
            return 0;
        }

        private void BrowseForFolder(string pathKey, string path)
        {
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", path, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                var gamePath = System.IO.Path.GetFullPath(".");
                gamePath = gamePath.Replace("\\", "/");
                if (newPath.StartsWith(gamePath) && newPath.Length > gamePath.Length)
                    newPath = newPath.Remove(0, gamePath.Length + 1);
                path = newPath;
                m_obboutputPath = newPath;
                PlayerPrefs.SetString(pathKey, path);
            }
        }

        private void BrowseForFolder()
        {
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", m_outputPath, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                var gamePath = System.IO.Path.GetFullPath(".");
                gamePath = gamePath.Replace("\\", "/");
                if (newPath.StartsWith(gamePath) && newPath.Length > gamePath.Length)
                    newPath = newPath.Remove(0, gamePath.Length + 1);
                m_outputPath = newPath;
                PlayerPrefs.SetString(outpathKey, m_outputPath);
            }
        }
        private void ResetPathToDefault()
        {
            m_outputPath = "AssetBundles/";
            m_outputPath += m_buildTarget.ToString();
            PlayerPrefs.SetString(outpathKey, m_outputPath);
        }

        private void WritePatchesXml()
        {
            RequestInfo info = new RequestInfo();
            info.appversion = versions.app_version;
            info.packversion = versions.max_pack;
            info.filelist = new List<RequestFileInfo>();
			byte[] buffer = new byte[10 * 1024];

			m_patchesXmlPath = m_packzipdirPath + "/" + patchesName;
            DirectoryInfo dir = new DirectoryInfo(m_packzipdirPath);
            foreach (FileInfo child in dir.GetFiles("*.zip"))
            {
                RequestFileInfo fileinfo = new RequestFileInfo();
                fileinfo.name = child.Name;
                fileinfo.size = child.Length;
				fileinfo.crc = ZipTools.GetFileCRC(m_packzipdirPath + "/" + fileinfo.name, buffer);
                info.filelist.Add(fileinfo);
            }

            info.filelist.Sort(SortFileInfoList);

            XmlDocument xmldoc = new XmlDocument();
            XmlElement patches = xmldoc.CreateElement("patches");
            patches.SetAttribute("appVersion", info.appversion);
            patches.SetAttribute("maxVersion", info.packversion.ToString());

            for (int i = 0; i < info.filelist.Count; i++)
            {
                RequestFileInfo fileinfo = info.filelist[i];
                XmlElement patch = xmldoc.CreateElement("patch");

                patch.SetAttribute("name", fileinfo.name);
                patch.SetAttribute("size", fileinfo.size.ToString());
                patch.SetAttribute("crc", fileinfo.crc.ToString());
                patches.AppendChild(patch);
            }

            xmldoc.AppendChild(patches);
            xmldoc.Save(m_patchesXmlPath);

            EditorUtility.DisplayDialog("BuildAsset", "Build Success", "ok");
        }

        private int SortFileInfoList(RequestFileInfo info1, RequestFileInfo info2)
        {
            string name1 = Path.GetFileNameWithoutExtension(info1.name);
            string name2 = Path.GetFileNameWithoutExtension(info2.name);

            string[] strarr_1 = name1.Split('-');
            string[] strarr_2 = name2.Split('-');

            if (strarr_1.Length > strarr_2.Length)
            {
                return -1;
            }
            else if (strarr_1.Length < strarr_2.Length)
            {
                return 1;
            }
            else
            {
                for (int i = 0; i < strarr_1.Length; i++)
                {
                    int int_1 = int.Parse(strarr_1[i]);
                    int int_2 = int.Parse(strarr_2[i]);
                    if (int_1 > int_2)
                    {
                        return 1;
                    }
                    else if (int_1 < int_2)
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }

        private void ReadVersionsJson()
        {
            string app_version = GameInfoManager.GetAttibute("app_version");

            m_platformVerPath = m_outputPath + "_version";
            CreateDir(m_platformVerPath);
            m_versionPath = m_platformVerPath + "/" + app_version;
            CreateDir(m_versionPath);
            m_packzipdirPath = m_versionPath + "/" + packzipName;
            CreateDir(m_packzipdirPath);

            m_versionJsonPath = m_platformVerPath + "/versions_" + app_version + ".json";

            if (File.Exists(m_versionJsonPath))
            {
                string version_json = File.ReadAllText(m_versionJsonPath);
                versions = JsonUtility.FromJson<Versions>(version_json);
            }
            else
            {
                versions = new Versions();
                versions.app_version = app_version;
                versions.max_pack = -1;
            }
        }

        //生成版本信息并copy版本文件到补丁文件夹及修改版本号
        private void CopyAssetToVersion()
        {
            //比较当前的版本和之前的版本是否一致
            if (versions.max_pack >= 0)
            {
                string buildTarget = m_buildTarget.ToString();
                int before_pack = versions.max_pack;

                string before_manifest_url = string.Format("{0}/{1}/{2}.manifest", m_versionPath, before_pack, buildTarget);

                string curr_manifest_url = string.Format("{0}/{1}.manifest", m_outputPath, buildTarget);

                bool comp = CompManifest(before_manifest_url, curr_manifest_url);
                if (comp)
                {
                    EditorUtility.DisplayDialog("BuildAsset message", "No resource variances", "ok");
                    return;
                }
            }

            string curr_url = string.Format("{0}", m_outputPath);
            versions.max_pack++;
            m_packversionPath = m_versionPath + "/" + versions.max_pack;
            CreateDir(m_packversionPath);
            string dest_url = string.Format("{0}", m_packversionPath);
            BackAsset(curr_url, dest_url);

            File.WriteAllText(m_versionJsonPath, JsonUtility.ToJson(versions));
        }

        private void BuildVersion()
        {
            string buildTarget = m_buildTarget.ToString();
            //比较当前的manifest与前一个版本的是否相同，相同则不进行差异出包
            int before_pack = versions.max_pack - 1;
            if (before_pack >= 0)
            {
                string before_url = string.Format("{0}/{1}/{2}", m_versionPath, before_pack, buildTarget);
                string before_manifest_url = string.Format("{0}/{1}/{2}.manifest", m_versionPath, before_pack, buildTarget);

                string curr_url = string.Format("{0}/{1}/{2}", m_versionPath, versions.max_pack, buildTarget);
                string curr_manifest_url = string.Format("{0}/{1}/{2}.manifest", m_versionPath, versions.max_pack, buildTarget);

                bool comp = CompManifest(before_manifest_url, curr_manifest_url);
                if (comp)
                {
                    Debug.Log("No resource variance !");
                    return;
                }

                AssetBundleManifest before_manifest = GetManifest(before_url);
                AssetBundleManifest curr_manifest = GetManifest(curr_url);

                CopyCompFiles(before_manifest, curr_manifest);
            }
            else
            {
                //如果是分包模式
                if (m_SubPackage.state)
                {
                    SubPackageBuild();
                }
                else
                {
                    WritePatchesXml();
                }
            }
        }

        private void SubPackageBuild()
        {
            string buildTarget = m_buildTarget.ToString();
            string curr_url = string.Format("{0}", m_outputPath);
            versions.max_pack++;
            m_packversionPath = m_versionPath + "/" + versions.max_pack;
            CreateDir(m_packversionPath);
            string dest_url = string.Format("{0}", m_packversionPath);
            BackAsset(curr_url, dest_url);
            File.WriteAllText(m_versionJsonPath, JsonUtility.ToJson(versions));

            string curr_target_url = string.Format("{0}/{1}/{2}", m_versionPath, versions.max_pack, buildTarget);
            AssetBundleManifest curr_manifest = GetManifest(curr_target_url);

            string source_root = string.Format("{0}/{1}", m_versionPath, versions.max_pack);

            string root = m_packzipdirPath + "/" + m_buildTarget.ToString();
            string target_sourceFileName = string.Format("{0}/{1}", source_root, m_buildTarget.ToString());
            string target_destFileName = string.Format("{0}/{1}", root, m_buildTarget.ToString());
            string target_mani_sourceFileName = string.Format("{0}/{1}.manifest", source_root, m_buildTarget.ToString());
            string target_mani_destFileName = string.Format("{0}/{1}.manifest", root, m_buildTarget.ToString());
            CreateDir(root);
            File.Copy(target_sourceFileName, target_destFileName, true);
            File.Copy(target_mani_sourceFileName, target_mani_destFileName, true);

            string[] bundles = curr_manifest.GetAllAssetBundles();
            int length = bundles.Length;
            for (int i = 0; i < length; i++)
            {
                string bundlename = bundles[i];
                string sourceFileName = string.Format("{0}/{1}", source_root, bundlename);
                string destFileName = string.Format("{0}/{1}", root, bundlename);

                string mani_sourceFileName = string.Format("{0}/{1}.manifest", source_root, bundlename);
                string mani_destFileName = string.Format("{0}/{1}.manifest", root, bundlename);

                CreateFileParentDir(bundlename);
                File.Copy(sourceFileName, destFileName, true);
                File.Copy(mani_sourceFileName, mani_destFileName, true);
            }

            ZipPackage();
        }

        private void CopyCompFiles(AssetBundleManifest before_manifest, AssetBundleManifest curr_manifest)
        {
            string source_root = string.Format("{0}/{1}", m_versionPath, versions.max_pack);

            string root = m_packzipdirPath + "/" + m_buildTarget.ToString();
            string target_sourceFileName = string.Format("{0}/{1}", source_root, m_buildTarget.ToString());
            string target_destFileName = string.Format("{0}/{1}", root, m_buildTarget.ToString());
            string target_mani_sourceFileName = string.Format("{0}/{1}.manifest", source_root, m_buildTarget.ToString());
            string target_mani_destFileName = string.Format("{0}/{1}.manifest", root, m_buildTarget.ToString());
            CreateDir(root);
            File.Copy(target_sourceFileName, target_destFileName, true);
            File.Copy(target_mani_sourceFileName, target_mani_destFileName, true);

            string[] bundles = curr_manifest.GetAllAssetBundles();
            int length = bundles.Length;
            for (int i = 0; i < length; i++)
            {
                string bundlename = bundles[i];
                Hash128 curr_hash = curr_manifest.GetAssetBundleHash(bundlename);
                Hash128 before_hash = before_manifest.GetAssetBundleHash(bundlename);
                if (before_hash != curr_hash)
                {
                    string sourceFileName = string.Format("{0}/{1}", source_root, bundlename);
                    string destFileName = string.Format("{0}/{1}", root, bundlename);

                    string mani_sourceFileName = string.Format("{0}/{1}.manifest", source_root, bundlename);
                    string mani_destFileName = string.Format("{0}/{1}.manifest", root, bundlename);

                    CreateFileParentDir(bundlename);
                    File.Copy(sourceFileName, destFileName, true);
                    File.Copy(mani_sourceFileName, mani_destFileName, true);
                }
            }

            ZipPackage();
        }

        private void ZipPackage()
        {
            string root = m_packzipdirPath + "/" + m_buildTarget.ToString();

            string zipname = m_packzipdirPath + "/" + versions.max_pack + ".zip";
            ZipTools.ZipFileDirectory(root, zipname, OnZipProgressHandler, OnZipCompleteHandler);

            int before_pack = versions.max_pack - 1;
            if (before_pack > 0)
            {
                string before_full_zippath = string.Format("{0}/0-{1}.zip", m_packzipdirPath, before_pack);
                ZipTools.UnZipDir(before_full_zippath, root, OnUnzipProgress, OnUnzipCompleted,replace:false);
            }
            else
            {
                OnUnzipCompleted();
            }

        }

        private void OnZipCompleteHandler()
        {
            EditorUtility.ClearProgressBar();
        }

        private void OnZipProgressHandler(string filename, long loadedsize, long toltalsize)
        {
            float progress = (float)loadedsize / (float)toltalsize;
            EditorUtility.DisplayProgressBar("Pack the file", filename, progress);
        }

        private void OnUnzipCompleted()
        {
            EditorUtility.ClearProgressBar();

            string root = m_packzipdirPath + "/" + m_buildTarget.ToString();
            string zipname = string.Format("{0}/0-{1}.zip", m_packzipdirPath, versions.max_pack);

            ZipTools.ZipFileDirectory(root, zipname, OnZipProgressHandler, OnZipCompleteHandler);

            if (Directory.Exists(root))
                Directory.Delete(root, true);

            WritePatchesXml();
        }

        private void OnUnzipProgress(string filename, long loadedsize, long toltalsize)
        {
            float progress = (float)loadedsize / (float)toltalsize;
            EditorUtility.DisplayProgressBar("UnPack the file", filename, progress);
        }

        private void CreateFileParentDir(string bundlename)
        {
            string root = m_packzipdirPath + "/" + m_buildTarget.ToString();
            CreateDir(root);
            string[] names = bundlename.Split('/');
            int count = names.Length - 1;
            for (int i = 0; i < count; i++)
            {
                root += "/" + names[i];
                CreateDir(root);
            }
        }

        private bool CompManifest(string arg1, string arg2)
        {
            string[] manifest_str1 = File.ReadAllLines(arg1);
            string crc1 = manifest_str1[1].Replace("CRC: ", "");
            string[] manifest_str2 = File.ReadAllLines(arg2);
            string crc2 = manifest_str2[1].Replace("CRC: ", "");
            if (crc1.Equals(crc2))
                return true;
            return false;
        }

        private AssetBundleManifest GetManifest(string url)
        {
            AssetBundle assetbundle = AssetBundle.LoadFromFile(url);
            if (assetbundle == null)
                return null;

            AssetBundleManifest manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            assetbundle.Unload(false);
            return manifest;
        }

        private void CreateDir(string dirurl, bool clear = false)
        {
            bool isexists = Directory.Exists(dirurl);
            if (isexists && clear)
            {
                Directory.Delete(dirurl, true);
            }
            if (!isexists)
                Directory.CreateDirectory(dirurl);
        }

        private static List<FileInfo> m_FilesList = new List<FileInfo>();
        public void BackAsset(string root_url, string dest_url)
        {
            //获得所有文件
            root_url = Path.GetFullPath(root_url);
            dest_url = Path.GetFullPath(dest_url);
            m_FilesList.Clear();

            GetDirectoryFiles(root_url);

            float progress = 0;
            int count = m_FilesList.Count;
            for (int i = 0; i < count; i++)
            {
                FileInfo file = m_FilesList[i];

                string path = "";
                int length = file.FullName.Length - file.Name.Length - root_url.Length - 2;
                if (length > 0)
                    path = file.FullName.Substring(root_url.Length + 1,
                    length);
                else
                    path = "";
                path = path.Replace(@"\", @"/");
                string dirctory = Path.Combine(dest_url, path);
                if (!Directory.Exists(dirctory))
                {
                    Directory.CreateDirectory(dirctory);
                }
                progress = (float)i / (float)count;
                EditorUtility.DisplayProgressBar("Copy Assets", file.Name, progress);
                file.CopyTo(Path.Combine(dirctory, file.Name), true);
            }
            EditorUtility.ClearProgressBar();
        }

        private void GetDirectoryFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (DirectoryInfo child in dir.GetDirectories())
            {
                GetDirectoryFiles(child.FullName);
            }

            foreach (FileInfo file in dir.GetFiles())
            {
                m_FilesList.Add(file);
            }
        }
    }
}
