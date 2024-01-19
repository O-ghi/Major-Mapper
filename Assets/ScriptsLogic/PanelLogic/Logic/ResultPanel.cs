using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResultPanel : UIBase
{
    private ResultUI _ResultUI;

    protected override void Init()
    {
        _ResultUI = new ResultUI();
        _ResultUI._resultPanel = this;
        _ResultUI.Init();
        InitButton();

    }

    private void InitButton()
    {
        SetButton(_ResultUI.ReturnBtn, EventTriggerType.PointerClick, (_event, _gameObject) => CloseBtn());

    }

    private void CloseBtn()
    {

    }

    private void GetResultData()
    {
        string baseURL = @"https://majormapperapi.azurewebsites.net/api/Test/GetTest/";

    }
}
