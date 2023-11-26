using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchPanel : UIBase
{
    public GameObject _JoyStick;
    public GameObject _GamePad;
    public GameObject _Button;
    protected override void Init()
    {
        _JoyStick = transform.Find("JoyStick").gameObject;
        _GamePad = transform.Find("GamePad").gameObject;
        _Button = transform.Find("Button").gameObject;

        _GamePad.SetActive(false);
    }

    public void ChangeMode(int mode)
    {
        switch (mode)
        {
            case 0:
                {
                    _JoyStick.SetActive(true);
                    _GamePad.SetActive(false);
                    break;
                }
            case 1:
                {
                    _JoyStick.SetActive(false);
                    _GamePad.SetActive(true);
                    break;
                }
        }
    }
}
