#if UNITY_EDITOR
using UnityEngine;

public class EditorPath
{
    public static string[] AllFix
    {
        get
        {
            var path = Application.dataPath + "/../../code_fix";
            var branchs = System.IO.Directory.GetDirectories(path);
            for (int i = 0; i < branchs.Length; ++i)
                branchs[i] = System.IO.Path.GetFileName(branchs[i]);
            return branchs;
        }
    }

    public static string CurrentFix
    {
        get
        {
            return UnityEditor.EditorPrefs.GetString("fixBranchName", "trunk");
        }
        set
        {
            UnityEditor.EditorPrefs.SetString("fixBranchName", value);
        }
    }

    public static string[] AllBranch
    {
        get
        {
#if !Main
            var path = Application.dataPath + "/../../code";
#else
            var path = Application.dataPath + "/../hotupdate/code";
#endif
            var branchs = System.IO.Directory.GetDirectories(path);
            for (int i = 0; i < branchs.Length; ++i)
                branchs[i] = System.IO.Path.GetFileName(branchs[i]);
            return branchs;
        }
    }

    public static string CurrentBranch
    {
        get
        {
            return UnityEditor.EditorPrefs.GetString("branchName", "trunk");
        }
        set
        {
            UnityEditor.EditorPrefs.SetString("branchName", value);
        }
    }

    public static string CurrentGuoShen
    {
        get
        {
            return UnityEditor.EditorPrefs.GetString("GuoShenName", "No");
        }
        set
        {
            UnityEditor.EditorPrefs.SetString("GuoShenName", value);
        }
    }

    public static bool HasGuoShen
    {
        get
        {
            //Duong: chi su dung 1 mode duy nhat cho moi asset
            return false;
            //return CurrentGuoShen != "No";
        }
    }

    public static string[] AllGuoShen
    {
        get
        {
            var path = "";
#if UNITY_IOS
            path = Application.dataPath + "/../../code/"+ CurrentBranch + "/GameAB/ios/guoshen";
#elif UNITY_ANDROID
            path = Application.dataPath + "/../../code/" + CurrentBranch + "/GameAB/android/guoshen";
#endif
            if (!System.IO.Directory.Exists(path))
                return new string[0];
            var arr = System.IO.Directory.GetDirectories(path);
            var newArr = new string[arr.Length + 1];
            newArr[0] = "No";
            for (int i = 0; i < arr.Length; ++i)
                newArr[i + 1] = System.IO.Path.GetFileName(arr[i]);
            return newArr;
        }
    }

    public static string GetGuoShenPath(string resName = null)
    {
        if (CurrentGuoShen == "No")
            return resName;

        var path = "";
#if UNITY_IOS
        path = Application.dataPath + "/../../code/" + CurrentBranch + "/GameAB/ios/guoshen/" + CurrentGuoShen;
#elif UNITY_ANDROID
        path = Application.dataPath + "/../../code/" + CurrentBranch + "/GameAB/android/guoshen/" + CurrentGuoShen;
#endif
        if (string.IsNullOrEmpty(resName))
            return path;

        if (!resName.EndsWith(PathUtil.abSuffix))
            resName += PathUtil.abSuffix;

        if (System.IO.File.Exists(path + "/force/" + resName))
            return path + "/force/" + resName;
        return path + "/ui/" + resName;
    }

    public static string GuoShenDebugDllPath
    {
        get
        {
            if (CurrentGuoShen == "No")
                return "";
            return GetGuoShenPath() + "/debug/GameLogic.dll";
        }
    }

    public static string GuoShenReleaseDllPath
    {
        get
        {
            if (CurrentGuoShen == "No")
                return "";
            return GetGuoShenPath() + "/release/GameLogic.dll";
        }
    }

    public static string GuoShenPdbPath
    {
        get
        {
            if (CurrentGuoShen == "No")
                return "";
            return GetGuoShenPath() + "/debug/GameLogic.pdb";
        }
    }
}
#endif