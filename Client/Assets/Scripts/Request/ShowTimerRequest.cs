using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class ShowTimerRequest : BaseRequest {
    public override void Awake()
    {
        base.Awake();
        SendRequest();
    }

    public override void SendRequest()
    {
        m_ActionCode = ActionCode.ShowTimer;
        base.AddRequestMothed();
    }

    public override void OnResponse(string data)
    {

        Debug.Log("当前数据为："+ data);
        int receive = ParsePackage.JSONDataDeSerialize<int>(data); 
        //if (receive.ReturnCode==ReturnCode.Success)
        //{
            EventDispatcher.Instance.TriggerEvent<int>(GameConst.ShowTimerCallBack,receive);
       // }
        
    }
}
