using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Agora.Rtc;

public class TestAgora : MonoBehaviour
{
    public string AppID;
    public string ChannelName;

    VideoSurface myView;
    VideoSurface remoteView;
    IRtcEngine mrtcEngine;

    // Start is called before the first frame update
    void Awake()
    {
        SetupUI();
    }

    // Update is called once per frame
    void Start()
    {
        
    }

    void SetupUI()
    {
        GameObject go = GameObject.Find("MyView");
        myView = go.AddComponent<VideoSurface>();

        go = GameObject.Find("LeaveButton");
        go?.GetComponent<Button>()?.onClick.AddListener(Leave);

        go = GameObject.Find("JoinButton");
        go?.GetComponent<Button>()?.onClick.AddListener(Join);
    }
  
    void Join()
    {

    }

    void Leave()
    {

    }
}
