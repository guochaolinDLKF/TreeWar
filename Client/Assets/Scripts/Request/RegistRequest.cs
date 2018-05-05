using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RegistRequest : BaseRequest
{

  

    public override void Awake()
    {
        base.Awake();
        EventDispatcher.Instance.AddEventListener<string,string>(GameConst.SendRegistRequest, SendRequest);
    }
    public void SendRequest(string username, string password)
    {
        m_RequestCode = RequestCode.UserData;
        m_ActionCode = ActionCode.Regiest;
        base.AddRequestMothed();
        PlayerPrefs.DeleteAll();
        string userid = Util.GuidTo16String();
        PlayerPrefs.SetString("UserId", userid);
        UserData sendUserData = new UserData(ReturnCode.None, userid, username, password);
        Debug.Log("发送注册");
        base.SendRequest(ParsePackage.JSONDataSerialize(sendUserData));
    }
    public override void OnResponse(string data)
    {
        Debug.Log("服务器返回的注册回调为："+ data);
        ScoreData userData = ParsePackage.JSONDataDeSerialize<ScoreData>(data);
        //string[] strs = data.Split(',');
        ReturnCode returnCode = userData.ReturnCode;
       
        EventDispatcher.Instance.TriggerEvent(GameConst.OnRegistCallBack, returnCode);
        //m_RegistPanel.OnLoginResponce(returnCode);

        if (returnCode == ReturnCode.Success)
        {
            m_Facade.SetUserInfoData(userData);
        }
    }
}
