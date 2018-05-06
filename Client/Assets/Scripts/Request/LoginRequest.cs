using System.Collections;
using System.Collections.Generic;
using System.Text;
using Common;
using UnityEngine;

public class LoginRequest : BaseRequest
{

    //private LoginPanel m_LoginPanel;
    public override void Awake()
    {
        base.Awake();
        EventDispatcher.Instance.AddEventListener<string, string>(GameConst.SendLoginRequest, SendRequest);
    }

    public void SendRequest(string username, string password)
    {
       
        m_RequestCode = RequestCode.UserData;
        m_ActionCode = ActionCode.Login;
        base.AddRequestMothed();
        UserData sendUserData = new UserData(ReturnCode.None, null, username, password);
        base.SendRequest(ParsePackage.JSONDataSerialize(sendUserData));
    }

    public override void OnResponse(string data)
    {
        Debug.Log("收到登陆回调："+data.Length);
        ScoreData scoreData = ParsePackage.JSONDataDeSerialize<ScoreData>(data);
        //string[] strs = data.Split(',');

        ReturnCode returnCode = scoreData.ReturnCode;
        EventDispatcher.Instance.TriggerEvent(GameConst.OnLoginCallBack, returnCode);
        // m_LoginPanel.OnLoginResponce(returnCode);
        if (returnCode == ReturnCode.Success)
        {
            m_Facade.SetUserInfoData(scoreData);
        }

    }
}
