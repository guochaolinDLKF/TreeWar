using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class StartPlayRequest : BaseRequest {
    public override void Awake()
    {
        base.Awake();
        SendRequest();
    }

    public override void SendRequest()
    {
        m_ActionCode = ActionCode.StartPlay;
        base.AddRequestMothed();
    }

    public override void OnResponse(string data)
    {
        Debug.Log("当前数据为：" + data);
        //MsgCallBack receive = ParsePackage.JSONDataDeSerialize<MsgCallBack>(data);
        //if (receive.ReturnCode == ReturnCode.Success)
        //{
           // EventDispatcher.Instance.TriggerEvent(GameConst.ShowTimerCallBack, int.Parse(data));
        //}
    }
}
