using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextLevelPanel : UIBase
{
    private NextLevelUI _NextLevelUI;

    protected override void Init()
    {
        _NextLevelUI = new NextLevelUI();
        _NextLevelUI._NextLevelPanel = this;
        _NextLevelUI.Init();
        InitButton();
    }

    private void InitButton()
    {
        SetButton(_NextLevelUI.NextLevelButton, EventTriggerType.PointerClick, (_event, _gameobject) => NextLevelButton());
    }

    private void NextLevelButton()
    {
        GameGlobal.Restart();
    }
}
