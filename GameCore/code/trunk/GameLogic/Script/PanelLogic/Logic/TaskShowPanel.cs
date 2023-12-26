using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskShowPanel : UIBase
{
    TaskShowUI _taskShowUI;

    protected override void Init()
    {
        _taskShowUI = new TaskShowUI();
        _taskShowUI.taskShowPanel = this;
        _taskShowUI.Init();
    }

    public void SetTitle(string title)
    {
        _taskShowUI._TitleTask.text = title;
    }
}
