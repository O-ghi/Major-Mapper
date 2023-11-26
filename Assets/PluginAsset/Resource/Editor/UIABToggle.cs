
using UnityEditor;
using UnityEngine;

public class UIABToggle
{
    //private const string MenuName = "Tools/Game/UI uses AB";
    //private const string useABUI = "use_ui_ab";

    //public static bool IsEnabled
    //{
    //    get { return EditorPrefs.GetBool(useABUI, true); }
    //    set { EditorPrefs.SetBool(useABUI, value); }
    //}

    //[MenuItem(MenuName)]
    //private static void ToggleAction()
    //{
    //    IsEnabled = !IsEnabled;
    //}

    //[MenuItem(MenuName, true)]
    //private static bool ToggleActionValidate()
    //{
    //    Menu.SetChecked(MenuName, IsEnabled);
    //    return true;
    //}


    private static bool battleLog = true;
    [MenuItem("Tools/Game/Battle Log", false, 1)]
    public static void bLog()
    {
        battleLog = EditorPrefs.GetBool("battle_log", true);
        battleLog = true;
        EditorPrefs.SetBool("battle_log", battleLog);
        Menu.SetChecked("Tools/Game/Battle Log", battleLog);
    }

    [MenuItem("Tools/Game/Battle Log", true, 1)]
    public static bool bLogOption()
    {
        battleLog = EditorPrefs.GetBool("battle_log", true);
        battleLog = false;
        EditorPrefs.SetBool("battle_log", battleLog);
        Menu.SetChecked("Tools/Game/Battle Log", battleLog);
        return true;
    }

    [MenuItem("Tools/Game/Open Cache")]
    public static void OpenCache()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}