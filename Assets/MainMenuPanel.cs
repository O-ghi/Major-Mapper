using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuPanel : UIBase
{
    public Transform playgamebutton;
    protected override void Init()
    {
        playgamebutton = transform.Find("PlayGame");
        SetButton(playgamebutton.GetOrAddComponent<TriggerListener>(), EventTriggerType.PointerClick, (_event, _gameobjet) => EnterScene());
    }
    public void EnterScene()
    {
        if (Application.isPlaying)
        {
            SceneManager.ChangeScene(1);
            this.gameObject.SetActive(false);
        }
    }
}
