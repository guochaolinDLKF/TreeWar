using Common;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameFacade : SingletonMMo<GameFacade>
{



    public UIManager m_UiMgr { get; private set; }
    public AudioManager m_AudioMgr { get; private set; }
    public PlayerManager m_PlayerMgr { get; private set; }
    public RequestManager m_RequestMgr { get; private set; }
    public CameraManager m_CameraMgr { get; private set; }
    public ClientManager m_ClientMgr { get; private set; }
    public ResourcesManager m_ResMgr { get; private set; }
    void Update()
    {
        m_UiMgr.Update();
        m_RequestMgr.Update();
        m_AudioMgr.Update();
        m_PlayerMgr.Update();
        m_CameraMgr.Update();
        m_ClientMgr.Update();
        m_ResMgr.Update();
    }
    public void InitManager()
    {
        m_UiMgr = new UIManager(this);
        m_AudioMgr = new AudioManager(this);
        m_CameraMgr = new CameraManager(this);
        m_PlayerMgr = new PlayerManager(this);
        m_RequestMgr = new RequestManager(this);
        m_ClientMgr = new ClientManager(this);
        m_ResMgr=new ResourcesManager(this);

        m_UiMgr.Init();
        m_RequestMgr.Init();
        m_PlayerMgr.Init();
        m_CameraMgr.Init();
        m_AudioMgr.Init();
        m_ClientMgr.Init();
        m_ResMgr.Init();
    }

    public void AddResuest(ActionCode actionCode, BaseRequest request)
    {
        if (m_RequestMgr == null)
        {
            return;
        }
        m_RequestMgr.AddResuest(actionCode, request);
    }

    public void RemoveRequest(ActionCode actionCode)
    {
        if (m_RequestMgr==null)
        {
            return;
        }
        m_RequestMgr.RemoveRequest(actionCode);
    }

    public void HandleResponse(ActionCode actionCode, string data)
    {
        if (m_RequestMgr == null)
        {
            return;
        }
        m_RequestMgr.HandleResponse(actionCode, data);
    }
    private void DestroyManager()
    {
        if (m_UiMgr==null|| m_RequestMgr==null|| m_PlayerMgr==null|| m_CameraMgr==null
            || m_AudioMgr==null|| m_ClientMgr==null|| m_ResMgr==null)
        {
            return;
        }
        m_UiMgr.Destroy();
        m_RequestMgr.Destroy();
        m_PlayerMgr.Destroy();
        m_CameraMgr.Destroy();
        m_AudioMgr.Destroy();
        m_ClientMgr.Destroy();
        m_ResMgr.Destroy();
    }

    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        m_ClientMgr.SendRequest(requestCode, actionCode, data);
    }
    public void ShowTipMessageAsync(string data)
    {
        m_UiMgr.ShowTipMessageAsync(data);
    }
    public void ShowMassage(string message)
    {
        m_UiMgr.ShowMassage(message);
    }

    public RoomData GetRoomData()
    {
        return m_PlayerMgr.SetRoomData;
    }

    public void SetRoomInfoData(RoomData rd)
    {
         m_PlayerMgr.SetRoomData = rd;
    }
    public ScoreData GetUserData()
    {
        return m_PlayerMgr.SetUserData;
    }
    public void SetUserInfoData(ScoreData ud)
    {
        m_PlayerMgr.SetUserData = ud; 
    }

    public void SetRoomPlayerListData(RoomAntity room)
    {
        m_PlayerMgr.SetPlayerList = room;
    }

    public RoomAntity GetRoomPlayerList()
    {
        return m_PlayerMgr.SetPlayerList;
    }
    void OnDestroy()
    {
        DestroyManager();
    }
}
