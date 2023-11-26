using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClearPrefs
{
    [MenuItem("Tools/Game/Clear local data", false, 1)]
    static void clearPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("Tools/Game/Close novice guide", false, 1)]
    static void CloseGuide()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("Only effective when running");
            return;
        }
        bool useILRT = UnityEditor.EditorPrefs.GetBool("ILRuntime_Editor_Enable", true);
        if (useILRT)
        {
            GameLauncherCore.ilrtApp.Invoke("GuideService", "closeGuide", null, null);
        }
        else
        {
            var type = GameLauncherCore.dllAssembly.GetType("GuideService");
            var met = type.GetMethod("closeGuide", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            met.Invoke(null, null);
        }
    }
}