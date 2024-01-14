/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2019.05.22
*/
using SimpleJSON;

public class SpecialMarker : BaseDownloader
{
    private static SpecialMarker instance;
    public static SpecialMarker Singleton
    {
        get
        {
            if (instance == null)
                instance = new SpecialMarker();
            return instance;
        }
    }

    public string ip { get; private set; }
    public bool auditing { get; private set; }
    public string Content { get; private set; }
    
    public override void Download()
    {
        var url = getDownloadUrl();
        WWWLoader.Singleton.Download(url, onLoadCmp, onLoadUpdate, "", loadCache, false);
    }

    protected override string getDownloadUrl()
    {
        return string.Format("");
    }

    protected override void onLoadCmp(string path, bool success, byte[] data)
    {
        base.onLoadCmp(path, success, data);
        if (!Loaded)
            return;

        if (success && data != null)
        {
            try
            {
                Content = System.Text.Encoding.UTF8.GetString(data);
                Debuger.Log("SpecialMarker--------->>", path, Content);
                JSONClass json = JSONClass.Parse(Content) as JSONClass;
                if (json != null)
                {
                    ip = json["ip"].Value;
                    auditing = json["auditing"].AsBool;
                    if(auditing)
                    {
                        Debuger.Err("SpecialMarker 标记成功");
                        VersionConfig.IsInAuditing = true;
                    }
                }
            }catch(System.Exception e)
            {
                Debuger.Err("SpecialMarker 解析json失败>" + path);
            }
        }
        else
        {
            Debuger.Err("SpecialMarker 失败>" + path);
        }
        
        if (mCallback != null)
            mCallback();
    }
}