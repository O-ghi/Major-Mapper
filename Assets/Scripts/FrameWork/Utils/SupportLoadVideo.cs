using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SupportLoadVideo : MonoBehaviour
{
    public string videoName;

    private void Awake()
    {
        var clipPlayer = GetComponent<VideoPlayer>();
        clipPlayer.clip = Resources.Load<VideoClip>(videoName);
    }
}
