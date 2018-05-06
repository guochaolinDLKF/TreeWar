using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class QuitLoginRequest : BaseRequest
{
    public override void Awake()
    {
        base.Awake();
        EventDispatcher.Instance.AddEventListener<string>(GameConst.QuitLoginRequest, SendQuitLoginRequest);
    }
     
    public void SendQuitLoginRequest(string userNmae)
    { 
        m_RequestCode = RequestCode.UserData;
        m_ActionCode = ActionCode.QuitLogin;
        base.AddRequestMothed();
        UserData user=new UserData(ReturnCode.None, PlayerPrefs.GetString("UserId"),userNmae,null);
        base.SendRequest(ParsePackage.JSONDataSerialize(user));
    }

    public override void OnResponse(string data)
    {
      UserData user=  ParsePackage.JSONDataDeSerialize<UserData>(data);
        switch (user.ReturnCode)
        {
                case ReturnCode.Success:
                EventDispatcher.Instance.TriggerEvent(GameConst.QuitLoginCallBack);
                break;
                case ReturnCode.Fail:
                m_Facade.ShowTipMessageAsync("返回登录失败");
                break;
        }
    }
}
