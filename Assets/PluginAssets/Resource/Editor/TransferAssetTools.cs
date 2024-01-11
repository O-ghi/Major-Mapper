using UnityEngine;
using UnityEditor;
using System.IO;

public class TransferAssetTools : ScriptableObject
{

    private const string BinaryAssetPath = "Assets/Bin/";

    [MenuItem("Tools/TransAssetToPGame-PGameLogic")]
    public static void TransferAsset()
    {
        string ScriptPGameLogicPath = Path.GetFullPath(Application.dataPath + "/../GameCore/code/trunk/GameLogic/Script/");
        string[] lstScript = Directory.GetFiles(ScriptPGameLogicPath, "*.cs", SearchOption.AllDirectories);
        foreach (string file in lstScript) File
                .Delete(file);
        //
        string LogicScriptPath = Application.dataPath + "/ScriptsLogic/";

        string[] pGameScript = Directory.GetFiles(LogicScriptPath, "*.cs", SearchOption.AllDirectories);
        foreach (string file in pGameScript)
        {
            string newfile = file;
            newfile = newfile.Replace(LogicScriptPath, ScriptPGameLogicPath);
            if (!Directory.Exists(Path.GetDirectoryName(newfile))) Directory.CreateDirectory(Path.GetDirectoryName(newfile));
            File.Copy(file, newfile);

            // File.SetAttributes(newfile, FileAttributes.Normal);
        }

        var dllBat = new System.Diagnostics.Process();
        dllBat.StartInfo.WorkingDirectory = ScriptPGameLogicPath + "..\\";

        dllBat.StartInfo.FileName = ScriptPGameLogicPath + "..\\" + "addScripts.bat";
        dllBat.StartInfo.Arguments = "close";
        dllBat.Start();
        dllBat.WaitForExit();

        //todo: Hien tai path duong dan dang if UNITY_EDITOR, nen se uu tien dong bo mode debug, khi build se dong bo mode release
#if UNITY_ANDROID
        dllBat.StartInfo.FileName = ScriptPGameLogicPath + "..\\" + "build_android_release.bat";
#endif
#if UNITY_IPHONE
        dllBat.StartInfo.FileName = ScriptPGameLogicPath + "..\\" + "build_ios_release.bat";
#endif
        dllBat.StartInfo.Arguments = "close";
        dllBat.Start();
        dllBat.WaitForExit();

        //
        Debug.Log("Build DLL Done");

        //EditorUtility.DisplayDialog("Complete", "Trans Ok", "OK", "");
    }
}