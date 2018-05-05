using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class JoinRoomRequest : BaseRequest
{
    public override void Awake()
    {
        base.Awake();
        EventDispatcher.Instance.AddEventListener<string>(GameConst.SendJoinRoomRequest, SendJoinRoomRequest);
    }

    public void SendJoinRoomRequest(string playername)
    {
        
        m_RequestCode = RequestCode.Room;
        m_ActionCode = ActionCode.JionRoom;
        base.AddRequestMothed();
        ScoreData roomInfo = new ScoreData(ReturnCode.None, 0, playername, 0, 0);
        base.SendRequest(ParsePackage.JSONDataSerialize(roomInfo));
    }

    public override void OnResponse(string data)
    {
        Debug.Log("加入房间：" + data);
        RoomAntity roomPlayerListData = ParsePackage.JSONDataDeSerialize<RoomAntity>(data);
        ReturnCode returnCode = roomPlayerListData.ReturnCode;
        switch (returnCode)
        {
            case ReturnCode.Success:
                m_Facade.SetRoomPlayerListData(roomPlayerListData);
                EventDispatcher.Instance.TriggerEvent(GameConst.JoinRoomNetCallBack);
                break;
            case ReturnCode.None:
                m_Facade.ShowTipMessageAsync("房间已被销毁");
                break;
            case ReturnCode.Fail:
                m_Facade.ShowTipMessageAsync("房间已满，无法加入");
                break;

        }
    }
}
