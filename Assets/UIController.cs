using Agora.Rtc;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System;
using System.IO;

public class UIController
{
    public Button joinButton;
    public Button leaveButton;

    internal IRtcEngine rtcEngine;
    internal string _appID;
    internal string _channelName;
    internal string _token;
    internal uint remoteUid;
    internal VideoSurface LocalView;
    internal VideoSurface RemoteView;
    internal AREA_CODE region = AREA_CODE.AREA_CODE_GLOB;
    internal string userRole = "";

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    // Define an ArrayList of permissions required for Android devices.
    private ArrayList permissionList = new ArrayList() { Permission.Camera, Permission.Microphone };
#endif

    void Start()
    {
        
        joinButton.onClick.AddListener(JoinVideoChat);
        leaveButton.onClick.AddListener(Leave);

        rtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();  
        rtcEngine.EnableVideo();
    }
    public void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        // Check for each permission in the permission list and request the user to grant it if necessary.
        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
#endif
    }
    void OnDestroy()
    {
        if (rtcEngine != null)
        {
            // Destroy the engine.
            rtcEngine.LeaveChannel();
            rtcEngine.Dispose();
            rtcEngine = null;
        }
    }

    void JoinVideoChat()
    {
        // Create an instance of the engine.
        SetupAgoraEngine();

        // Setup local video view.
        SetupLocalVideo();

        // Join the channel using the specified token and channel name.
        rtcEngine.JoinChannel(_token, _channelName);
    }
    public virtual void SetupLocalVideo()
    {
        // Set the local video view.
        LocalView.SetForUser(remoteUid, _channelName);

        // Start rendering local video.
        LocalView.SetEnable(true);

    }

    public virtual void SetupAgoraEngine()
    {
        if (_appID == "" || _token == "")
        {
            Debug.Log("Please set an app ID and a token in the config file.");
            return;
        }
        // Create an instance of the video SDK engine.
        rtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();

        // Set context configuration based on the product type
        CHANNEL_PROFILE_TYPE channelProfile  = true
            ? CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION
            : CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;

        RtcEngineContext context = new RtcEngineContext(_appID, 0, channelProfile,
            AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT, region, null);

        rtcEngine.Initialize(context);

        // Enable the video module.
        rtcEngine.EnableVideo();

        // Set the user role as broadcaster.
        rtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

        // Attach the eventHandler
        InitEventHandler();

    }

    public virtual void InitEventHandler()
    {
        rtcEngine.InitEventHandler(new UserEventHandler(this));
    }
    // An event handler class to deal with video SDK events
    internal class UserEventHandler : IRtcEngineEventHandler
    {
        internal readonly UIController agoraManager;

        internal UserEventHandler(UIController videoSample)
        {
            agoraManager = videoSample;
        }
        // This callback is triggered when the local user joins the channel.
        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log("You joined channel: " + connection.channelId);
        }

        // This callback is triggered when a remote user leaves the channel or drops offline.
        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            agoraManager.DestroyVideoView(uid);
        }
        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            //agoraManager.MakeVideoView(uid, connection.channelId);
            // Save the remote user ID in a variable.
            agoraManager.remoteUid = uid;
        }
    }

    //public void MakeVideoView(uint uid, string channelName)
    //{
    //    // Create and configure a remote user's video view
    //    AgoraUI agoraUI = new AgoraUI();
    //    GameObject userView = agoraUI.MakeRemoteView(uid.ToString());
    //    userView.AddComponent<AgoraUI>();

    //    VideoSurface videoSurface = userView.AddComponent<VideoSurface>();
    //    videoSurface.SetForUser(uid, channelName, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
    //    videoSurface.OnTextureSizeModify += (int width, int height) =>
    //    {
    //        float scale = (float)height / (float)width;
    //        videoSurface.transform.localScale = new Vector3(-5, 5 * scale, 1);
    //        Debug.Log("OnTextureSizeModify: " + width + " " + height);
    //    };
    //    videoSurface.SetEnable(true);

    //    RemoteView = videoSurface;
    //}
    public virtual void Leave()
    {
        if (rtcEngine != null)
        {
            // Leave the channel and clean up resources
            rtcEngine.LeaveChannel();
            rtcEngine.DisableVideo();
            LocalView.SetEnable(false);
            DestroyVideoView(remoteUid);
            OnDestroy();
        }
    }
    public void DestroyVideoView(uint uid)
    {
        var userView = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(userView, null))
        {
            userView.SetActive(false); // Deactivate the GameObject
        }
    }
}
