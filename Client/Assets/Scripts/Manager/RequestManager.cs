using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RequestManager : BaseManager
{
   
    public RequestManager(GameFacade facede) : base(facede)
    {
     
    }
    Dictionary<ActionCode,BaseRequest> requestDict=new Dictionary<ActionCode, BaseRequest>();
    /// <summary>
    /// 添加请求
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="request"></param>
    public void AddResuest(ActionCode actionCode, BaseRequest request)
    {
       
        if (requestDict.ContainsKey(actionCode))
        {
            return;
        }
        requestDict.Add(actionCode, request);
    }

    public override void Init()
    {
        GameObject goRQ = new GameObject("RequestController");
        goRQ.AddComponent<LoginRequest>();
        goRQ.AddComponent<RegistRequest>();
        goRQ.AddComponent<GetRoomRequest>();
        goRQ.AddComponent<CreatRoomRequest>();
        goRQ.AddComponent<JoinRoomRequest>();
        goRQ.AddComponent<UpdateRoomRequest>();
        goRQ.AddComponent<QuitRoomRequest>();
        goRQ.AddComponent<StartGameRequest>();
        goRQ.AddComponent<ShowTimerRequest>();
        goRQ.AddComponent<StartPlayRequest>();

    }

    public void RemoveRequest(ActionCode actionCode)
    {
        requestDict.Remove(actionCode);
    }
    /// <summary>
    /// 响应控制
    /// </summary>
    public void HandleResponse(ActionCode actionCode, string data)
    {
        
        BaseRequest request= requestDict.TryGet(actionCode);
        if (request == null)
        {
            Debug.LogError("找不到请求的消息类型"+ actionCode);return;
        }
        request.OnResponse(data);
    } 
}
