using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class UpdateRoomRequest : BaseRequest {
    public override void Awake()
    {
        base.Awake();
        SendRequest();
    }

    public override void SendRequest()
    {
        m_ActionCode = ActionCode.UpdateRoomPlayerList;
        m_RequestCode =RequestCode.Room;
        base.AddRequestMothed();
        //string sendData = "";
        //base.SendRequest(sendData);
    }

    public override void OnResponse(string data)
    {
        Debug.Log("更新玩家列表");
        RoomAntity reciveData = ParsePackage.JSONDataDeSerialize<RoomAntity>(data);
        if (reciveData.ReturnCode==ReturnCode.Success)
        {
            m_Facade.SetRoomPlayerListData(reciveData);
            EventDispatcher.Instance.TriggerEvent(GameConst.UpdateRoomPlayerList);
        }
    }
}
