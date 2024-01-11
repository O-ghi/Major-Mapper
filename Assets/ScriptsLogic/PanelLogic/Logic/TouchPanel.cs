using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TouchPanel : UIBase
{
    public GameObject _JoyStick;
    public GameObject _GamePad;
    public GameObject _Button;
    public Transform _NextButton;
    public Transform _NextButton1;
    protected override void Init()
    {
        _JoyStick = transform.Find("JoyStick").gameObject;
        _GamePad = transform.Find("GamePad").gameObject;
        _Button = transform.Find("Button").gameObject;
        _NextButton = transform.Find("Next");
        _NextButton1 = transform.Find("Next (1)");
        _GamePad.SetActive(false);

        SetButton(_NextButton.GetOrAddComponent<TriggerListener>(), EventTriggerType.PointerClick, (_event, _gameobject) => SwitchScene());
        SetButton(_NextButton1.GetOrAddComponent<TriggerListener>(), EventTriggerType.PointerClick, (_event, _gameobject) => FakeData());

    }
    public void SwitchScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay(Maze)");
        _JoyStick.SetActive(false);
        _GamePad.SetActive(false);
        _Button.SetActive(false);

        EntityBaseLogic.Singleton.Clear();
    }
    public void FakeData()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameMain");
        EntityBaseLogic.Singleton.Clear();
        PanelManager.Clear();
        //NetworkingManager.instance.OnFakeData();
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
