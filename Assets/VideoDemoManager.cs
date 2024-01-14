using Agora.Rtc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class VideoDemoManager : MonoBehaviour
{
    [Header("Agora Settings")]
    // The name of the channel
    public string channelName = "defaultChannel";

    public string appId;

    [Header("Scene References")]
    public Button joinButton;
    public Text buttonText;


    private IRtcEngine mRtcEngine = null;
    private uint myId;
    private void Start()
    {
        #if (UNITY_2018_3_OR_NEWER)
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        if(!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif



    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
