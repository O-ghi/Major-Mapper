using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class BuildUtil
{
    public static int byteToMB = 2 << 20;
    public static readonly BuildTarget targetPlatform = EditorUserBuildSettings.activeBuildTarget;
    public static readonly string projectDir = Directory.GetParent(Application.dataPath).FullName.Replace("\\", "/") + "/";
    public static readonly string resourcesDir = Application.dataPath + "/Resources/";
    public static readonly string installTempDir = projectDir + "/installTempDir/";
    static public readonly string tempUpkDir = projectDir + "/tempUpk/";

    public static readonly string moduleAtlas = "GUI/ModuleAtlas/";//模块纹理集
    static public readonly string[] installScenes = {"Assets/Scenes/Loading.unity" };
    static public readonly string[] excludeExt = { ".svn", ".meta", ".bat", ".proj", ".luaprj" };
    static public readonly MD5 md5 = MD5.Create();

    public static long time { get { return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond; } }

    public static bool LegalFile(string path)
    {
        if (ArrayUtility.Contains<string>(excludeExt, Path.GetExtension(path))) return false;
        if ((File.GetAttributes(path) & FileAttributes.Hidden) == FileAttributes.Hidden) return false;
        if (Path.GetFileNameWithoutExtension(path) == "") return false;
        return true;
    }
}
