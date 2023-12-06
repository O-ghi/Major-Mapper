using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoginPanel : UIBase
{
    private LoginUI _LoginUI;

    protected override void Init()
    {
        _LoginUI = new LoginUI();
        _LoginUI._LoginPanel = this;
        _LoginUI.Init();
        InitButton();
    }

    private void InitButton()
    {
        SetButton(_LoginUI.SubmitButton, EventTriggerType.PointerClick, (_event, _gameObject) => SubmitButton());
        
    }

    private void SubmitButton()
    {
        if(string.IsNullOrEmpty(_LoginUI.Email.text) || string.IsNullOrEmpty(_LoginUI.Password.text))
        {
            //Hiện popup thông báo
            return;
        }
        NetworkingManager.instance.OnLogInButton();
    }
}
