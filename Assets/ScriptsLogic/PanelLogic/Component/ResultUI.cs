using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResultUI 
{
    public ResultPanel _resultPanel;

    public TriggerListener ReturnBtn;
    public Transform[] ListResult;
    public void Init()
    {
        ReturnBtn = _resultPanel.transform.Find("resultBtn").GetOrAddComponent<TriggerListener>();
        ListResult = new Transform[] { _resultPanel.transform.Find("ListResult/Result"), _resultPanel.transform.Find("ListResult/Result (1)"), _resultPanel.transform.Find("ListResult/Result (2)"), _resultPanel.transform.Find("ListResult/Result (3)"), _resultPanel.transform.Find("ListResult/Result (4)"), _resultPanel.transform.Find("ListResult/Result (5)")};

    }
}
