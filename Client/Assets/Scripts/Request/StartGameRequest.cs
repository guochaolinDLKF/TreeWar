using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameRequest : BaseRequest {
    public override void Awake()
    {
        base.Awake();
        m_RequestCode = RequestCode.Game;
        m_ActionCode = ActionCode.StartGame;
        base.AddRequestMothed();
        EventDispatcher.Instance.AddEventListener(GameConst.StartGameRequest, SendRequest);
    }

    public override void SendRequest()
    {
        string data = "startRoom";
        base.SendRequest(data);
    }

    public override void OnResponse(string data)
    {
        ScoreData reciveData = ParsePackage.JSONDataDeSerialize<ScoreData>(data);
        if (reciveData.ReturnCode == ReturnCode.Success)
        {
            EventDispatcher.Instance.TriggerEvent(GameConst.StartGameCallBack);
            Debug.Log("开始游戏");
        }
    }
}
