using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class GetRoomRequest : BaseRequest
{
    private RoomData ReciveData = null;
    private byte[] ReceiveData = null;
    public override void Awake()
    {
        base.Awake();
        EventDispatcher.Instance.AddEventListener(GameConst.SendGetRoomListRequest, SendRequest);
    }

    public override void SendRequest()
    {

        m_RequestCode = RequestCode.Room;
        m_ActionCode = ActionCode.GetRoomList;
        base.AddRequestMothed();
        string data = "getRoom";
        Debug.Log(data);
        base.SendRequest(data);
    }

    public override void Update()
    {

    }

    public override void OnResponse(string data)
    {

        ReciveData = ParsePackage.JSONDataDeSerialize<RoomData>(data);
        ReturnCode returnCode = ReciveData.ReturnCode;
        Debug.Log("取得房间数据：" + ReciveData.RoomListData.Count);
        if (returnCode == ReturnCode.Success)
        {
            Debug.Log("取得房间数据：" + ReciveData.RoomListData.Count);
            m_Facade.SetRoomInfoData(ReciveData);
            EventDispatcher.Instance.TriggerEvent(GameConst.GetRoomListSuccessCallBack);
        }

    }
}
