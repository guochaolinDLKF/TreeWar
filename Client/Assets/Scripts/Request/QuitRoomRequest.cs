using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class QuitRoomRequest : BaseRequest {
    public override void Awake()
    {
        base.Awake();
        EventDispatcher.Instance.AddEventListener(GameConst.QuitRoomRequest, SendRequest);
        m_ActionCode = ActionCode.QuitRoom;
        m_RequestCode = RequestCode.Room;
        base.AddRequestMothed();
    }

    public override void SendRequest()
    {
        m_ActionCode=ActionCode.QuitRoom;
        m_RequestCode=RequestCode.Room;
        //base.AddRequestMothed();
        string send = "quit";
        base.SendRequest(ParsePackage.JSONDataSerialize(send));
    }

    public override void OnResponse(string data)
    {
        Debug.Log("获取退出房间回调");
        RoomAntity recive = ParsePackage.JSONDataDeSerialize<RoomAntity>(data);
        if (recive.ReturnCode==ReturnCode.Success)
        {
            EventDispatcher.Instance.TriggerEvent(GameConst.QuitRoomCallBack);
        }
    }
}
