/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.09.22
*/

using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class ABBuilder
{
    private const string shaderListPath = "Assets/ArtResources/OutPut/Other/shader_list.prefab";
    public static BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;

    private static void initPath()
    {
        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);
    }

    [MenuItem("Tools/AB/Modify package ab output path #&L", false, 200)]
    public static void SetABOutPath()
    {
        string defaultPath = Application.dataPath + "/../ABOut";
        string path = EditorPrefs.GetString(AB_Path, defaultPath);
        if (path == defaultPath)
            path = Path.GetFullPath(path);
        string selectPath = EditorUtility.OpenFolderPanel("Select package output path", path, "");
        if (!string.IsNullOrEmpty(selectPath))
            path = selectPath;
        EditorPrefs.SetString(AB_Path, path);
        Debug.LogWarning("ab path modified to ->" + path);
    }

    [MenuItem("Tools/AB/Check Shader AssetBundle", false, 200)]
    public static void CheckShaderAsssetbundle()
    {
        EditorUtility.DisplayProgressBar("Check AB", "Đang check all AB", 0);
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (objs == null || objs.Length == 0)
        {
            EditorUtility.DisplayDialog("Check AB", "The current selection is invalid and cannot be packaged\nPlease select the resource or prefab in the Assets directory", "OK");
            EditorUtility.ClearProgressBar();
            return;
        }
        //tạo file check data
        StringBuilder checkText = new StringBuilder();

        initPath();
        EditorUtility.DisplayProgressBar("Chuẩn bị", "Đang kiểm tra shader list", 0);
        getShaderList();
        EditorUtility.DisplayProgressBar("Chuẩn bị", "Đang kiểm tra Assets", 0.2f);
        BuildOneAB(objs, false, ref checkText, "", true);
        string outputPath = Application.dataPath + "/../OutLogBundle/";
        if (!System.IO.Directory.Exists(outputPath))
            System.IO.Directory.CreateDirectory(outputPath);
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputPath + "/logCheckShaderAB.txt", false, new UTF8Encoding(false)))
        {
            sw.Write(checkText);
            sw.Flush();
        }

        EditorUtility.DisplayProgressBar("End", "Check hoàn thành", 1);
        EditorUtility.ClearProgressBar();
        System.Diagnostics.Process.Start(outputPath);
    }
    [MenuItem("Tools/AB/Check All AssetBundle", false, 200)]
    public static void CheckAllAsssetbundle()
    {
        EditorUtility.DisplayProgressBar("Check AB", "Đang check all AB", 0);
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (objs == null || objs.Length == 0)
        {
            EditorUtility.DisplayDialog("Check AB", "The current selection is invalid and cannot be packaged\nPlease select the resource or prefab in the Assets directory", "OK");
            EditorUtility.ClearProgressBar();
            return;
        }
        //tạo file check data
        StringBuilder checkText = new StringBuilder();

        initPath();
        EditorUtility.DisplayProgressBar("Chuẩn bị", "Đang kiểm tra shader list", 0);
        getShaderList();
        EditorUtility.DisplayProgressBar("Chuẩn bị", "Đang kiểm tra Assets", 0.2f);

        BuildOneAB(objs, false, ref checkText);
        string outputPath = Application.dataPath + "/../OutLogBundle/";
        if (!System.IO.Directory.Exists(outputPath))
            System.IO.Directory.CreateDirectory(outputPath);
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputPath + "/logCheckAB.txt", false, new UTF8Encoding(false)))
        {
            sw.Write(checkText);
            sw.Flush();
        }

        EditorUtility.DisplayProgressBar("End", "Check hoàn thành", 1);
        EditorUtility.ClearProgressBar();
        System.Diagnostics.Process.Start(outputPath);
    }

    private const string AB_Path = "Editor_AB_Out_Path";
    public static string outPath
    {
        get
        {
#if UNITY_ANDROID
            string defaultDic = Application.dataPath + "/../ABOut/Android";
#elif UNITY_IPHONE
            string defaultDic = Application.dataPath + "/../ABOut/IOS";
#else
            string defaultDic = Application.dataPath + "/../ABOut/Other";
#endif
            return EditorPrefs.GetString(AB_Path, defaultDic);
        }
    }

    private static List<Shader> shaderList = new List<Shader>();
    /// 获取shaderList
    private static void getShaderList()
    {
        shaderList.Clear();
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(shaderListPath);
        shaderList.AddRange(obj.GetComponent<ShaderList>().shaders);
    }

    [MenuItem("Tools/AB/selected resource package AB #&C", false, 201)]
    public static void BuildAB()
    {
        EditorUtility.DisplayProgressBar("Nhắc nhở", "Sẵn sàng đóng gói", 0);
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (objs == null || objs.Length == 0)
        {
            EditorUtility.DisplayDialog("Prompt", "The current selection is invalid and cannot be packaged\nPlease select the resource or prefab in the Assets directory", "OK");
            EditorUtility.ClearProgressBar();
            return;
        }
        StringBuilder checkText = new StringBuilder();

        initPath();
        EditorUtility.DisplayProgressBar("Prepare", "Start Packaging", 0);
        getShaderList();
        BuildOneAB(objs, true, ref checkText);

        EditorUtility.DisplayProgressBar("End", "Package Complete", 1);
        EditorUtility.ClearProgressBar();
        System.Diagnostics.Process.Start(outPath);
        Debug.Log("Packing completed, path -> " + outPath);
    }

    //
    [MenuItem("Tools/AB/Buil Release/Build all AB (Ignore goushen)", false, 201)]
    public static void BuilReleaseBundel()
    {
        buildAllABIgnoreGoushen(false);
    }
    [MenuItem("Tools/AB/Buil Release/Check all AB (Ignore goushen)", false, 201)]
    public static void CheckReleaseBundel()
    {
        buildAllABIgnoreGoushen(true);
    }
    ///
    ///Build All AB Ngoại trừ mode review (goushen)
    ///
    /// 
    [MenuItem("Tools/AB/build all AB (Ignore goushen)", false, 201)]
    private static void buildAllABIgnoreGoushen(bool isChecker)
    {
        EditorUtility.DisplayProgressBar("Nhắc nhở", "Sẵn sàng đóng gói", 0);
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (objs == null || objs.Length == 0)
        {
            EditorUtility.DisplayDialog("Prompt", "The current selection is invalid and cannot be packaged\nPlease select the resource or prefab in the Assets directory", "OK");
            EditorUtility.ClearProgressBar();
            return;
        }


        initPath();
        EditorUtility.DisplayProgressBar("Prepare", "Start Packaging", 0);
        getShaderList();
        //2021/09/03 HRP TOAN ADD STT
        string RootPath = "Assets/ArtResources/OutPut/";
        //List 1
        List<string> list1 = new List<string>()
        {
            "UI",
            "Other",
            "Scene",
            "Effect",
            "Model/other",
            "Model/Ship",
            "Model/ui",
            "ModelInside",
        };
        List<string> list2 = new List<string>()
        {
            "Model/monster",
        };
        List<string> list3 = new List<string>()
        {
            "Model/NewHeroModel",
        };
        Dictionary<int, List<string>> dicBuildAB = new Dictionary<int, List<string>>()
        {
            { 1, list1 },
            { 2, list2 },
            { 3, list3 },
        };
        // Phân Loại ngoại trừ, để chứa những object ko bao gồm trên

        List<string> ignoreNamePath = new List<string>()
        {
            "guoshen"
        };

        //
        Dictionary<int, List<Object>> dicBuildABObject = new Dictionary<int, List<Object>>();
        Dictionary<int, List<string>> dicBuildABPath = new Dictionary<int, List<string>>();

        //create dic
        foreach (var key in dicBuildAB.Keys)
        {
            dicBuildABObject.Add(key, new List<Object>());
            dicBuildABPath.Add(key, new List<string>());

        }
        //
        foreach (int listIndex in dicBuildAB.Keys)
        {
            foreach (var path in dicBuildAB[listIndex])
            {
                string path_obj = RootPath + path;
                if (!dicBuildABPath[listIndex].Contains(path_obj)) dicBuildABPath[listIndex].Add(path_obj);
            }

        }

        for (int i = 0; i < objs.Length; i++)
        {
            string path_obj = AssetDatabase.GetAssetPath(objs[i]);
            bool isAdd = false;
            //Neu khong nam trong danh sach ignore
            if (!isIgnorePath(path_obj, ignoreNamePath))//!Directory.Exists(path_obj) &&
            {
                foreach (int listIndex in dicBuildABPath.Keys)
                {
                    foreach (var rootPath in dicBuildABPath[listIndex])
                    {
                        if (path_obj.StartsWith(rootPath))
                        {
                            dicBuildABObject[listIndex].Add(objs[i]);
                            isAdd = true;
                            break;
                        }
                    }
                    if (isAdd)
                    {
                        break;
                    }
                }
            }
        }

        //
        StringBuilder checkText = new StringBuilder();
        int indexnumber = 0;

        if (isChecker)
        {
            //build AB
            foreach (var listAB in dicBuildABObject.Values)
            {
                indexnumber++;
                BuildOneAB(listAB.ToArray(), false, ref checkText, "", false, indexnumber);
                //foreach (var p in listAB)
                //{
                //    checkText.AppendLine(indexnumber+"."+p.name);
                //}
                //checkText.Append("=================================");
            }
            //Export file log
            string outputPath = Application.dataPath + "/../OutLogBundle/";
            if (!System.IO.Directory.Exists(outputPath))
                System.IO.Directory.CreateDirectory(outputPath);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputPath + "/logCheckAB.txt", false, new UTF8Encoding(false)))
            {
                sw.Write(checkText);
                sw.Flush();
            }

        }
        else
        {
            //build AB
            foreach (var listAB in dicBuildABObject.Values)
            {
                indexnumber++;
                BuildOneAB(listAB.ToArray(), true, ref checkText, "", false, indexnumber);

            }
        }
        //2021/09/03 HRP TOAN ADD END
        EditorUtility.DisplayProgressBar("End", "Package Complete", 1);
        EditorUtility.ClearProgressBar();
        System.Diagnostics.Process.Start(outPath);
        Debug.Log("Packing completed, path -> " + outPath);
    }
    private static bool isIgnorePath(string path, List<string> listIgnore)
    {
        string lowerPath = path.ToLower();
        for (int i = 0; i < listIgnore.Count; i++)
        {
            if (path.Contains(listIgnore[i]))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 打包打个文件，包含依赖
    /// </summary>
    public static void BuildOneAB(Object[] objs, bool isBuild, ref StringBuilder checkText, string optionOutPath = "", bool onlyCheckShader = false, int indexnumber =0)
    {
        //Kiểm tra shader, nếu check sẽ bỏ qua các lỗi
        if (checkHasDefaultRes(objs, ref checkText, isBuild))
            return;
        if (onlyCheckShader)
            return;
        //判断ModelInside
        List<Object> insideList = new List<Object>(objs);
        string pathRoot = "Assets/ArtResources/OutPut/";
        int maxobj = objs.Length;
        float index = 0f;
        foreach (var obj in objs)
        {
            index++;
            EditorUtility.DisplayProgressBar(string.Format("Process {0}: {1}", indexnumber, objs.Length)
                , index + "/" + maxobj, (float)index / maxobj);
            string path = AssetDatabase.GetAssetPath(obj);
            if (isBuild)
            {
                //材质球单独打包，如果renderQueue为-1可能无法还原renderQueue
                string[] objDeps = AssetDatabase.GetDependencies(path, true);
                foreach (var dep in objDeps)
                {
                    //剔除材质球
                    Object depObj = AssetDatabase.LoadAssetAtPath<Object>(dep);
                    if (depObj is Material)
                    {
                        ((Material)depObj).renderQueue = ((Material)depObj).renderQueue;
                        EditorUtility.SetDirty(depObj);
                    }
                }
            }


            //if (path.StartsWith(pathRoot + "ModelInside/"))
            //{
                //var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                //if (go == null)
                //    continue;
                //todo: Duong, Hiện tại ko dùng dep để load model inside nên comment
                //var dep = go.transform.Find("dep");
                //if (dep == null)
                //    continue;
                //for (int i = 0; i < dep.childCount; ++i)
                //{
                //    var child = dep.GetChild(i).name;
                //    var depObj = AssetDatabase.LoadAssetAtPath<Object>(pathRoot + "Model/" + child.Replace("@", "/") + ".prefab");
                //    if (depObj == null)
                //    {
                //        if (isBuild)
                //        {
                //            EditorUtility.DisplayDialog("Cảnh báo", "Không thế tìm thấy phụ thuộc modelInside>" + obj.name + ">" + child.Replace("@", "/") + ".prefab", "Fix");
                //            return;
                //        }
                //        else
                //        {
                //            checkText.AppendLine("Lỗi, ModelInside, không thế tìm thấy phụ thuộc modelInside>" + obj.name + ">" + child.Replace("@", "/") + ".prefab");
                //        }

                //    }
                //    if (!insideList.Contains(depObj))
                //        insideList.Add(depObj);
                //}
            //}
        }
        AssetDatabase.SaveAssets();

        string alwaysPath = "Assets/ArtResources/ArtBase/Effect";
        List<Object> inBuildList = new List<Object>();
        List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
        foreach (var obj in objs)
        {
            //HookCreater.CreateObjHook(obj as GameObject);

            string path = AssetDatabase.GetAssetPath(obj);

            if (path.StartsWith(pathRoot + "UI/"))
            {
                if (path.StartsWith(pathRoot + "UI/UIWindow/"))
                {
                    //UI分为界面和资源     文件夹不管
                    if (Directory.Exists(Application.dataPath + path.Substring(6)))
                        continue;

                    if (path.EndsWith(".bytes"))
                    {
                        //界面和UI sprite
                        AssetBundleBuild abBuild = new AssetBundleBuild();
                        abBuild.assetBundleName = obj.name.ToLower() + PathUtil.abSuffix;
                        abBuild.assetNames = new string[] { path };
                        buildList.Add(abBuild);
                    }
                    else
                    {
                        //图片资源
                        string prePath = path.Substring(0, path.Length - Path.GetFileName(path).Length);
                        string[] arr = obj.name.Split('!');
                        string ui = arr[0];
                        if (File.Exists(Application.dataPath + prePath.Substring(6) + ui + ".png"))
                        {
                            List<string> list2 = new List<string>();
                            var sObj = AssetDatabase.LoadAssetAtPath<Object>(prePath + ui + ".png");
                            if (!inBuildList.Contains(sObj))
                            {
                                inBuildList.Add(sObj);
                                list2.Add(prePath + ui + ".png");
                                if (File.Exists(Application.dataPath + prePath.Substring(6) + ui + "!a.png"))
                                {
                                    var aObj = AssetDatabase.LoadAssetAtPath<Object>(prePath + ui + "!a.png");
                                    if (!inBuildList.Contains(aObj))
                                    {
                                        inBuildList.Add(aObj);
                                        list2.Add(prePath + ui + "!a.png");
                                    }
                                }
                                AssetBundleBuild abBuild = new AssetBundleBuild();
                                abBuild.assetBundleName = ui + PathUtil.abSuffix;
                                abBuild.assetNames = list2.ToArray();
                                buildList.Add(abBuild);
                            }
                        }
                    }
                }
                else if (path.StartsWith(pathRoot + "UI/UISingle/"))
                {
                    //排除文件夹
                    if (Directory.Exists(Application.dataPath + path.Substring(6)))
                        continue;

                    //大的UI散图
                    string suffix = Path.GetExtension(path).ToLower();
                    if (suffix == ".png" || suffix == ".jpg" || suffix == ".jpeg" || suffix == ".tga")
                    {
                        AssetBundleBuild abBuild = new AssetBundleBuild();
                        abBuild.assetBundleName = obj.name.ToLower() + PathUtil.abSuffix;
                        abBuild.assetNames = new string[] { path };
                        buildList.Add(abBuild);
                    }
                }
                else
                {
                    //ui纹理 不是文件夹不管
                    if (!Directory.Exists(Application.dataPath + path.Substring(6)))
                        continue;

                    List<string> list = new List<string>();
                    DirectoryInfo info = new DirectoryInfo(Application.dataPath + path.Substring(6));
                    var files = info.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        if (!file.Name.EndsWith(".meta"))
                        {
                            string filePath = file.FullName.Substring(Application.dataPath.Length - 6);
                            list.Add(filePath);
                        }
                    }
                    AssetBundleBuild abBuild = new AssetBundleBuild();
                    abBuild.assetBundleName = obj.name.ToLower() + PathUtil.abSuffix;
                    abBuild.assetNames = list.ToArray();
                    buildList.Add(abBuild);
                }
            }
            else if (path.StartsWith(pathRoot + "Scene/"))
            {
                //Scene all split
                //It’s not a prefab.
                if (false == path.ToLower().EndsWith(".prefab"))
                    continue;

                string[] deps = AssetDatabase.GetDependencies(path, true);
                foreach (var dep in deps)
                {
                    if (!Buildable(AssetDatabase.LoadAssetAtPath<Object>(dep)))
                        continue;

                    //剔除材质球
                    Object depObj = AssetDatabase.LoadAssetAtPath<Object>(dep);
                    if (depObj == null || depObj is Material || depObj is TextAsset || depObj is AnimationClip || depObj is RuntimeAnimatorController)
                        continue;

                    if (depObj != obj && dep.ToLower().EndsWith(".prefab"))
                    {
                        if (isBuild)
                        {
                            EditorUtility.DisplayDialog("Cảnh báo", "Không cho phép GameObject bị phụ thuộc" + obj.name + ">" + depObj.name, "Fix");
                            return;
                        }
                        else
                        {
                            checkText.AppendLine("Lỗi, Scene, không cho phép GameObject bị phụ thuộc" + obj.name + ">" + depObj.name);
                        }

                    }

                    if (!inBuildList.Contains(depObj))
                    {
                        inBuildList.Add(depObj);
                        string depName = Path.GetFileNameWithoutExtension(dep);
                        AssetBundleBuild abb = new AssetBundleBuild();
                        abb.assetBundleName = depName.ToLower() + PathUtil.abSuffix;
                        abb.assetNames = new string[] { dep };
                        buildList.Add(abb);
                    }
                }
            }
            else if (path.StartsWith(pathRoot + "Model/"))
            {
                //The model is divided into three parts: prefabs, textures, and others

                //It’s not a prefab.
                if (false == path.ToLower().EndsWith(".prefab"))
                    continue;

                string[] deps = AssetDatabase.GetDependencies(path, true);
                List<string> otherList = new List<string>();
                foreach (var dep in deps)
                {
                    if (!Buildable(AssetDatabase.LoadAssetAtPath<Object>(dep)))
                        continue;

                    string depName = Path.GetFileNameWithoutExtension(dep);
                    Object depObj = AssetDatabase.LoadAssetAtPath<Object>(dep);
                    if (depObj == null)
                        continue;

                    if (depObj != obj && dep.ToLower().EndsWith(".prefab"))
                    {
                        if (isBuild)
                        {
                            //Special model of timeline
                            EditorUtility.DisplayDialog("Cảnh báo", "Không cho phép GameObject bị phụ thuộc" + obj.name + ">" + depObj.name, "Fix");
                            return;
                        }
                        else
                        {
                            checkText.AppendLine("Lỗi, Model,  không cho phép GameObject bị phụ thuộc" + obj.name + ">" + depObj.name);
                        }

                    }

                    if (depObj is Texture || (depObj is GameObject && !dep.ToLower().EndsWith(".fbx")) || dep.StartsWith(alwaysPath))
                    {
                        if (!inBuildList.Contains(depObj))
                        {
                            var abName = depName.ToLower();
                            if (depObj is Material)
                                abName = "mat_" + abName;

                            inBuildList.Add(depObj);
                            AssetBundleBuild abBuild = new AssetBundleBuild();
                            abBuild.assetBundleName = abName + PathUtil.abSuffix;
                            abBuild.assetNames = new string[] { dep };
                            buildList.Add(abBuild);
                        }
                    }
                    else if (depObj is TextAsset)
                    {
                        //Logic split to prefab together
                    }
                    else
                    {
                        otherList.Add(dep);
                    }
                }

                //Put other types into ab
                if (otherList.Count > 0)
                {
                    AssetBundleBuild abb = new AssetBundleBuild();
                    string otherAbName = obj.name.ToLower().Substring(obj.name.IndexOf("_") + 1) + "_other_res";
                    abb.assetBundleName = otherAbName + PathUtil.abSuffix;
                    abb.assetNames = otherList.ToArray();

                    bool added = false;
                    foreach (var build in buildList)
                    {
                        if (build.assetBundleName == abb.assetBundleName)
                        {
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                        buildList.Add(abb);
                }
            }
            else if (path.StartsWith(pathRoot + "ModelInside/"))
            {
                //包含模型的资源特殊处理一波
                string[] deps = AssetDatabase.GetDependencies(path, true);
                foreach (var dep in deps)
                {
                    if (!Buildable(AssetDatabase.LoadAssetAtPath<Object>(dep)))
                        continue;
                    if (dep.StartsWith("Assets/ArtResources/ArtBase/Model/") && !dep.StartsWith("Assets/ArtResources/ArtBase/Model/other"))
                        continue;

                    //材质球和动画部剔除
                    Object depObj = AssetDatabase.LoadAssetAtPath<Object>(dep);
                    if (depObj == null || depObj is Material || depObj is AnimationClip || depObj is RuntimeAnimatorController || depObj is TextAsset)
                        continue;

                    if (!inBuildList.Contains(depObj))
                    {
                        inBuildList.Add(depObj);
                        string depName = Path.GetFileNameWithoutExtension(dep);
                        AssetBundleBuild abb = new AssetBundleBuild();
                        abb.assetBundleName = depName.ToLower() + PathUtil.abSuffix;
                        abb.assetNames = new string[] { dep };
                        buildList.Add(abb);
                    }
                }
            }
            else if (path.StartsWith(pathRoot + "Effect/"))
            {
                //特效只有材质球不拆分
                string[] deps = AssetDatabase.GetDependencies(path, true);
                foreach (var dep in deps)
                {
                    if (!Buildable(AssetDatabase.LoadAssetAtPath<Object>(dep)))
                        continue;

                    //剔除材质球和动画
                    Object depObj = AssetDatabase.LoadAssetAtPath<Object>(dep);
                    if (depObj == null || depObj is AnimationClip || depObj is RuntimeAnimatorController || depObj is TextAsset)
                        continue;

                    if (depObj != obj && dep.ToLower().EndsWith(".prefab"))
                    {
                        if (isBuild)
                        {
                            EditorUtility.DisplayDialog("Cảnh báo", "Không cho phép GameObject bị phụ thuộc" + obj.name + ">" + depObj.name, "Fix");
                            return;
                        }
                        else
                        {
                            checkText.AppendLine("Lỗi, Effect,  không cho phép GameObject bị phụ thuộc" + obj.name + ">" + depObj.name);

                        }

                    }

                    if (!inBuildList.Contains(depObj))
                    {
                        inBuildList.Add(depObj);
                        string depName = Path.GetFileNameWithoutExtension(dep);
                        var abName = depName.ToLower();
                        if (depObj is Material)
                            abName = "mat_" + abName;

                        inBuildList.Add(depObj);
                        AssetBundleBuild abb = new AssetBundleBuild();
                        abb.assetBundleName = abName + PathUtil.abSuffix;
                        abb.assetNames = new string[] { dep };
                        buildList.Add(abb);
                    }
                }
            }
            else if (path.StartsWith(pathRoot + "Other/"))
            {
                //Những người khác tuân theo các quy tắc mặc định, không có cách đặt tên đặc biệt nào sẽ được chia nhỏ tất cả
                if (Directory.Exists(Application.dataPath + path.Substring(6)) && path.EndsWith("_wjj"))
                {
                    //Folder packaging
                    DirectoryInfo dir = new DirectoryInfo(Application.dataPath + path.Substring(6));
                    List<string> fileInFolder = new List<string>();
                    foreach (var file in dir.GetFiles("*.*", SearchOption.TopDirectoryOnly))
                    {
                        string assetPath = file.FullName.Substring(Application.dataPath.Length - 6);
                        if (Buildable(AssetDatabase.LoadAssetAtPath<Object>(assetPath)))
                        {
                            fileInFolder.Add(assetPath);
                        }
                    }

                    AssetBundleBuild abb = new AssetBundleBuild();
                    abb.assetBundleName = obj.name.ToLower() + PathUtil.abSuffix;
                    abb.assetNames = fileInFolder.ToArray();
                    buildList.Add(abb);
                    continue;
                }
                if (!Buildable(obj))
                    continue;

                string[] deps = AssetDatabase.GetDependencies(path, true);
                List<string> depList = new List<string>();
                foreach (var dep in deps)
                {
                    if (!Buildable(AssetDatabase.LoadAssetAtPath<Object>(dep)))
                        continue;

                    var depObj = AssetDatabase.LoadAssetAtPath<Object>(dep);
                    if (depObj != obj && dep.ToLower().EndsWith(".prefab"))
                    {
                        if (isBuild)
                        {
                            EditorUtility.DisplayDialog("Cảnh báo", "Không cho phép GameObject bị phụ thuộc" + obj.name + ">" + depObj.name, "Fix");
                            return;
                        }
                        else
                        {
                            checkText.AppendLine("Lỗi, Other,  không cho phép GameObject bị phụ thuộc" + obj.name + ">" + depObj.name);

                        }

                    }

                    string folder = Path.GetDirectoryName(path);
                    if (folder.EndsWith("_wjj"))
                        continue;

                    string depName = Path.GetFileNameWithoutExtension(dep);
                    if (depName.EndsWith("_bcf") || dep == path)
                    {
                        //不拆分的资源
                        depList.Add(dep);
                    }
                    else if (depName.EndsWith("_bdb"))
                    {
                        //不打包的资源
                        continue;
                    }
                    else
                    {
                        AssetBundleBuild abb = new AssetBundleBuild();
                        abb.assetBundleName = depName.ToLower() + PathUtil.abSuffix;
                        abb.assetNames = new string[] { dep };
                        buildList.Add(abb);
                    }
                }

                //批量打包
                AssetBundleBuild abBuild = new AssetBundleBuild();
                abBuild.assetBundleName = obj.name.ToLower() + PathUtil.abSuffix;
                abBuild.assetNames = depList.ToArray();
                buildList.Add(abBuild);
            }
        }

        foreach (var build in buildList)
        {
            foreach (var build2 in buildList)
            {
                foreach (var name in build2.assetNames)
                {
                    if (build.assetBundleName == build2.assetBundleName)
                        continue;

                    foreach (var name2 in build.assetNames)
                    {
                        if (name == name2)
                        {
                            Debuger.Err(string.Format("Lỗi, 2 asset bundle cùng tên: BundleName: {0} , AB1 : {1} , AB2: {2}", name, build.assetBundleName, build2.assetBundleName));
                            checkText.AppendLine(string.Format("Lỗi, 2 asset bundle cùng tên: BundleName: {0} , AB1 : {1} , AB2: {2}", name, build.assetBundleName, build2.assetBundleName));

                        }
                    }
                }
            }
        }

        if (isBuild)
        {
            if (string.IsNullOrEmpty(optionOutPath))
                optionOutPath = outPath;
            if (!Directory.Exists(optionOutPath))
                Directory.CreateDirectory(optionOutPath);

            //buildList.Add(getShaderListBuild());
            BuildPipeline.BuildAssetBundles(optionOutPath, buildList.ToArray(), buildOptions, EditorUserBuildSettings.activeBuildTarget);
            ChangeDependenceConf(optionOutPath);
        }
    }
    private static void SaveLogCheckAB(StringBuilder stringBuilder)
    {

    }

    private static AssetBundleBuild getShaderListBuild()
    {
        var deps = AssetDatabase.GetDependencies(shaderListPath);
        List<string> list = new List<string>();
        foreach (var dep in deps)
        {
            if (dep.EndsWith(".shader"))
                list.Add(dep);
        }
        list.Add(shaderListPath);
        var shaderAbd = new AssetBundleBuild();
        shaderAbd.assetBundleName = "shader_list" + PathUtil.abSuffix;
        shaderAbd.assetNames = list.ToArray();
        return shaderAbd;
    }

    private static bool checkException(Dictionary<Object, string> buildMap, Object obj, string abName)
    {
        return false;
    }

    /// <summary>
    /// 是否包含Unity自带资源(包含引用)
    /// </summary>
    private static bool checkHasDefaultRes(Object[] objs, ref StringBuilder checkText, bool isBuild = true)
    {
        List<string> listShaderError = new List<string>();
        if (objs == null || objs.Length == 0) return true;
        foreach (var obj in objs)
        {
            var depObjs = EditorUtility.CollectDependencies(new Object[] { obj });
            foreach (var dep in depObjs)
            {
                if (dep is Shader)
                {
                    //shader
                    if (!shaderList.Contains(dep as Shader))
                    {
                        if (isBuild)
                        {
                            EditorUtility.DisplayDialog("Prompt", obj.name + "Trích dẫn shader:" + dep.name + "\n Vui lòng thêm shader vào shaderList trước rồi đóng gói lại:" + AssetDatabase.GetAssetPath(dep), "OK");
                            return true;
                        }
                        else
                        {
                            if (!listShaderError.Contains(dep.name))
                            {
                                listShaderError.Add(dep.name);
                            }
                            checkText.AppendLine("Lỗi check Shader: Object lỗi: " + obj.name + " Trích dẫn shader: " + dep.name);
                        }


                    }
                    if (dep.name == "Standard")
                    {
                        if (isBuild)
                        {
                            EditorUtility.DisplayDialog("Nhắc", obj.name + "đề cập đến Standard.shader, vui lòng thay tiêu chuẩn bằng một trình đổ bóng khác", "OK");
                            return true;
                        }
                        else
                        {
                            if (!listShaderError.Contains(dep.name))
                            {
                                listShaderError.Add(dep.name);
                            }
                            checkText.AppendLine("Lỗi check Shader: Object lỗi: " + obj.name + " đang sử dụng Standard.shader");
                        }


                    }
                }
                else if (AssetDatabase.GetAssetPath(dep).ToLower().Contains("unity_builtin_extra"))
                {
                    if (dep is Material)
                    {
                        //Shader tạm thời được cho phép
                        Debuger.Wrn("Tài nguyên tham chiếu tài nguyên riêng của Unity, shader>", dep.name);
                        //checkText.AppendLine("Warning Tài nguyên tham chiếu tài nguyên riêng của Unity: " + dep.name);
                    }
                    else
                    {
                        if (isBuild)
                        {
                            Debuger.Err("Tài nguyên tham khảo tài nguyên riêng của Unity, vui lòng đóng gói lại sau khi sửa đổi");
                            EditorUtility.DisplayDialog("Cảnh báo", string.Format("Tài nguyên đề cập đến tài nguyên riêng của Unity, vui lòng thay đổi và đóng gói lại \n {0} chứa {1}", obj.name, dep.name), "Fix");
                            return true;
                        }
                        else
                        {
                            if (!listShaderError.Contains(dep.name))
                            {
                                listShaderError.Add(dep.name);
                            }
                            checkText.AppendLine(string.Format("Lỗi, Tài nguyên đề cập đến tài nguyên riêng của Unity: {0} chứa {1}", obj.name, dep.name));
                        }

                    }
                }
            }
        }
        checkText.AppendLine("Shader list lỗi: " + System.String.Join(", ", listShaderError.ToArray()));

        return false;
    }

    /// <summary>
    /// 改变依赖文件格式
    /// </summary>
    private static void ChangeDependenceConf(string outputPath)
    {
        var abPath = outputPath + "/" + Path.GetFileName(outputPath).ToLower();
        AssetBundle ab = AssetBundle.LoadFromFile(abPath);
        if (ab == null) return;
        var abm = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        if (abm == null) return;
        var arr = abm.GetAllAssetBundles();
        foreach (var name in arr)
        {
            List<string> depList = new List<string>();
            var deps = abm.GetDirectDependencies(name);
            for (int i = 0; i < deps.Length; ++i)
            {
                string dep = Path.GetFileNameWithoutExtension(deps[i]);
                depList.Add(dep);
            }
            if (depList.Count > 0)
                File.WriteAllLines(outputPath + "/" + Path.GetFileNameWithoutExtension(name) + ".d", depList.ToArray());
        }
        ab.Unload(true);

        File.Delete(abPath);
        File.Delete(abPath + ".manifest");
    }

    /// <summary>
    /// 是否可以被打包
    /// </summary>
    public static bool Buildable(Object obj, bool log = true)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        if (path.ToLower().EndsWith(".asset"))
        {
            Object depObj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (depObj.GetType().FullName.Contains("UnityEditor"))
            {
                if (log)
                    Debuger.Log("Editor Only资源不能打包，跳过 > ", path);
                return false;
            }
        }
        return Buildable(path);
    }

    /// <summary>
    /// 是否可以被打包
    /// </summary>
    public static bool Buildable(string path)
    {
        string name = Path.GetFileName(path).ToLower();
        if (name.EndsWith(".png")          //纹理
            || name.EndsWith(".jpg")          //纹理
            || name.EndsWith(".jpeg")          //纹理
            || name.EndsWith(".tga")          //纹理
            || name.EndsWith(".psd")          //纹理

            || name.EndsWith(".mat")          //材质

            || name.EndsWith(".ogg")          //声音
            || name.EndsWith(".mp3")          //声音
            || name.EndsWith(".wav")          //声音

            || name.EndsWith(".mp4")          //视频
            || name.EndsWith(".flv")          //视频

            || name.EndsWith(".prefab")       //预制件
            || name.EndsWith(".anim")         //动画
            || name.EndsWith(".controller")   //动画控制器
            || name.EndsWith(".mesh")         //mesh文件
            || name.EndsWith(".bytes")        //二进制文件
            || name.EndsWith(".asset")        //其它文件

            || name.EndsWith(".ttf")        //字体文件
            || name.EndsWith(".fontsettings") //图片字体文件

            || name.EndsWith(".fbx")            //fbx
                                                //|| name.EndsWith(".unity")      //场景
                                                //|| name.EndsWith(".exr")        //场景光照贴图
            )
        {
            return true;
        }
        return false;
    }
}