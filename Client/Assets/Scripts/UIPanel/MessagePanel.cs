using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : BasePanel
{
    private Text m_txt;
    private string message = String.Empty;
    private Tweener tw;
    public override void OnInit()
    {
        m_txt = GetComponent<Text>();
        tw = m_txt.DOFade(0, 0.2f);
        tw.SetAutoKill(false);
        tw.Pause();
    }

    private void Update()
    {
        if (message != String.Empty)
        {
            ShowTipMessage(message);

        }
    }
    public override void OnEnter(params object[] paramData)
    { 
        m_UIMgr.InjectMsgPanel(this);
        tw.PlayBackwards();
    }

    public void ShowTipMessageAsync(string data)
    {
        message = data;
    }
    public void ShowTipMessage(string data)
    {
        tw.PlayBackwards();
        m_txt.text = data;
        Invoke("Hide", 1.5f);

    }

    void Hide()
    {
        tw.PlayForward();
        message = String.Empty;



    }
}
