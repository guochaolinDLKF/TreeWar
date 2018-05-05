using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    private Button m_Btn;
    private Button m_QuitBtn;
    private Tweener tw;
    public override void OnInit()
    {
        m_Btn = transform.Find("LoginButton").GetComponent<Button>();
        m_QuitBtn = transform.Find("QuitButton").GetComponent<Button>();
        m_Btn.onClick.AddListener(OnLoginClick);
        m_QuitBtn.onClick.AddListener(QuitBtnClick);
        tw = transform.DOLocalMoveX(0, 0.5f);
        tw.SetAutoKill(false);// 不自动消失掉动画
        tw.SetEase(Ease.Linear);
        tw.Pause();
    }

    public override void OnEnter(params object[] paramData)
    { 
        tw.PlayForward();
    } 

    void QuitBtnClick()
    {
        Application.Quit();
    }
    public override void OnExit()
    {
        tw.PlayBackwards();
    }

    void OnLoginClick()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        m_UIMgr.PopPanel();
        m_UIMgr.PushPanel(UIPanelType.Login); 

    }
}
