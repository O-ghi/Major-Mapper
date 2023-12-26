using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingUI 
{
    public FishingPanel _fishingPanel;

    public Slider _CatchPercentage;
    public Rigidbody2D _CatchingBar;
    public Transform _TopBar;
    public Transform _BottomBar;
    public RectTransform _Fish;

    public void Init()
    {
        _CatchPercentage = _fishingPanel.transform.Find("Holder/CatchPercentage").GetComponent<Slider>();
        _CatchingBar = _fishingPanel.transform.Find("Holder/fishingbar/CatchingBar").GetComponent<Rigidbody2D>();
        _TopBar = _fishingPanel.transform.Find("Holder/fishingbar/Top");
        _BottomBar = _fishingPanel.transform.Find("Holder/fishingbar/Bottom");
        _Fish = _fishingPanel.transform.Find("Holder/fishingbar/Fish").GetOrAddComponent<RectTransform>();
    }
}
