/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.10.20
*/

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class PlayerBuilder
{
    [MenuItem("Tools/Player/TestABDll", false, 1)]
    public static void BuildTestABDll()
    {
        var path = Application.dataPath + "/../../code/trunk/GameLogic/";
        var dllBat = new System.Diagnostics.Process();
        dllBat.StartInfo.WorkingDirectory = path;
        dllBat.StartInfo.FileName = path + "build_android_release.bat";
        dllBat.StartInfo.Arguments = "close";
        dllBat.Start();
        dllBat.WaitForExit();
        Debuger.Log("Dll导出完成");

        string outputPath = PathUtil.GetForceABPath() + "../update";
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        EditorUtility.DisplayProgressBar("打包", "打包Dll", 0);
        PathUtil.codeOffset = PathUtil.scriptOffset;
        PathUtil.codeKey = PathUtil.scriptKey;
        BuildAsBundle(PathUtil.EditorDllPath, outputPath, PathUtil.DllScriptBundleName);
        EditorUtility.ClearProgressBar();
        System.Diagnostics.Process.Start(outputPath);
    }
    [MenuItem("Tools/Player/Decode Dll", false, 1)]
    public static void DecodeABDll()
    {
        string dest = Application.dataPath + "/buildTmp/ds_pkg_711_.bytes";
        string destDll = Application.dataPath + "/buildTmp/ds_pkg_711_dll.bytes";

        //Directory.CreateDirectory(destDir);
        //File.Copy(path, dest, true);
        PathUtil.codeOffset = PathUtil.scriptOffset;
        PathUtil.codeKey = PathUtil.scriptKey;
        var bytes = File.ReadAllBytes(dest);
        PathUtil.Decode(bytes);
        File.WriteAllBytes(destDll, bytes);

        AssetDatabase.Refresh();

    }

    public static void BuildUpdate()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            BuildUpdateResource(BuildTarget.Android);
        else
            BuildUpdateResource(BuildTarget.iOS);
    }

    [MenuItem("Tools/Player/TestBuildUpdate", false, 1)]

    public static void BuildUpdate_WithOutDll()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            BuildUpdateResource(BuildTarget.Android, false);
        else
            BuildUpdateResource(BuildTarget.iOS, false);
    }

    public static bool ResourceConfuse = true;
    private const string lanaugeRef = "chinese";
    private static SimpleJSON.JSONClass nameArrJson;
    private static string getRandomFileContentStr(int minLen = 3, int maxLen = 20)
    {
        int len = UnityEngine.Random.Range(minLen, maxLen);
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < len; ++i)
            sb.Append((char)UnityEngine.Random.Range(0, 128));
        return sb.ToString();
    }

    public static string getRandomName(bool isRes = true, int minLen = 2, int maxLen = 5)
    {
        if (nameArrJson == null)
        {
#if PGAME
            var path = Application.dataPath + "/Hotupdate Assets/PluginAssets/Resource/Editor/mix_word_list.json";
#else
            var path = Application.dataPath + "/PluginAssets/Resource/Editor/mix_word_list.json";
#endif
            if (File.Exists(path))
                nameArrJson = SimpleJSON.JSONArray.Parse(File.ReadAllText(path)) as SimpleJSON.JSONClass;
        }

        var ret = "";
        int num = UnityEngine.Random.Range(minLen, maxLen);
        if (isRes)
        {
            //Resources
            var len = nameArrJson["res"].Count;
            int idx = UnityEngine.Random.Range(0, len);
            ret = nameArrJson["res"][idx] + "_";
            for (int i = 0; i < num; ++i)
            {
                len = nameArrJson["name"].Count;
                idx = UnityEngine.Random.Range(0, len);
                ret += nameArrJson["name"][idx];
                if (i < num - 1)
                    ret += "_";
            }
            ret = ret.ToLower(); //All lowercase
        }
        else
        {
            //Function name
            for (int i = 0; i < num; ++i)
            {
                int len = nameArrJson["name"].Count;
                int idx = UnityEngine.Random.Range(0, len);

                var w = nameArrJson["name"][idx].Value;
                if (string.IsNullOrEmpty(ret))
                    ret = w;
                else
                    ret += w[0].ToString().ToUpper() + w.Substring(1).ToLower();//驼峰
            }
        }

        if (ret.Length > 3)
            return ret;

        return "xyz";
    }

    public static void BuildUpdateResource(BuildTarget target, bool includeDll = true)
    {
        if (EditorUserBuildSettings.activeBuildTarget != target)
        {
            if (target == BuildTarget.Android)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, target);
            else
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, target);
        }

        string outputPath = PathUtil.GetForceABPath() + "../update";
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        var mFiles = Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories);
        if (mFiles.Length > 0)
        {
            Debug.LogError("Note that the update directory is not empty");
            //Directory.Delete(outputPath, true);
        }

        if (includeDll)
        {
            EditorUtility.DisplayProgressBar("Package", "Package Dll", 0);
            PathUtil.codeOffset = PathUtil.scriptOffset;
            PathUtil.codeKey = PathUtil.scriptKey;
            BuildAsBundle(PathUtil.EditorDllPath, outputPath, PathUtil.DllScriptBundleName,true);
        }

        EditorUtility.DisplayProgressBar("Package", "Package Update Configuration Table", 0.3f);
        PathUtil.codeOffset = PathUtil.binOffset;
        PathUtil.codeKey = PathUtil.binKey;
        BuildDirToAB(PathUtil.GetForceABPath() + "../_bean/", outputPath, "*.bytes", "_fix_" + PathUtil.UpdateBeanSuffix, false, false);

        EditorUtility.DisplayProgressBar("Package", "Organize and update ab dependencies", 0.5f);
        MergeABDependence(new string[] { PathUtil.GetForceABPath(), PathUtil.GetBackABPath() }, outputPath, "_fix_" + PathUtil.UpdateDepSuffix);

        if (!Directory.Exists(PathUtil.GetForceABPath() + "../_uiWindow/"))
            Directory.CreateDirectory(PathUtil.GetForceABPath() + "../_uiWindow/");
        var files = Directory.GetFiles(PathUtil.GetForceABPath() + "../_uiWindow/", "*.*", SearchOption.AllDirectories);
        if (files.Length > 0)
        {
            EditorUtility.DisplayProgressBar("Package", "Package Update UI", 0.7f);
            string uiDestPath = Application.dataPath + "/ArtResources/OutPut/UI/UIWindow/";
            if (Directory.Exists(uiDestPath))
            {
                mFiles = Directory.GetFiles(uiDestPath, "*.*", SearchOption.AllDirectories);
                if (mFiles.Length > 0)
                    Debug.LogError("Note that OutPut/UI/UIWindow/ is not empty");
                //Directory.Delete(uiDestPath, true);
            }
            else
            {
                Directory.CreateDirectory(uiDestPath);
            }

            //Root directory
            files = Directory.GetFiles(PathUtil.GetForceABPath() + "../_uiWindow/", "*.*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                string dest = uiDestPath + fileName;
                File.Copy(files[i], dest, true);
            }
            AssetDatabase.Refresh();
            List<UnityEngine.Object> uiList = new List<UnityEngine.Object>();
            files = Directory.GetFiles(uiDestPath, "*.*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".png") || files[i].EndsWith(".jpg") || files[i].EndsWith(".jpeg") || files[i].EndsWith(".bytes"))
                {
                    var aPath = files[i].Substring(Application.dataPath.Length - 6);
                    var refPath = "Assets/Resources/UITextures/chinese/formatAndroid.png";
                    if (target == BuildTarget.iOS)
                        refPath = "Assets/Resources/UITextures_IOS/chinese/formatIOS.png";
                    if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(refPath) == null)
                    {
                        refPath = "Assets/Resources/UITextures/" + Path.GetFileName(aPath);
                        if (target == BuildTarget.iOS)
                            refPath = "Assets/Resources/UITextures_IOS/" + Path.GetFileName(aPath);
                        if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(refPath) == null)
                        {
                            refPath = "Assets/Resources/UITextures/english/" + Path.GetFileName(aPath);
                            if (target == BuildTarget.iOS)
                                refPath = "Assets/Resources/UITextures_IOS/english/" + Path.GetFileName(aPath);
                        }
                    }
                    Debuger.Log("refPath " + refPath);
                    //ResFormatMaker.CopySetting(refPath, aPath, target);

                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(aPath);
                    uiList.Add(obj);

                }
            }
            AssetDatabase.Refresh();
            var log = new StringBuilder();
            ABBuilder.BuildOneAB(uiList.ToArray(),true, ref log, outputPath);
            Directory.Delete(uiDestPath, true);
            AssetDatabase.Refresh();

            //Danh mục đa ngôn ngữ
            var dirs = Directory.GetDirectories(PathUtil.GetForceABPath() + "../_uiWindow/", "*", SearchOption.TopDirectoryOnly);
            foreach (var child in dirs)
            {
                Directory.CreateDirectory(uiDestPath);
                var folder = Path.GetFileName(child);
                files = Directory.GetFiles(child, "*.*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = Path.GetFileName(files[i]);
                    string dest = uiDestPath + fileName;
                    File.Copy(files[i], dest, true);
                }
                AssetDatabase.Refresh();
                List<UnityEngine.Object> uiList1 = new List<UnityEngine.Object>();
                files = Directory.GetFiles(uiDestPath, "*.*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].EndsWith(".png") || files[i].EndsWith(".jpg") || files[i].EndsWith(".jpeg") || files[i].EndsWith(".bytes"))
                    {
                        var aPath = files[i].Substring(Application.dataPath.Length - 6);
                        var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(aPath);
                        uiList1.Add(obj);
                    }
                }
                if (!Directory.Exists(outputPath + "/" + folder))
                    Directory.CreateDirectory(uiDestPath + "/" + folder);
                AssetDatabase.Refresh();
                ABBuilder.BuildOneAB(uiList1.ToArray(), true,ref log, outputPath + "/" + folder);
                Directory.Delete(uiDestPath, true);
                AssetDatabase.Refresh();
            }
        }

        EditorUtility.DisplayProgressBar("Package", "Package to update multi-language configuration", 0.9f);
        MakeDeflanguageConf(target, null, outputPath, "_ref_" + PathUtil.UpdateLanguageSuffix);

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        var finalFiles = Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories);
        foreach (var file in finalFiles)
        {
            if (file.EndsWith(".manifest"))
                File.Delete(file);
        }
        System.Diagnostics.Process.Start(outputPath);
    }

    //多语言资源列表
    public static void MakeDeflanguageConf(BuildTarget tartget, List<string> removeLanList, string outPath, string outName, int size = 1024 * 1024 * 5)
    {
        string tempPath = Application.dataPath + "/" + Path.GetFileNameWithoutExtension(outName) + ".bytes";
        if (File.Exists(tempPath))
            File.Delete(tempPath);

        Dictionary<string, bool> confMap = new Dictionary<string, bool>();
        var dicArr = new string[] { PathUtil.GetForceABPath(), PathUtil.GetBackABPath(), PathUtil.EditorUIABPath };

        List<string> langaugeList = new List<string>();
        foreach (var dicPath in dicArr)
        {
            var fileList = new List<string>(Directory.GetFiles(dicPath, "*.*", SearchOption.AllDirectories));
            int dicLen = new DirectoryInfo(dicPath).FullName.Length;
            foreach (var file in fileList)
            {
                var name = new FileInfo(file).FullName;
                if (name.ToLower().EndsWith(".d")
                    || name.ToLower().EndsWith(".meta")
                    || name.ToLower().EndsWith(".manifest"))
                    continue;

                var tPath = name.Substring(dicLen);
                tPath = tPath.Replace("\\", "/");
                if (tPath.IndexOf("/") > 0)
                {
                    var folder = Path.GetDirectoryName(tPath);
                    //Debug.Log("folder " + folder + " |||| lanaugeRef:" + lanaugeRef);

                    if (folder == lanaugeRef)
                        confMap[Path.GetFileNameWithoutExtension(tPath)] = true;

                    if (!langaugeList.Contains(folder))
                    {
                        //出包才加入列表
                        if (removeLanList != null && !removeLanList.Contains(folder))
                            langaugeList.Add(folder);
                    }
                }
            }
        }
        Debug.Log("confMap " + confMap.Count);
        if (confMap.Count == 0)
            return;

        try
        {
            int offset = 0;
            var bytes = new byte[size];
            XBuffer.WriteInt(confMap.Count, bytes, ref offset);
            foreach (var kv in confMap)
                XBuffer.WriteString(kv.Key, bytes, ref offset);

            //包含哪些语言
            XBuffer.WriteInt(langaugeList.Count, bytes, ref offset);
            foreach (var lang in langaugeList)
                XBuffer.WriteString(lang, bytes, ref offset);

            Debug.Log("多语言配置文件列表大小>" + offset / 1024 + "kb");

            var stream = new FileInfo(tempPath).OpenWrite();
            stream.Write(bytes, 0, offset);
            stream.Flush();
            stream.Close();
            stream.Dispose();
        }
        catch (Exception e)
        {
            if (e is XBufferOutOfIndexException)
                MakeDeflanguageConf(tartget, removeLanList, outPath, outName, size * 2);
            return;
        }
        AssetDatabase.Refresh();

        AssetBundleBuild abb = new AssetBundleBuild();
        abb.assetBundleName = outName;
        abb.assetNames = new string[] { tempPath.Substring(tempPath.IndexOf("Assets/")) };
        BuildPipeline.BuildAssetBundles(outPath, new AssetBundleBuild[] { abb }, ABBuilder.buildOptions, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();

        File.Delete(tempPath);
        DirectoryInfo info = new DirectoryInfo(outPath);
        File.Delete(outPath + "/" + info.Name);
        File.Delete(outPath + "/" + info.Name + ".manifest");
        File.Delete(outPath + "/" + outName + ".manifest");
    }


    public static string PlayerBuildTempPath = Application.dataPath + "/../_pb_tmp/";
    public static bool MakeForceAsBib(string curLanguage, List<string> removeLanList, int size = 1024 * 1024 * 1000)
    {
        Dictionary<string, string> lastForceMap = new Dictionary<string, string>();

        var singleUIList = new List<string>(new string[] { "chinese" });

        //var uiDir = new DirectoryInfo(PathUtil.EditorUIABPath);
        var forceDir = new DirectoryInfo(PathUtil.GetForceABPath());
        //var backDir = new DirectoryInfo(PathUtil.GetBackABPath());

        // ui, được xử lý riêng
        // ui ngôn ngữ đơn không chứa tệp thư mục gốc
        // Không một ngôn ngữ nào ui chứa thư mục gốc và các tệp trong thư mục
        var abList = new List<FileInfo>();
        //if (!singleUIList.Contains(curLanguage))
        //    abList.AddRange(uiDir.GetFiles("*" + PathUtil.abSuffix, SearchOption.TopDirectoryOnly));
        // Thư mục ngôn ngữ hiện tại
        //var curUIDir = new DirectoryInfo(PathUtil.EditorUIABPath + curLanguage + "/");
        //if (curUIDir.Exists)
        //    abList.AddRange(curUIDir.GetFiles("*" + PathUtil.abSuffix, SearchOption.AllDirectories));
        //Tạo ngôn ngữ khác ngoài ngôn ngữ mặc định trong danh sách include
        //var dirArr = uiDir.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
        //foreach (var dir in dirArr)
        //{
        //    var dirName = Path.GetFileName(dir.FullName).ToLower();
        //    // Loại trừ ngôn ngữ hiện tại hoặc các ngôn ngữ không được bao gồm
        //    if (dirName == curLanguage || removeLanList.Contains(dirName))
        //    continue;
        //    curUIDir = new DirectoryInfo(PathUtil.EditorUIABPath + dirName + "/");
        //    abList.AddRange(curUIDir.GetFiles("*" + PathUtil.abSuffix, SearchOption.AllDirectories));
        //}
        int uiResLen = abList.Count;

        //force
        abList.AddRange(forceDir.GetFiles("*" + PathUtil.abSuffix, SearchOption.AllDirectories)
            .Where(name => !name.FullName.EndsWith(".manifest")));
        int forceResLen = abList.Count;

        Debuger.Log("abList size " + abList.Count);
        Debuger.Log("abList forceDir " + forceDir);
        ////Liệu tài nguyên trở lại có vào gói đầu tiên hay không
        //if (containsBack)
        //    abList.AddRange(backDir.GetFiles("*" + PathUtil.abSuffix, SearchOption.AllDirectories));

        var fileMap = new Dictionary<string, string>();
        for (int i = 0; i < abList.Count; ++i)
        {
            var file = abList[i];
            //var tPath = file.FullName.Substring(uiDir.FullName.Length);
            var tPath = "";
            if (i >= uiResLen)
                tPath = file.FullName.Substring(forceDir.FullName.Length);
            //if (i >= forceResLen)
            //    tPath = file.FullName.Substring(backDir.FullName.Length);

            string folderName = "", folderNamePath = "";
            tPath = tPath.Replace("\\", "/");
            if (tPath.Contains("/"))
            {
                folderName = tPath.Split('/')[0];
                folderNamePath = folderName + "/";
            }

            //Không bao gồm ngôn ngữ này
            //if (removeLanList.Contains(folderName))
            //    continue;

            //Ngôn ngữ hiện tại được sao chép vào thư mục gốc
            if (folderName == curLanguage)
            {
                tPath = tPath.Remove(0, folderNamePath.Length);
                folderNamePath = "";
            }
            fileMap[tPath] = file.FullName;
        }
        Debuger.Log("fileMap size: " + fileMap.Count);
        //ab temporary>dll/bean/def, etc.
        //var arr = new DirectoryInfo(PlayerBuildTempPath).GetFiles("*.*", SearchOption.AllDirectories);
        //foreach (var file in arr)
        //{
        //    if (file.Name.EndsWith(PathUtil.abSuffix) || file.Name.EndsWith(PathUtil.bytesSuffix))
        //        fileMap[file.Name] = file.FullName;
        //}
 
        //try
        //{
            //get mainifest
            var ab = AssetBundle.LoadFromFile(AssetsUtility.GetCachePlatformPath() + AssetsUtility.ASSETBUNDLE);
            Debuger.Log("fileMap ASSETBUNDLE: " + (AssetsUtility.GetCachePlatformPath() + AssetsUtility.ASSETBUNDLE));
     
            AssetBundleManifest abm = null;

            if (ab == null)
            {
                Debug.Log("Failed to load AssetBundle!");
            }
            else
            {
                abm = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            Debuger.Log("fileMap abm is: " + (abm != null));
        
            //
            int offsetConf = 0;
            var bytesConf = new byte[size / 5];
            int offsetBib = 0;
            var bytesBib = new byte[size];

            string bibName = getRandomName() + PathUtil.bytesSuffix;
            XBuffer.WriteString(bibName, bytesConf, ref offsetConf);//bib name
            string abcConfName = getRandomName() + PathUtil.bytesSuffix;
            XBuffer.WriteString(abcConfName, bytesConf, ref offsetConf);//abc configuration name
            Debuger.Log("fileMap bibName: " + bibName + "  ||  abcConfName: "+ abcConfName);

            //if (UnityEngine.Random.Range(0, 10) < 100)
            //{
            //    return false;
            //}
            //Hot list md5
            XBuffer.WriteInt(lastForceMap.Count, bytesConf, ref offsetConf);
            foreach (var kv in lastForceMap)
            {
                XBuffer.WriteString(kv.Key, bytesConf, ref offsetConf);
                XBuffer.WriteString(kv.Value, bytesConf, ref offsetConf);
            }

            //Write file header
            XBuffer.WriteInt(fileMap.Count, bytesConf, ref offsetConf);
            XBuffer.WriteLong(DateTime.Now.Ticks, bytesBib, ref offsetBib);

            //mess up the order
            var keyList = new List<string>(fileMap.Keys);
            var randomKeyList = mixStrArray(keyList.ToArray());
            //Debuger.Log("randomKeyList size " + randomKeyList.Count);
            //if(UnityEngine.Random.Range(0,10) < 100)
            //{
            //    return false;
            //}
            for (int i = 0; i < randomKeyList.Count; ++i)
            {
                var key = randomKeyList[i];
                var val = fileMap[key];
                byte[] data = File.ReadAllBytes(val);
                //byte[] md5Bytes = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(data);
                //string md5 = System.BitConverter.ToString(md5Bytes).Replace("-", "");

                // Cấu hình / cấu hình bib gỡ cài đặt một tệp riêng biệt, vì fileStream không thể được sử dụng để đọc tiêu đề tệp bib trong thư mục luồng                
                XBuffer.WriteString(key, bytesConf, ref offsetConf); //文件名
                //get md5
                string hashS = "";
                //if (key != AssetsUtility.ASSETBUNDLE)
                //{
                //    hashS = abm.GetAssetBundleHash(key).ToString();

                //}

                //else
                //{
                //    hashS = AssetsUtility.GetMd5Hash(key);
                //}
                hashS = AssetsUtility.GetMd5Hash(data);


                XBuffer.WriteString(hashS, bytesConf, ref offsetConf); //md5
                XBuffer.WriteInt(offsetBib, bytesConf, ref offsetConf); //Offset in bib

                //文件
                Array.Copy(data, 0, bytesBib, offsetBib, data.Length);
                offsetBib += data.Length;
            }

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                XBuffer.WriteString(getRandomFileContentStr(8, 1024 * 100), bytesBib, ref offsetBib);// Nhầm lẫn / 100k kích thước ngẫu nhiên
            else
                XBuffer.WriteString(getRandomFileContentStr(8, 1024 * 1024 * 3), bytesBib, ref offsetBib);// Confusion/3M random size

        //Write config

#if PGAME
            var path = Application.dataPath + "/Hotupdate Assets/Resources/" + PathUtil.BuildInFileListName;
#else
            var path = Application.dataPath + "/Resources/" + PathUtil.BuildInFileListName;
#endif
            if (File.Exists(path))
                File.Delete(path);
            var stream = new FileInfo(path).OpenWrite();
            stream.Write(bytesConf, 0, offsetConf);
            stream.Flush();
            stream.Close();
            stream.Dispose();

            //Write file
            if (Directory.Exists(Application.streamingAssetsPath))
                Directory.Delete(Application.streamingAssetsPath, true);
            Directory.CreateDirectory(Application.streamingAssetsPath);
            path = Application.streamingAssetsPath + "/" + bibName;
            stream = new FileInfo(path).OpenWrite();
            stream.Write(bytesBib, 0, offsetBib);
            stream.Flush();
            stream.Close();
            stream.Dispose();
            Debug.Log("bib generated successfully>" + path);
        //}
        //catch (Exception e)
        //{
        //    Debuger.Err(e.Message, e.StackTrace);
        //    return MakeForceAsBib(curLanguage, removeLanList, size * 2);
        //}
        return true;
    }
    [MenuItem("Tools/Player/MakeConfigGame",false,2)]
    public static void MakeConfigGameTools()
    {
        MakeConfigGame();
    }

    public static string GameConfigPath = Application.dataPath + "/AssetDll/ConfigGame/";
    public static void MakeConfigGame(int size = 1024 * 1024 * 1000)
    {
        if (!Directory.Exists(GameConfigPath))
            Directory.CreateDirectory(GameConfigPath);

        string configGamePath = Application.dataPath + "/ConfigGame/";
        var forceDir = new DirectoryInfo(configGamePath);

        var abList = new List<FileInfo>();
      
        //force
        abList.AddRange(forceDir.GetFiles("*" + PathUtil.bytesSuffix, SearchOption.AllDirectories)
            .Where(name => !name.FullName.EndsWith(".meta")));
        int forceResLen = abList.Count;

        ////Liệu tài nguyên trở lại có vào gói đầu tiên hay không
        //if (containsBack)
        //    abList.AddRange(backDir.GetFiles("*" + PathUtil.abSuffix, SearchOption.AllDirectories));

        var fileMap = new Dictionary<string, string>();
        for (int i = 0; i < abList.Count; ++i)
        {
            var file = abList[i];
            var tPath = file.FullName.Substring(forceDir.FullName.Length);


            string folderName = "", folderNamePath = "";
            tPath = tPath.Replace("\\", "/");
            if (tPath.Contains("/"))
            {
                folderName = tPath.Split('/')[0];
                folderNamePath = folderName + "/";
            }

            fileMap[tPath] = file.FullName;
        }

        try
        {
            
            int offsetConf = 0;
            var bytesConf = new byte[size / 5];
            int offsetBib = 0;
            var bytesBib = new byte[size];

        
            //string abcConfName = getRandomName() + PathUtil.bytesSuffix;
            //XBuffer.WriteString(abcConfName, bytesConf, ref offsetConf);//abc configuration name


            //Write file header
            XBuffer.WriteInt(fileMap.Count, bytesConf, ref offsetConf);
            //XBuffer.WriteLong(DateTime.Now.Ticks, bytesBib, ref offsetBib);

            //mess up the order
            var keyList = new List<string>(fileMap.Keys);
            var randomKeyList = mixStrArray(keyList.ToArray());

            for (int i = 0; i < randomKeyList.Count; ++i)
            {
                var key = randomKeyList[i];
                var val = fileMap[key];
                byte[] data = File.ReadAllBytes(val);

                // Cấu hình / cấu hình bib gỡ cài đặt một tệp riêng biệt, vì fileStream không thể được sử dụng để đọc tiêu đề tệp bib trong thư mục luồng                
                XBuffer.WriteString(key, bytesConf, ref offsetConf); //文件名
                //get md5
  

                XBuffer.WriteInt(data.Length, bytesConf, ref offsetConf); //md5
                XBuffer.WriteInt(offsetBib, bytesConf, ref offsetConf); //Offset in bib

                //文件
                Array.Copy(data, 0, bytesBib, offsetBib, data.Length);
                offsetBib += data.Length;

            }

 


            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                XBuffer.WriteString(getRandomFileContentStr(8, 1024 * 100), bytesBib, ref offsetBib);// Nhầm lẫn / 100k kích thước ngẫu nhiên
            else
                XBuffer.WriteString(getRandomFileContentStr(8, 1024 * 1024 * 3), bytesBib, ref offsetBib);// Confusion/3M random size

            //
            //write file bib
            string bibName = AssetsUtility.GetMd5Hash(bytesBib) + PathUtil.bytesSuffix;
            XBuffer.WriteString(bibName, bytesConf, ref offsetConf);//bib name

            //Write config
            var path = GameConfigPath + "buildin_config_list.bytes";
            if (File.Exists(path))
                File.Delete(path);
            var stream = new FileInfo(path).OpenWrite();
            stream.Write(bytesConf, 0, offsetConf);
            stream.Flush();
            stream.Close();
            stream.Dispose();

            //Write file
            //if (Directory.Exists(Application.streamingAssetsPath))
            //    Directory.Delete(Application.streamingAssetsPath, true);
            //Directory.CreateDirectory(Application.streamingAssetsPath);
            path = GameConfigPath + bibName;
            if (File.Exists(path))
                File.Delete(path);
            stream = new FileInfo(path).OpenWrite();
            stream.Write(bytesBib, 0, offsetBib);
            stream.Flush();
            stream.Close();
            stream.Dispose();
            Debug.Log("config game generated successfully>" + path);
        }
        catch (Exception e)
        {
            Debuger.Err(e.Message, e.StackTrace);
            MakeConfigGame(size * 2);
        }
    }
    public static void MakeAbc(int size = 1024 * 1024 * 100)
    {
        try
        {
            var path = Application.dataPath + "/Resources/" + PathUtil.BuildInFileListName;
            var listData = File.ReadAllBytes(path);
            int offset = 0;
            //bib名字
            string bibName = XBuffer.ReadString(listData, ref offset);
            //abcName
            string abcConfName = XBuffer.ReadString(listData, ref offset);
            var abcName = getRandomName() + PathUtil.bytesSuffix;

            int offsetConf = 0;
            var bytesConf = new byte[size / 3];
            //abcName
            XBuffer.WriteString(abcName, bytesConf, ref offsetConf);
            //dll
            if (File.Exists(EditorPath.GuoShenReleaseDllPath))
            {
                var dllData = File.ReadAllBytes(EditorPath.GuoShenReleaseDllPath);
                XBuffer.WriteInt(dllData.Length, bytesConf, ref offsetConf);
                Array.Copy(dllData, 0, bytesConf, offsetConf, dllData.Length);
                offsetConf += dllData.Length;
            }
            else
            {
                XBuffer.WriteInt(0, bytesConf, ref offsetConf);
            }

            //dep依赖
            var depFiles = Directory.GetFiles(EditorPath.GetGuoShenPath() + "/force", "*.d", SearchOption.AllDirectories);
            XBuffer.WriteInt(depFiles.Length, bytesConf, ref offsetConf);
            var mixArr = mixStrArray(depFiles);
            foreach (var d in mixArr)
            {
                string name = Path.GetFileNameWithoutExtension(d);
                var arr = ResDepManager.Singleton.GetDependence(name);
                if (arr == null)
                {
                    XBuffer.WriteString(name, bytesConf, ref offsetConf);
                    XBuffer.WriteShort(0, bytesConf, ref offsetConf);
                    continue;
                }
                XBuffer.WriteString(name, bytesConf, ref offsetConf);
                XBuffer.WriteShort((short)arr.Length, bytesConf, ref offsetConf);
                foreach (var s in arr)
                    XBuffer.WriteString(s, bytesConf, ref offsetConf);
            }

            //bean
            var beanFiles = Directory.GetFiles(EditorPath.GetGuoShenPath() + "/bean", "*" + PathUtil.bytesSuffix, SearchOption.AllDirectories);
            XBuffer.WriteInt(beanFiles.Length, bytesConf, ref offsetConf);
            var mixBeans = mixStrArray(beanFiles);
            foreach (var b in mixBeans)
            {
                string name = Path.GetFileNameWithoutExtension(b);
                XBuffer.WriteString(name, bytesConf, ref offsetConf);
                var beanData = File.ReadAllBytes(b);
                XBuffer.WriteInt(beanData.Length, bytesConf, ref offsetConf);
                Array.Copy(beanData, 0, bytesConf, offsetConf, beanData.Length);
                offsetConf += beanData.Length;
            }

            //资源
            var fileMap = new Dictionary<string, string>();
            var guoShenPath = EditorPath.GetGuoShenPath();
            //var uigsPath = new DirectoryInfo(guoShenPath + "/ui");
            var forcegsDir = new DirectoryInfo(guoShenPath + "/force");
            //foreach (var f in uigsPath.GetFiles("*" + PathUtil.abSuffix, SearchOption.TopDirectoryOnly))
            //    fileMap[f.Name] = f.FullName;
            foreach (var f in forcegsDir.GetFiles("*" + PathUtil.abSuffix, SearchOption.TopDirectoryOnly))
                fileMap[f.Name] = f.FullName;

            int fileOffset = 0;
            var bytesFile = new byte[size];

            XBuffer.WriteInt(fileMap.Count, bytesConf, ref offsetConf);
            var fList = mixStrArray(new List<string>(fileMap.Keys).ToArray());
            for (int i = 0; i < fList.Count; ++i)
            {
                var val = fileMap[fList[i]];
                XBuffer.WriteString(fList[i], bytesConf, ref offsetConf); //文件名
                XBuffer.WriteInt(fileOffset, bytesConf, ref offsetConf); //文件位置偏移量

                //文件
                byte[] data = File.ReadAllBytes(val);
                Array.Copy(data, 0, bytesFile, fileOffset, data.Length);
                fileOffset += data.Length;
            }
            //100k随机
            XBuffer.WriteString(getRandomFileContentStr(8, 1024 * 100), bytesConf, ref offsetConf);
            //混淆1M大小随机
            XBuffer.WriteString(getRandomFileContentStr(8, 1024 * 1024), bytesFile, ref fileOffset);

            //conf写磁盘
            var tmpPath = PlayerBuildTempPath + abcConfName;
            if (File.Exists(tmpPath))
                File.Delete(tmpPath);
            var stream = new FileInfo(tmpPath).OpenWrite();
            stream.Write(bytesConf, 0, offsetConf);
            stream.Flush();
            stream.Close();
            stream.Dispose();
            //打成ab
            AssetDatabase.Refresh();
            BuildAsBundle(tmpPath, Application.streamingAssetsPath, abcConfName);
            if (File.Exists(tmpPath))
                File.Delete(tmpPath);
            Debug.Log("abc config生成成功>" + abcConfName);

            //ab文件写磁盘
            var abcPath = Application.streamingAssetsPath + "/" + abcName;
            if (File.Exists(abcPath))
                File.Delete(abcPath);
            stream = new FileInfo(abcPath).OpenWrite();
            stream.Write(bytesFile, 0, fileOffset);
            stream.Flush();
            stream.Close();
            stream.Dispose();
            Debug.Log("abc生成成功>" + abcPath);
        }
        catch (Exception e)
        {
            Debuger.Err(e.Message, e.StackTrace);
            if (e is XBufferOutOfIndexException)
                MakeAbc(size * 2);
        }
    }

    public static void SDKDeal(ref string proOutPath, string curSdk, bool beforeBuild, BuildTarget target)
    {
#if UNITY_ANDROID
        curSdk= curSdk + "_android";
#elif UNITY_IPHONE
        curSdk = curSdk + "_ios";
#endif

        Debuger.Log("curSdk>", curSdk);
        //sdk相关配置

#if PGAME
        var jsonStr = File.ReadAllText(Application.dataPath + "/Hotupdate Assets/PluginAssets/Resource/Editor/build.json");
#else
        var jsonStr = File.ReadAllText(Application.dataPath + "/PluginAssets/Resource/Editor/build.json");
#endif
        var json = SimpleJSON.JSONClass.Parse(jsonStr);
        var sdkJson = json[curSdk];
        if (sdkJson == null)
        {
            Debuger.Err("build.json未配置当前渠道信息", curSdk);
            return;
        }
        if (sdkJson["same"] != null)
            sdkJson = json[sdkJson["same"].Value];
        if (sdkJson == null)
        {
            Debuger.Err("Thông tin kênh hiện tại không được định cấu hình trong build.json", curSdk);
            return;
        }

        if (beforeBuild)
        {
            if (target == BuildTarget.Android)
                ILRuntimeCLRBinding.GenerateCLRBindingByAnalysis();
            

            //密码
            if (sdkJson["keyStore"] != null)
            {
                PlayerSettings.Android.keystoreName = sdkJson["keyStore"].Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath);
                PlayerSettings.Android.keyaliasName = sdkJson["alias"].Value;
                PlayerSettings.keystorePass = sdkJson["storePass"].Value;
                PlayerSettings.keyaliasPass = sdkJson["aliasPass"].Value;
            }

            //签名
            if (sdkJson["name"] != null)
            {
                string fileName = Path.GetFileName(proOutPath);
                fileName = fileName.Replace(PlayerSettings.productName, sdkJson["name"].Value);
                proOutPath = Path.GetDirectoryName(proOutPath) + "/" + fileName;
                PlayerSettings.productName = sdkJson["name"].Value;
            }

            if (sdkJson["id"] != null)
            {
                PlayerSettings.applicationIdentifier = sdkJson["id"].Value;
                PlayerSettings.SetApplicationIdentifier(target == BuildTarget.Android ? BuildTargetGroup.Android : BuildTargetGroup.iOS, sdkJson["id"].Value);
            }
            if (sdkJson["version"] != null)
                PlayerSettings.bundleVersion = sdkJson["version"].Value;
            if (sdkJson["versionCode"] != null)
            {
                if (target == BuildTarget.Android)
                    PlayerSettings.Android.bundleVersionCode = sdkJson["versionCode"].AsInt;
                else
                    PlayerSettings.iOS.buildNumber = sdkJson["versionCode"].Value;
            }
            //ios 版本号
            if (sdkJson["iosVer"] != null)
                PlayerSettings.iOS.targetOSVersionString = sdkJson["iosVer"].Value;

            //过审时是否使用内购
            UnityEditor.Analytics.AnalyticsSettings.enabled = sdkJson["inappPurchase"].AsBool;
            UnityEditor.Purchasing.PurchasingSettings.enabled = sdkJson["inappPurchase"].AsBool;
            UnityEditor.Advertisements.AdvertisementSettings.enabled = sdkJson["inappPurchase"].AsBool;

            //删除
            var arr = sdkJson["delete"] as SimpleJSON.JSONArray;
            if (arr != null)
            {
                foreach (var node in arr.Childs)
                {
                    var path = node.Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath);
                    if (File.Exists(path))
                        File.Delete(path);
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                        Directory.CreateDirectory(path);
                    }
                }
            }

            //复制
            arr = sdkJson["copy"] as SimpleJSON.JSONArray;
            if (arr != null)
            {
                foreach (var node in arr.Childs)
                {
                    var path1 = node["from"].Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath);
                    var path2 = node["to"].Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath);

                    if (File.Exists(path1))
                        File.Copy(path1, path2, true);

                    if (Directory.Exists(path1))
                    {
                        if (!Directory.Exists(path2))
                            Directory.CreateDirectory(path2);
                        copyDir(path1, path2);
                    }
                }
            }

            //闪屏
            AssetDatabase.Refresh();
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/splash.png");
            if (sdkJson["splash"] == null || !(sdkJson["splash"].AsBool))
                tex = null;
            PlayerSettings.Android.splashScreenScale = AndroidSplashScreenScale.ScaleToFit;
            SerializedObject projectSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            SerializedProperty it = projectSettings.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "androidSplashScreen" || it.name == "iPhoneSplashScreen")
                    it.objectReferenceValue = tex;
            }
            projectSettings.ApplyModifiedProperties();
        }
        else
        {
#if UNITY_IOS
            var infoPath = proOutPath + "/Data/Raw/build_info.txt";
            if (File.Exists(infoPath))
                File.WriteAllText(infoPath, string.Format("Build from {0} at {1}", getRandomName(false, 1, 1), DateTime.Now));
#endif

            UnityEditor.Analytics.AnalyticsSettings.enabled = false;
            UnityEditor.Purchasing.PurchasingSettings.enabled = false;
            AssetDatabase.Refresh();

            //删除
            var arr = sdkJson["endDelete"] as SimpleJSON.JSONArray;
            if (arr != null)
            {
                foreach (var node in arr.Childs)
                {
                    var path = node.Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath);
                    if (target == BuildTarget.Android && EditorUserBuildSettings.exportAsGoogleAndroidProject)
                        path = node.Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath + "/" + Application.productName);
                    if (File.Exists(path))
                        File.Delete(path);
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                        Directory.CreateDirectory(path);
                    }
                }
            }

            //复制
            arr = sdkJson["endCopy"] as SimpleJSON.JSONArray;
            if (arr != null)
            {
                foreach (var node in arr.Childs)
                {
                    var path1 = node["from"].Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath);
                    var path2 = node["to"].Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath);
                    if (target == BuildTarget.Android && EditorUserBuildSettings.exportAsGoogleAndroidProject)
                    {
                        //如果是android更新so的方式
                        path1 = node["from"].Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath + "/" + Application.productName);
                        path2 = node["to"].Value.Replace("${1}", Application.dataPath).Replace("${2}", proOutPath + "/" + Application.productName);
                    }

                    if (File.Exists(path1))
                        File.Copy(path1, path2, true);

                    if (Directory.Exists(path1))
                    {
                        if (!Directory.Exists(path2))
                            Directory.CreateDirectory(path2);
                        copyDir(path1, path2);
                    }
                }
            }

            //xcode工程里面的图片全部混淆
            if (target == BuildTarget.iOS && Directory.Exists(proOutPath))
            {
                var dirArr = Directory.GetDirectories(proOutPath);
                foreach (var dir in dirArr)
                {
                    if (!dir.ToLower().EndsWith(".bundle") && dir != "Data")
                        continue;

                    foreach (var file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                    {
                        var lowerName = file.ToLower();
                        if (lowerName.EndsWith(".png")
                            || lowerName.EndsWith(".jpg")
                            || lowerName.EndsWith(".jpeg")
                            || lowerName.EndsWith("unity default resources"))
                        {
                            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
                            fs.Position = fs.Length;
                            StreamWriter sw = new StreamWriter(fs);
                            sw.Write(getRandomFileContentStr(9, 20));
                            sw.Flush();
                            sw.Close();
                            fs.Close();
                        }
                    }
                }
            }
        }
    }

    public static void MakeBackAsObb(string backBasePath, string curLanguage, List<string> removeLanList, string outPath, long size = 1024 * 1024 * 500)
    {
        try
        {
            //var backBasePath = Application.dataPath + "/../ABout/Android/_backBaseRes.json";
            var json = new SimpleJSON.JSONNode();
            if (File.Exists(backBasePath))
            {
                var jsonStr = File.ReadAllText(backBasePath);
                json = SimpleJSON.JSONClass.Parse(jsonStr);
                if (json == null || json.Count <= 0)
                {
                    EditorUtility.DisplayDialog("make obb", "back基本版本错误>" + backBasePath, "ok");
                    return;
                }
            }
            else
            {
                Debug.LogWarning("生成obb时没有找到back基础版本，obb中所有资源版本号将设置为1");
            }

            //当前语言文件夹
            List<string> curLanFileList = new List<string>();
            var mulFiles = Directory.GetFiles(PathUtil.GetBackABPath() + curLanguage);
            foreach (var f in mulFiles)
                curLanFileList.Add(Path.GetFileName(f));

            var abPath = PathUtil.GetBackABPath();
            var files = Directory.GetFiles(abPath, "*" + PathUtil.abSuffix, SearchOption.AllDirectories);
            var fileMap = new Dictionary<string, string>();
            //过滤文件
            for (int i = 0; i < files.Length; ++i)
            {
                var file = files[i];
                var name = file.Substring(abPath.Length);
                name = name.Replace("\\", "/");

                if (name.Contains("/"))
                {
                    //当前语言去掉文件夹
                    var folder = Path.GetDirectoryName(name);
                    //排除不要的语言资源
                    if (removeLanList.Contains(folder))
                        continue;
                    if (folder == curLanguage)
                        name = Path.GetFileName(name);
                }
                else
                {
                    //根目录下和当前语言文件夹下相同的不要
                    if (curLanFileList.Contains(name))
                        continue;
                }

                fileMap[name] = file;
            }

            int offset = 0;
            var bytes = new byte[size];

            string pkgName = PlayerSettings.applicationIdentifier;
            XBuffer.WriteInt(0, bytes, ref offset);
            XBuffer.WriteString("ABCEDFGHIJKLMNOPQRSTUVWXYZ123456", bytes, ref offset); //MD5码占位(32个字符)
            XBuffer.WriteString(pkgName, bytes, ref offset);
            XBuffer.WriteLong(DateTime.Now.Ticks, bytes, ref offset);
            XBuffer.WriteInt(fileMap.Count, bytes, ref offset);

            //写文件头
            var offsetList = new List<int>();
            var fList = mixStrArray(new List<string>(fileMap.Keys).ToArray());
            for (int i = 0; i < fList.Count; ++i)
            {
                var name = fList[i];
                var path = fileMap[name];

                int version = 1;
                if (json[name] != null && json[name]["ver"] != null)
                {
                    version = json[name]["ver"].AsInt;
                    byte[] data = File.ReadAllBytes(path);
                    byte[] md5Bytes = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(data);
                    string md5 = System.BitConverter.ToString(md5Bytes).Replace("-", "");
                    if (json[name]["md5"].Value != md5)
                        version++;
                }

                XBuffer.WriteString(name, bytes, ref offset); //名字
                XBuffer.WriteInt(version, bytes, ref offset); //版本号
                offsetList.Add(offset);
                XBuffer.WriteInt(0, bytes, ref offset);       //偏移量
            }
            //偏移量结尾，用于计算最后一个文件的长度
            offsetList.Add(offset);
            XBuffer.WriteInt(0, bytes, ref offset);
            int headDataLength = offset;

            //写文件
            for (int i = 0; i < fList.Count; ++i)
            {
                var path = fileMap[fList[i]];
                var data = File.ReadAllBytes(path);

                var off = offsetList[i];
                XBuffer.WriteInt(offset, bytes, ref off);//偏移量
                Array.Copy(data, 0, bytes, offset, data.Length);
                offset += data.Length;
            }
            //偏移量结尾，用于计算最后一个文件的长度
            var offEnd = offsetList[fList.Count];
            XBuffer.WriteInt(offset, bytes, ref offEnd);

            //计算文件头md5码
            var md5Data = new byte[headDataLength];
            Array.Copy(bytes, 0, md5Data, 0, headDataLength);
            byte[] configMd5Bytes = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(md5Data);
            string configMd5 = System.BitConverter.ToString(configMd5Bytes).Replace("-", "");

            //头文件长度
            int zero = 0;
            XBuffer.WriteInt(headDataLength, bytes, ref zero);
            //文件头的md5码作为校验码
            XBuffer.WriteString(configMd5, bytes, ref zero);

            //随机100k填充
            XBuffer.WriteString(getRandomFileContentStr(8, 1024 * 100), bytes, ref offset);

            var outDicPath = Path.GetDirectoryName(outPath);
            var obbPath = outDicPath + "/main." + PlayerSettings.Android.bundleVersionCode + "." + pkgName + ".obb";
            if (File.Exists(obbPath))
                File.Delete(obbPath);

            var stream = new FileInfo(obbPath).OpenWrite();
            stream.Write(bytes, 0, offset);
            stream.Flush();
            stream.Close();
            stream.Dispose();
            Debug.Log("obb 生成成功>" + obbPath);
        }
        catch (Exception e)
        {
            Debuger.Err(e.Message, e.StackTrace);
            if (e is XBufferOutOfIndexException)
                MakeBackAsObb(backBasePath, curLanguage, removeLanList, outPath, size * 2);
        }
    }

    private static void copyDir(string fromPath, string toPath)
    {
        DirectoryInfo dir = new DirectoryInfo(fromPath);
        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
        foreach (FileSystemInfo i in fileinfo)
        {
            if (i is DirectoryInfo)
            {
                if (!Directory.Exists(toPath + "/" + i.Name))
                    Directory.CreateDirectory(toPath + "/" + i.Name);
                copyDir(i.FullName, toPath + "/" + i.Name);
            }
            else
            {
                File.Copy(i.FullName, toPath + "/" + i.Name, true);
            }
        }
    }

    /// <summary>
    /// Merge dependency
    /// </summary>
    public static void MergeABDependence(string[] inputPathArr, string outPath, string depName, int size = 2048 * 1024)
    {
        EditorUtility.DisplayProgressBar("Gói", "Hợp nhất phụ thuộc tài nguyên", 0);
        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        string tempPath = Application.dataPath + "/" + Path.GetFileNameWithoutExtension(depName) + ".bytes";
        if (File.Exists(tempPath))
            File.Delete(tempPath);

        List<string> files = new List<string>();
        foreach (var path in inputPathArr)
        {
            var fs = Directory.GetFiles(path, "*.d", SearchOption.AllDirectories);
            files.AddRange(fs);
        }

        if (files.Count == 0)
            return;

        try
        {
            int offset = 0;
            var bytes = new byte[size];
            XBuffer.WriteInt(files.Count, bytes, ref offset);
            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                var arr = ResDepManager.Singleton.GetDependence(name, false);
                if (arr == null)
                {
                    XBuffer.WriteString(name, bytes, ref offset);
                    XBuffer.WriteShort(0, bytes, ref offset);
                    continue;
                }
                XBuffer.WriteString(name, bytes, ref offset);
                XBuffer.WriteShort((short)arr.Length, bytes, ref offset);
                foreach (var s in arr)
                    XBuffer.WriteString(s, bytes, ref offset);
            }
            Debug.Log("Dependent file list size>" + offset / 1024 + "kb");
            var stream = new FileInfo(tempPath).OpenWrite();
            stream.Write(bytes, 0, offset);
            stream.Flush();
            stream.Close();
            stream.Dispose();
        }
        catch (Exception e)
        {
            if (e is XBufferOutOfIndexException)
                MergeABDependence(inputPathArr, outPath, depName, size * 2);
            return;
        }
        AssetDatabase.Refresh();

        AssetBundleBuild abb = new AssetBundleBuild();
        abb.assetBundleName = depName;
        abb.assetNames = new string[] { tempPath.Substring(tempPath.IndexOf("Assets/")) };
        BuildPipeline.BuildAssetBundles(outPath, new AssetBundleBuild[] { abb }, ABBuilder.buildOptions, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();

        File.Delete(tempPath);
        DirectoryInfo info = new DirectoryInfo(outPath);
        File.Delete(outPath + "/" + info.Name);
        File.Delete(outPath + "/" + info.Name + ".manifest");
        File.Delete(outPath + "/" + depName + ".manifest");
        EditorUtility.DisplayProgressBar("Package", "Merge Resources", 1);
    }

    public static bool MakeBuildInFileList(string lastForceJsonPath, int size = 1024 * 1024)
    {
        string destFile = Application.dataPath + "/Resources/" + PathUtil.BuildInFileListName;
        if (File.Exists(destFile))
            File.Delete(destFile);

        Dictionary<string, string> lastForceMap = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(lastForceJsonPath))
        {
            //增量更新需要把上次更新的文件写到包内文件列表，配置表和UI不是ab，需特殊处理
            var jsonStr = File.ReadAllText(lastForceJsonPath);
            var json = SimpleJSON.JSONClass.Parse(jsonStr);
            if (json == null)
                return false;
            var jsonArr = json["files"] as SimpleJSON.JSONArray;
            if (jsonArr == null)
                return false;
            for (int i = 0; i < jsonArr.Count; ++i)
                lastForceMap[jsonArr[i]["res"].Value] = jsonArr[i]["md5"].Value;
        }

        try
        {
            //StreamingAssets目录资源混淆
            int offset = 0;
            var bytes = new byte[size];
            var abPath = PathUtil.GetABBuildinPath();
            var files = Directory.GetFiles(PathUtil.GetABBuildinPath(), "*" + PathUtil.abSuffix, SearchOption.AllDirectories);
            XBuffer.WriteInt(files.Length + lastForceMap.Count, bytes, ref offset);
            foreach (var kv in lastForceMap)
            {
                XBuffer.WriteString(kv.Key, bytes, ref offset);
                XBuffer.WriteString(kv.Value, bytes, ref offset);
            }
            foreach (var file in files)
            {
                // Only record resource ab, dll, dependency and other information need not be recorded
                if (!file.EndsWith(PathUtil.abSuffix))
                    continue;
                var name = file.Substring(abPath.Length);
                byte[] data = File.ReadAllBytes(file);
                byte[] md5Bytes = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(data);
                string md5 = System.BitConverter.ToString(md5Bytes).Replace("-", "");
                name = name.Replace("\\", "/");

                //还原名字
                var newName = BuildInNameMap.GetRealName(name.Replace(PathUtil.abSuffix, ""));
                if (name.EndsWith(PathUtil.abSuffix))
                    newName = newName + PathUtil.abSuffix;
                name = newName;

                XBuffer.WriteString(name, bytes, ref offset);
                XBuffer.WriteString(md5, bytes, ref offset);

                if (ResourceConfuse)
                {
                    //Add resource obfuscation suffix
                    FileStream fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
                    fs.Position = fs.Length;
                    StreamWriter sw = new StreamWriter(fs);
                    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                        sw.Write(getRandomFileContentStr(6, 13));
                    else
                        sw.Write(getRandomFileContentStr(13, 1000));
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
            Debug.Log("First package file list size>" + offset / 1024 + "kb");
            var stream = new FileInfo(destFile).OpenWrite();
            stream.Write(bytes, 0, offset);
            stream.Flush();
            stream.Close();
            stream.Dispose();
        }
        catch (Exception e)
        {
            if (e is XBufferOutOfIndexException)
                return MakeBuildInFileList(lastForceJsonPath, size * 2);
        }
        AddRandomTextureToResource();
        AssetDatabase.Refresh();
        return true;
    }

    public static void AddRandomTextureToResource()
    {
        //resource下面的目录添加随机资源
        if (Directory.Exists(Application.dataPath + "/Resources/_hunxiaotu"))
            Directory.Delete(Application.dataPath + "/Resources/_hunxiaotu", true);
        Directory.CreateDirectory(Application.dataPath + "/Resources/_hunxiaotu");

        int count = UnityEngine.Random.Range(1, 10);
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            count = UnityEngine.Random.Range(5, 15);
        int[] sizeArr = new int[] { 2, 4, 8, 16, 32, 64 };
        for (int i = 0; i < count; ++i)
        {
            int wid = 0, hei = 0;
            TextureFormat format = TextureFormat.RGB24;
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                //2的幂
                wid = sizeArr[UnityEngine.Random.Range(0, sizeArr.Length)];
                hei = sizeArr[UnityEngine.Random.Range(0, sizeArr.Length)];
            }
            else
            {
                //正方形
                wid = sizeArr[UnityEngine.Random.Range(0, sizeArr.Length)];
                hei = wid;
            }
            Texture2D tex = new Texture2D(wid, hei, format, false);
            for (int a = 0; a < tex.width; ++a)
            {
                for (int b = 0; b < tex.height; ++b)
                {
                    Color color = UnityEngine.Random.ColorHSV();
                    color.a = 1;
                    tex.SetPixel(a, b, color);
                }
            }
            var data = tex.EncodeToJPG();
            string path = "Resources/_hunxiaotu/" + tex.GetHashCode() + ".jpeg";
            File.WriteAllBytes(Application.dataPath + "/" + path, data);
            AssetDatabase.Refresh();
            TextureImporter importer = AssetImporter.GetAtPath("Assets/" + path) as TextureImporter;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }

    //dll
    public static void BuildAsBundle(string path, string outPath, string outName, bool encode = false)
    {
        DirectoryInfo outDir = new DirectoryInfo(outPath);
        if (!outDir.Exists)
            outDir.Create();

        string destDir = Application.dataPath + "/buildTmp/";
        string dest = destDir + outName;
        Directory.CreateDirectory(destDir);
        File.Copy(path, dest, true);
        if (encode)
        {
            var bytes = File.ReadAllBytes(dest);
            File.Delete(dest);
            PathUtil.Encode(bytes);
            File.WriteAllBytes(dest, bytes);
        }
        AssetDatabase.Refresh();

        //打成一个bundle
        AssetBundleBuild abd = new AssetBundleBuild();
        abd.assetBundleName = outName;
        abd.assetNames = new string[] { dest.Substring(dest.LastIndexOf("Assets/buildTmp/")) };
        BuildPipeline.BuildAssetBundles(outPath, new AssetBundleBuild[] { abd }, ABBuilder.buildOptions, EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.Refresh();
        //删除临时目录
        Directory.Delete(Application.dataPath + "/buildTmp/", true);
        //删除多余的bundle文件
        foreach (var file in Directory.GetFiles(outPath))
        {
            if (!file.EndsWith(PathUtil.bytesSuffix)
                && !file.EndsWith(PathUtil.abSuffix)
                && !file.EndsWith(PathUtil.UpdateDepSuffix)
                && !file.EndsWith(PathUtil.UpdateBeanSuffix)
                && !file.EndsWith(PathUtil.UpdateLuaSuffix))
                File.Delete(file);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }
    public static void BuildDirToAssetAndAssignBundleName(string inPath, string outPath, string buildExtension, string outName = "", bool encode = false)
    {
        DirectoryInfo outDir = new DirectoryInfo(outPath);
        if (!outDir.Exists)
            outDir.Create();

        string[] files = Directory.GetFiles(inPath, buildExtension, SearchOption.AllDirectories);
        if (files.Length == 0)
            return;

        List<string> buildList = new List<string>();
        //string destDir = Application.dataPath + "/buildTmp/";
        //Directory.CreateDirectory(destDir);
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(files[i]);
            string dest = outPath + fileName + PathUtil.bytesSuffix;
            File.Copy(files[i], dest, true);
            if (encode)
            {
                var bytes = File.ReadAllBytes(dest);
                PathUtil.Encode(bytes);
                File.WriteAllBytes(dest, bytes);
            }
            buildList.Add(dest.Substring(dest.LastIndexOf("Assets/buildTmp/")));
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }
    //拷贝文件
    public static void BuildDirToAB(string inPath, string outPath, string buildExtension, string outName = "", bool oneBuild = false, bool encode = false)
    {
        DirectoryInfo outDir = new DirectoryInfo(outPath);
        if (!outDir.Exists)
            outDir.Create();

        string[] files = Directory.GetFiles(inPath, buildExtension, SearchOption.AllDirectories);
        if (files.Length == 0)
            return;

        List<string> buildList = new List<string>();
        string destDir = Application.dataPath + "/buildTmp/";
        Directory.CreateDirectory(destDir);
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(files[i]);
            string dest = destDir + fileName + PathUtil.bytesSuffix;
            File.Copy(files[i], dest, true);
            if (encode)
            {
                var bytes = File.ReadAllBytes(dest);
                PathUtil.Encode(bytes);
                File.WriteAllBytes(dest, bytes);
            }
            buildList.Add(dest.Substring(dest.LastIndexOf("Assets/buildTmp/")));
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        // Kết hợp thành một gói
        if (oneBuild)
        {
            AssetBundleBuild abd = new AssetBundleBuild();
            abd.assetBundleName = outName;
            abd.assetNames = buildList.ToArray();
            BuildPipeline.BuildAssetBundles(outPath, new AssetBundleBuild[] { abd }, ABBuilder.buildOptions, EditorUserBuildSettings.activeBuildTarget);
        }
        else
        {
            var list = new List<AssetBundleBuild>();
            foreach (var b in buildList)
            {
                AssetBundleBuild abd = new AssetBundleBuild();
                abd.assetBundleName = Path.GetFileNameWithoutExtension(b).ToLower() + Path.GetExtension(outName);
                abd.assetNames = new string[] { b };
                list.Add(abd);
            }
            BuildPipeline.BuildAssetBundles(outPath, list.ToArray(), ABBuilder.buildOptions, EditorUserBuildSettings.activeBuildTarget);
        }

        AssetDatabase.Refresh();
        //删除临时目录
        Directory.Delete(Application.dataPath + "/buildTmp/", true);
        //删除多余的bundle文件
        foreach (var file in Directory.GetFiles(outPath))
        {
            if (!file.EndsWith(PathUtil.bytesSuffix)
                && !file.EndsWith(PathUtil.abSuffix)
                && !file.EndsWith(PathUtil.UpdateDepSuffix)
                && !file.EndsWith(PathUtil.UpdateBeanSuffix)
                && !file.EndsWith(PathUtil.UpdateLuaSuffix))
                File.Delete(file);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static List<string> mixStrArray(string[] arr)
    {
        if (arr == null || arr.Length == 0)
            return new List<string>();

        var list = new List<string>();
        var oldList = new List<string>(arr);
        int num = oldList.Count;
        for (int i = 0; i < num; ++i)
        {
            var idx = UnityEngine.Random.Range(0, oldList.Count);
            list.Add(oldList[idx]);
            oldList.RemoveAt(idx);
        }
        return list;
    }


    public static void CopyCacheAssetToStreamingAsset()
    {
        EditorUtility.DisplayProgressBar("Copying...", "Đang copy tất cả AB vào folder StreamingAsset", 0.2f);

        //Copy from persistentDataPath Pgame -> StreamingAssets PgameMain
        string fromPath = Application.dataPath + "/../../SaveBundle/Caches";

        string toPath = Application.dataPath + "/../hotupdate/PGameMain/Assets/StreamingAssets";
        copyDirAB(fromPath, toPath);
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Completed", "Hoàn thành AB vào folder StreamingAsset", "OK");
    }
    private static void copyDirAB(string fromPath, string toPath)
    {
        DirectoryInfo dir = new DirectoryInfo(fromPath);
        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
        foreach (FileSystemInfo i in fileinfo)
        {
            //Debug.Log("i.FullName " + i.FullName  +"   || bool: "+(i.FullName.EndsWith(".manifest") == false));
            //if (i.FullName.EndsWith(".manifest") == false)
            //{
            if (i is DirectoryInfo)
            {
                if (!Directory.Exists(toPath + "/" + i.Name))
                    Directory.CreateDirectory(toPath + "/" + i.Name);
                copyDir(i.FullName, toPath + "/" + i.Name);

                var finalFiles = Directory.GetFiles(toPath + "/" + i.Name, "*.*", SearchOption.AllDirectories);

                foreach (var file in finalFiles)
                {
                    if (file.EndsWith(".manifest"))
                        File.Delete(file);
                }

            }
            else
            {
                File.Copy(i.FullName, toPath + "/" + i.Name, true);
            }
            //}    

        }
    }
}