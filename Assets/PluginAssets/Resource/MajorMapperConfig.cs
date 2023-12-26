using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GihotConfigList
{
    public static MajorMapperConfig GetMajorMapper(PlatformData targetPlatform)
    {
        MajorMapperConfig newConfig = new MajorMapperConfig();
        newConfig.platformData = targetPlatform;
        switch (targetPlatform)
        {
            case PlatformData.android_test:
                //set channel
                newConfig.Channel = Channel.majorMapper;
                newConfig.ChannelName = Channel.majorMapper.ToString();
                newConfig.versionPrefixUrl = "";
                newConfig.lastName = "Test";
                break;
            case PlatformData.android_dev:
                //set channel
                newConfig.Channel = Channel.majorMapper;
                newConfig.ChannelName = Channel.majorMapper.ToString();
                newConfig.versionPrefixUrl = "";
                newConfig.lastName = "Dev";
                break;
            case PlatformData.android_uat:
                //set channel
                newConfig.Channel = Channel.majorMapper;
                newConfig.ChannelName = Channel.majorMapper.ToString();
                newConfig.versionPrefixUrl = "";
                newConfig.lastName = "UAT";
                break;
            case PlatformData.android_release:
                //set channel
                newConfig.Channel = Channel.majorMapper;
                newConfig.ChannelName = Channel.majorMapper.ToString();
                newConfig.versionPrefixUrl = "";
                newConfig.lastName = "Release";
                break;
        }

        return newConfig;
    }
}

[System.Serializable]
public class MajorMapperConfig
{
    #region Params
    public Channel Channel = Channel.majorMapper;
    public string ChannelName = "???";
    public PlatformData platformData;
    public string versionPrefixUrl = "http://";
    public string lastName = "";
    //
    #endregion
}

public enum Language
{
    chinese,
    english,
}

public enum Channel
{
    //real channel
    majorMapper = 0,
}

public enum PlatformData
{
    //android
    android_test = 0,
    android_dev = 1,
    android_uat = 2,
    android_release = 3,
}

public enum ConfigChannelName
{

}