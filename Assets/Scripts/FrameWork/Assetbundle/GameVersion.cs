using UnityEngine;
using System.Collections;

public class GameVersion
{
    private static string _exe;
    public static string EXE { 
        get {
#if UNITY_EDITOR
            return (PlayerPrefs.GetInt("Editor PlatformData", 1)).ToString();
#else
            return _exe;
#endif

        }
        set {
            _exe = value;
        }

    }
    public static string BUILD = "001";
    public static string RES = "0";
}


public class GameEnviroment
{
    public enum EEnviroment
    {
        eDev = 0,
        eUAT,
        eRelease,
    };

    public static EEnviroment NETWORK_ENV = EEnviroment.eDev;        // NEED to change for Distribution
    public static bool NeedAndroidObb = false;
    //
    public static string HotWalletUrl = ": https://test-market.galixcity.io/mywallet/";
    public static string IDServerAddress = "https://test-n69.gihot.vn/";

}
