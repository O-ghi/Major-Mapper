using SimpleJSON;

public class VersionConfigChecker : BaseDownloader
{
    private static VersionConfigChecker instance;
    public static VersionConfigChecker Singleton
    {
        get
        {
            if (instance == null)
                instance = new VersionConfigChecker();
            return instance;
        }
    }

    /// app版本号
    public int AppVersion { get; private set; }
    /// 是否为强更版本（app）
    public bool IsForceUpdateApp { get; private set; }
    /// 强更App时是否跳转到引用商店（不是则内部下载）
    public bool AppStoreUpdateApp { get; private set; }
    ///强更资源版本号
    public int ForceResVersion { get; private set; }

    /// 游戏内游戏内提示资源更新
    public bool TipForceResInGame { get; private set; }

    /// 游戏内强制更新强更资源
    public bool IsForceUpdateResInGame { get; private set; }

    public string Content { get; private set; }

    public bool Success { get; private set; }

    public override void Download()
    {
        string url = getDownloadUrl();
        WWWLoader.Singleton.Download(url, onLoadCmp, onLoadUpdate, System.DateTime.Now.Ticks.ToString(), loadCache);
    }

    protected override void onLoadCmp(string path, bool success, byte[] data)
    {
        Success = false;
        base.onLoadCmp(path, success, data);
        if (!Loaded)
            return;

        if (success && data != null)
        {
            string str = System.Text.Encoding.Default.GetString(data);
            Debuger.Log(str);
            Content = str;
            JSONClass json = JSONClass.Parse(str) as JSONClass;
            if (json != null && json["app"] != null)
            {
                Success = true;
                AppVersion = json["app"].AsInt;
                IsForceUpdateApp = json["forceApp"].AsBool;
                AppStoreUpdateApp = json["appStore"].AsBool;

                ForceResVersion = json["forceRes"].AsInt;
                TipForceResInGame = json["tipForceRes"].AsBool;
                IsForceUpdateResInGame = json["forceForceRes"].AsBool;

                //版本号有提升，通知逻辑层
                if(AppVersion > LocalConfig.Singleton.LocalAppVersion)
                {
                    if (mCallback != null)
                        mCallback();
                }
                else if (ForceResVersion > LocalConfig.Singleton.LocalForceResVersion && TipForceResInGame)
                {
                    if (mCallback != null)
                        mCallback();
                }
            }
        }
    }
}