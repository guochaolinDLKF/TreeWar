using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class CreatRoomRequest : BaseRequest {
    private RoomData ReciveData = null;
    public override void Awake()
    {
        base.Awake();
        EventDispatcher.Instance.AddEventListener(GameConst.SendCreatRoomRequest, SendRequest);
    }
    public override void SendRequest() 
    {
       
        m_RequestCode = RequestCode.Room;
        m_ActionCode = ActionCode.CreatRoom;
        base.AddRequestMothed();
        string data = "creatRoom";
        base.SendRequest(data); 
    }

    public override void OnResponse(string data)
    {
        Debug.Log("创建房间列表回调数据：" + data);
        ReciveData = ParsePackage.JSONDataDeSerialize<RoomData>(data);
        ReturnCode returnCode = ReciveData.ReturnCode;
        if (returnCode==ReturnCode.Success)
        {
            for (int i = 0; i < ReciveData.RoomListData.Count; i++)
            {
                m_Facade.SetRoomPlayerListData(ReciveData.RoomListData[i]);
            }
            
            m_Facade.SetRoomInfoData(ReciveData);
            EventDispatcher.Instance.TriggerEvent(GameConst.CreatRoomListSuccessCallBack);
        }
        
    }
}
