using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private Button m_LoginBtn;
    private Button m_RegistBtn;
    private InputField m_Username;
    private InputField m_Password;
   // private LoginRequest m_LoginRequest;
    private Tweener tween;
    //[SerializeField]
    //private RoomRequest m_RoomRequest;
    public override void OnInit()
    {
        



        m_LoginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        m_RegistBtn = transform.Find("RegistBtn").GetComponent<Button>();
        m_Username = transform.Find("UserName/InputUsername").GetComponent<InputField>();
        m_Password = transform.Find("Password/InputPassword").GetComponent<InputField>();
        //m_LoginRequest = GetComponent<LoginRequest>();
        m_LoginBtn.onClick.AddListener(LoginOnClick);
        m_RegistBtn.onClick.AddListener(RegistOnClick);
        gameObject.SetActive(true);
        tween = transform.DOLocalMoveX(0, 0.5f);
        tween.SetAutoKill(false);// 不自动消失掉动画
        tween.SetEase(Ease.Linear);
        tween.Pause();
    }

    public override void OnEnter(params object[] paramData)
    {
        if (PlayerPrefs.HasKey("username")&& PlayerPrefs.HasKey("password"))
        {
            PlayerPrefs.GetString("username");
            PlayerPrefs.GetString("password");
        }
        EventDispatcher.Instance.AddEventListener<ReturnCode>(GameConst.OnLoginCallBack, OnLoginResponce);
        EventDispatcher.Instance.AddEventListener(GameConst.GetRoomListSuccessCallBack, GetRoomListSuccessCallBack);
        tween.PlayForward();
    }

    void RegistOnClick()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        m_UIMgr.PopPanel();
        m_UIMgr.PushPanel(UIPanelType.Regist);
    }
    void LoginOnClick()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        if (String.IsNullOrEmpty(m_Username.text) || String.IsNullOrEmpty(m_Password.text))
        {
            m_UIMgr.ShowMassage("账户名和密码不能为空");
            return;
        }
        PlayerPrefs.SetString("username", m_Username.text);
        PlayerPrefs.SetString("password", m_Password.text);
        EventDispatcher.Instance.TriggerEvent(GameConst.SendLoginRequest, m_Username.text, m_Password.text);
        //m_LoginRequest.SendRequest(m_Username.text, m_Password.text);

    }

    public void OnLoginResponce(ReturnCode returnCode)
    {
        Debug.Log("服务器返回值为：" + (ReturnCode)returnCode);
        if (returnCode == ReturnCode.Success)
        {
            SendGetRoomListRequest();
        }
        else
        {
            m_UIMgr.ShowTipMessageAsync("用户名或者密码错误，请重新输入");
        }
    }
    void SendGetRoomListRequest()
    {
        EventDispatcher.Instance.TriggerEvent(GameConst.SendGetRoomListRequest);
    }
    public void GetRoomListSuccessCallBack()
    {    //登录成功进入房间列表
        m_UIMgr.PopPanel();
        m_UIMgr.SyncPushPanel(UIPanelType.RoomList);
    }
    public override void OnExit()
    {
        tween.PlayBackwards();
        EventDispatcher.Instance.RemoveEventListener<ReturnCode>(GameConst.OnLoginCallBack, OnLoginResponce);
        EventDispatcher.Instance.RemoveEventListener(GameConst.GetRoomListSuccessCallBack, GetRoomListSuccessCallBack);
    }
}
