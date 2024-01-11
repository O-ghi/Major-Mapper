using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskShowUI
{
    public TaskShowPanel taskShowPanel;

    //
    public Text _TitleTask;


    public void Init()
    {
        _TitleTask = taskShowPanel.transform.Find("Background/TitleTask").GetOrAddComponent<Text>();
    }
}
