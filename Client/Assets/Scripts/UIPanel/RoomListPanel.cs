using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{

    private Transform m_RoomList;
    private Transform m_OwnerInfo;
    private Tweener m_RoomListTw;
    private Tweener m_OwnerInfoTw;
    private Button m_CreatRoomBtn;
    private Button m_RefreshBtn;
    private Button m_CloseBtn;
    private Text m_OwnerName;
    private Text m_TotalCount;
    private Text m_WinCount;
    [SerializeField]
    private CanvasGroup m_Canvas;
    [SerializeField]
    private GridLayoutGroup m_Content;
    [SerializeField]
    private Text m_NullRoomTxt;
    private List<RoomItem> m_RoomitemList;
    private bool IsNeedRefresh = false;
    private bool IsPause = false;
    public override void OnInit()
    {


        m_RoomList = transform.Find("RoomList").transform;
        m_OwnerInfo = transform.Find("OwnerInfo").transform;
        m_CreatRoomBtn = transform.Find("RoomList/CreatRoomBtn").GetComponent<Button>();
        m_RefreshBtn = transform.Find("RoomList/RefreshBtn").GetComponent<Button>();
        m_CloseBtn = transform.Find("RoomList/CloseBtn").GetComponent<Button>();

        m_OwnerName = transform.Find("OwnerInfo/OwnerName").GetComponent<Text>();
        m_TotalCount = transform.Find("OwnerInfo/TotalCount").GetComponent<Text>();
        m_WinCount = transform.Find("OwnerInfo/WinCount").GetComponent<Text>();

        m_CloseBtn.onClick.AddListener(CloseBtnClick);
        m_CreatRoomBtn.onClick.AddListener(CreatRoomBtn);
        m_RefreshBtn.onClick.AddListener(Refresh);
        m_RoomListTw = m_RoomList.DOLocalMoveX(86, 0.5f);
        m_OwnerInfoTw = m_OwnerInfo.DOLocalMoveX(-269, 0.5f);
        m_RoomListTw.SetAutoKill(false);
        m_RoomListTw.Pause();
        m_OwnerInfoTw.SetAutoKill(false);
        m_OwnerInfoTw.Pause();
        m_RoomitemList = new List<RoomItem>();
        IsNeedRefresh = true;
        
    }

    void RefreshRoomList()
    {
        GameObject go;
        if (GameFacade.Instance.GetRoomData().RoomListData.Count <= 0)
        {
            m_NullRoomTxt.gameObject.SetActive(true);
        }
        else
        {
            m_NullRoomTxt.gameObject.SetActive(false);
        }
        ClearRoomList();
        for (int i = 0; i < GameFacade.Instance.GetRoomData().RoomListData.Count; i++)
        {
            go = GameFacade.Instance.m_ResMgr.Load(ResourceType.UIItem, "RoomItem");
            go.transform.SetParent(m_Content.transform, false);
            go.GetComponent<RoomItem>().SetUIItem(
                GameFacade.Instance.GetRoomData().RoomListData[i].PlayerListData[0].UserName,
                GameFacade.Instance.GetRoomData().RoomListData[i].PlayerListData[0].TotalCount,
                GameFacade.Instance.GetRoomData().RoomListData[i].PlayerListData[0].WinCount, JionRoomClickCallBack);
            m_RoomitemList.Add(go.GetComponent<RoomItem>());
        }
    }

    void Update()
    {
        if (IsNeedRefresh)
        {
            //SendGetRoomListRequest();
            Debug.Log("刷新房间列表");
            RefreshRoomList();
            IsNeedRefresh = false;
        }
        if (IsPause)
        {
            m_Canvas.interactable = false;
        }
        else
        {
            m_Canvas.interactable = true;
        }
    }
    private void ClearRoomList()
    {
        for (int i = 0; i < m_RoomitemList.Count; i++)
        {
            Destroy(m_RoomitemList[i].gameObject);
        }
        if (m_RoomitemList.Count > 0)
        {
            m_RoomitemList.Clear();
        }

    }
    void CloseBtnClick()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        EventDispatcher.Instance.TriggerEvent(GameConst.QuitLoginRequest,GameFacade.Instance.GetUserData().UserName);
      
    }

    void RtrurnLoginPanelCallBack()
    {
        m_UIMgr.PopPanel();
        m_UIMgr.SyncPushPanel(UIPanelType.Start);
    }
    void Refresh()
    {
        IsNeedRefresh = true;
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        SendGetRoomListRequest();
    }
    void SendGetRoomListRequest()
    {
        Debug.Log("继续进行房间列表");
        EventDispatcher.Instance.TriggerEvent(GameConst.SendGetRoomListRequest);
    }
    public void GetRoomListSuccessCallBack()
    {
        Debug.Log("获取房间列表回调");
        IsNeedRefresh = true;
    }
    void CreatRoomBtn()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        SendCreatRoomListRequest();

    }
    void SendCreatRoomListRequest()
    {
        EventDispatcher.Instance.TriggerEvent(GameConst.SendCreatRoomRequest);
        // m_RoomRequest.SendCreatRoomRequest();
    }
    public void CreatRoomListSuccessCallBack()
    {
        IsNeedRefresh = true;
        m_UIMgr.SyncPushPanel(UIPanelType.Room);
    }
    public override void OnEnter(params object[] paramData)
    {
        EventDispatcher.Instance.AddEventListener(GameConst.StartGameCallBack, StartGameCallBack);
        EventDispatcher.Instance.AddEventListener(GameConst.GetRoomListSuccessCallBack, GetRoomListSuccessCallBack);
        EventDispatcher.Instance.AddEventListener(GameConst.CreatRoomListSuccessCallBack, CreatRoomListSuccessCallBack);
        EventDispatcher.Instance.AddEventListener(GameConst.JoinRoomNetCallBack, JoinRoomNetCallBack);
        EventDispatcher.Instance.AddEventListener(GameConst.QuitLoginCallBack, RtrurnLoginPanelCallBack);
        m_RoomListTw.PlayForward();
        m_OwnerInfoTw.PlayForward();
        IsNeedRefresh = true;
        SetOwnerInfo();
    }

    void SetOwnerInfo()
    {
        m_OwnerName.text = GameFacade.Instance.GetUserData().UserName;
        m_TotalCount.text = "总场数：" + GameFacade.Instance.GetUserData().TotalCount.ToString();
        m_WinCount.text = "胜场数：" + GameFacade.Instance.GetUserData().WinCount.ToString();
    }
    public override void OnPause()
    {
        IsPause = true;
    }

    public override void OnResume()
    {
        SendGetRoomListRequest();
        //IsNeedRefresh = true;
        IsPause = false;


        //m_Canvas.interactable = true;
    }
    void StartGameCallBack()
    {
        Debug.Log("开始倒计时");
        m_UIMgr.PopPanel();
        m_UIMgr.SyncPushPanel(UIPanelType.Game);
    }
    void JionRoomClickCallBack(string playername)
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        SendJoinRoomRequest(playername);


        Debug.Log("加入了房间：" + playername);
    }

    void SendJoinRoomRequest(string playername)
    {
        EventDispatcher.Instance.TriggerEvent(GameConst.SendJoinRoomRequest, playername);
    }

    public void JoinRoomNetCallBack()
    {
        m_UIMgr.SyncPushPanel(UIPanelType.Room);
    }
    public override void OnExit()
    {
        m_RoomListTw.PlayBackwards();
        m_OwnerInfoTw.PlayBackwards();
        EventDispatcher.Instance.RemoveEventListener(GameConst.QuitLoginCallBack, RtrurnLoginPanelCallBack); 
        EventDispatcher.Instance.RemoveEventListener(GameConst.StartGameCallBack, StartGameCallBack);
        EventDispatcher.Instance.RemoveEventListener(GameConst.GetRoomListSuccessCallBack, GetRoomListSuccessCallBack);
        EventDispatcher.Instance.RemoveEventListener(GameConst.CreatRoomListSuccessCallBack, CreatRoomListSuccessCallBack);
        EventDispatcher.Instance.RemoveEventListener(GameConst.JoinRoomNetCallBack, JoinRoomNetCallBack);
    }
}
