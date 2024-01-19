using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Agora.Rtc;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class PlayerVideo : MonoBehaviour
{
    public VideoSurface VideoSurface;
    // Start is called before the first frame update
    public void Set(uint uid)
    {
        VideoSurface.gameObject.SetActive(true);
        VideoSurface.SetEnable(true);
        VideoSurface.SetForUser(uid);
    }
    public void Clear()
    {
        VideoSurface.SetEnable(false);
    }
}
