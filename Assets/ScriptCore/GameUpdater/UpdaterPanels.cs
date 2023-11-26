using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void MsgCallBack();

public class UpdaterPanels : MonoBehaviour {

    public GameObject progressPanel;
    public GameObject msgboxPanel;
    public GameObject msglabelPanel;
    public GameObject errorboxPanel;

    public Text msgboxlabel;
    public Slider progressbar;
    public Text progresslabel;
    public Text msglabel;
    public Text errorlabel;
    public Text progressMsgLabel;

    private GameObject m_currGo;

    private MsgCallBack m_OkCallback;
    private MsgCallBack m_CancelCallback;
    private MsgCallBack m_ErrorCallback;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowMsgBox(string msg, MsgCallBack okCallback = null, MsgCallBack cancelCallback = null)
    {
        Turn(msgboxPanel);
        msgboxlabel.text = msg;
        m_OkCallback = okCallback;
        m_CancelCallback = cancelCallback;
    }

    public void ShowMsgLabel(string msg)
    {
        Turn(msglabelPanel);
        msglabel.text = msg;
    }

    public void ShowProgress(float progress,string message = "")
    {
        Turn(progressPanel);
        progressbar.value = progress;
        progresslabel.text = string.Format("{0}%", (int)(progress * 100));
        progressMsgLabel.text = message;
    }

    public void ShowErrorBox(string msg, MsgCallBack okCallback = null)
    {
        Turn(errorboxPanel);
        errorlabel.text = msg;
        m_ErrorCallback = okCallback;
    }

    private void Turn(GameObject target)
    {
        if (m_currGo != null && m_currGo != target)
            m_currGo.SetActive(false);

        if (!target.activeSelf)
            target.SetActive(true);
        m_currGo = target;
    }
    
    public void OkCallback()
    {
        if (m_OkCallback != null)
            m_OkCallback();
        m_currGo.SetActive(false);
    }

    public void CancelCallback()
    {
        if (m_CancelCallback != null)
            m_CancelCallback();
    }

    public void ErrorOkCallback()
    {
        if (m_ErrorCallback != null)
            m_ErrorCallback();
    }

}
