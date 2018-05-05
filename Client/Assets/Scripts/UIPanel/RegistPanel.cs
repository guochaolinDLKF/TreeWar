using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RegistPanel : BasePanel
{
    private Button m_ReturnBtn;
    private Button m_RegistAndLoginBtn;
    private InputField m_Username;
    private InputField m_Password;
    private Tweener tween;
    public override void OnInit()
    {
        

        m_ReturnBtn = transform.Find("ReturnBtn").GetComponent<Button>();
        m_RegistAndLoginBtn = transform.Find("RegistBtn").GetComponent<Button>();
        m_Username = transform.Find("UserName/Inputusername").GetComponent<InputField>();
        m_Password = transform.Find("Password/Inputpassword").GetComponent<InputField>();
        //m_Regist = GetComponent<RegistRequest>();
        m_RegistAndLoginBtn.onClick.AddListener(OnRegistLogin);
        m_ReturnBtn.onClick.AddListener(OnReturnLogin);
        gameObject.SetActive(true);
        tween = transform.DOLocalMoveX(0, 0.5f);
        tween.SetAutoKill(false);// 不自动消失掉动画
        tween.SetEase(Ease.Linear);
        Debug.Log("生成并设置");
        tween.Pause();
    }

    public override void OnEnter(params object[] paramData)
    {
        EventDispatcher.Instance.AddEventListener<ReturnCode> (GameConst.OnRegistCallBack, OnLoginResponce);
        EventDispatcher.Instance.AddEventListener(GameConst.GetRoomListSuccessCallBack, GetRoomListSuccessCallBack);
        tween.PlayForward();
    }
    void OnRegistLogin()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        if (String.IsNullOrEmpty(m_Username.text) || String.IsNullOrEmpty(m_Password.text))
        {
            m_UIMgr.ShowMassage("用户名或者密码不能为空");
            return;
        }
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("username", m_Username.text);
        PlayerPrefs.SetString("password", m_Password.text);
        EventDispatcher.Instance.TriggerEvent<string,string>(GameConst.SendRegistRequest, m_Username.text, m_Password.text);
    }
    public void OnLoginResponce(ReturnCode returnCode)
    {
        Debug.Log("服务器返回值为：" + (ReturnCode)returnCode);
        if (returnCode == ReturnCode.Success)
        {
            m_UIMgr.ShowTipMessageAsync("注册成功");
            SendGetRoomListRequest();
            //注册成功进入房间
        }
        else
        {
            m_UIMgr.ShowTipMessageAsync("用户名重复，请重新输入");
        }
    }

    void SendGetRoomListRequest()
    {
        EventDispatcher.Instance.TriggerEvent(GameConst.SendGetRoomListRequest);
    }


    public void GetRoomListSuccessCallBack()
    { 
        m_UIMgr.PopPanel(); 
        m_UIMgr.SyncPushPanel(UIPanelType.RoomList);
    }


    void OnReturnLogin()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        m_UIMgr.PopPanel();
        m_UIMgr.PushPanel(UIPanelType.Login);
    }
    public override void OnExit()
    {
        tween.PlayBackwards();
        EventDispatcher.Instance.RemoveEventListener<ReturnCode>(GameConst.OnRegistCallBack, OnLoginResponce);
        EventDispatcher.Instance.RemoveEventListener(GameConst.GetRoomListSuccessCallBack, GetRoomListSuccessCallBack);
    }
}
