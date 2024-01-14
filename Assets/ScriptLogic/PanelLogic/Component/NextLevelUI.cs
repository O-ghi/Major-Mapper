using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextLevelUI 
{
    public NextLevelPanel _NextLevelPanel;

    public TriggerListener NextLevelButton;

    public void Init()
    {
        NextLevelButton = _NextLevelPanel.transform.Find("Button/NextLevelButton").GetOrAddComponent<TriggerListener>();
    }
}
