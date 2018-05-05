using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    [SerializeField]
    private Text m_TimerTxt;

    private int m_timer = -1;
    private Tweener m_Tw;
    public override void OnInit()
    {
        m_TimerTxt.transform.localScale = Vector3.zero;
        m_Tw = m_TimerTxt.transform.DOScale(2, 0.3f);
        m_Tw.SetAutoKill(false);// 不自动消失掉动画
        m_Tw.SetEase(Ease.Linear);
        m_Tw.Pause();
    }

    public override void OnEnter(params object[] paramData)
    {
        EventDispatcher.Instance.AddEventListener<int>(GameConst.ShowTimerCallBack, ShowSuncTimer);
    }

    void Update()
    {
        if (m_timer != -1)
        {
            ShowTimer();
            m_timer = -1;
        }
    }

    void ShowSuncTimer(int timer)
    {
        this.m_timer = timer;
    }

    void ShowTimer()
    {
        m_TimerTxt.text = m_timer.ToString();
        m_Tw.PlayForward();
        m_Tw.SetDelay(0.4f);
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.Alert);
        m_Tw.OnComplete(() =>
        {
            m_Tw.PlayBackwards();
        });
    }

    public override void OnExit()
    {
        EventDispatcher.Instance.RemoveEventListener<int>(GameConst.ShowTimerCallBack, ShowSuncTimer); 
    }
}
