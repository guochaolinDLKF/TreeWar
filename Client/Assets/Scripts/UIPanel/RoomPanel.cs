using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    [SerializeField]
    private Transform m_Content;
    [SerializeField]
    private Button m_ReadyBtn;
    [SerializeField]
    private Button m_PlayGameBtn;
    [SerializeField]
    private Button m_QuitRoomBtn;
   // [SerializeField]
    //private Text m_ReadyTxt;
    [SerializeField]
    private Text m_WaitJionTxt;
    private Tweener tw;
    private List<PlayerItem> m_PlayeritemList;
    private bool isUpdate = false;
    private bool isClearPlayerList = false;
    public override void OnInit()
    {
       


        m_ReadyBtn.onClick.AddListener(ReadyBtnClickCallBack);
        m_PlayGameBtn.onClick.AddListener(PlayerGameBtnClick);
        m_QuitRoomBtn.onClick.AddListener(QuitRoomBtnClick);
        tw = transform.DOLocalMoveX(0, 0.5f);
        tw.SetAutoKill(false);
        tw.SetEase(Ease.Linear);
        tw.Pause();
        m_PlayeritemList=new List<PlayerItem>();
        
    }
    /// <summary>
    /// 刷新当前房间中的玩家列表
    /// </summary>
    void RefreshPlayerList()
    {
        GameObject go;
        ClearPlayerList();
        //Debug.Log("玩家列表长度："+ GameFacade.Instance.GetRoomPlayerList().PlayerListData.Count);
        for (int i = 0; i < GameFacade.Instance.GetRoomPlayerList().PlayerListData.Count; i++)
        {
            go = GameFacade.Instance.m_ResMgr.Load(ResourceType.UIItem, "PlayerItem");
            go.transform.SetParent(m_Content.transform, false);
            go.GetComponent<PlayerItem>().SetUI(GameFacade.Instance.GetRoomPlayerList().PlayerListData[i].Id,
                GameFacade.Instance.GetRoomPlayerList().PlayerListData[i].UserName,
                GameFacade.Instance.GetRoomPlayerList().PlayerListData[i].TotalCount
           , GameFacade.Instance.GetRoomPlayerList().PlayerListData[i].WinCount);
            m_PlayeritemList.Add(go.GetComponent<PlayerItem>());
        }
    }

    void Update()
    {
        if (isUpdate)
        {
            RefreshPlayerList();
            UpdatePlayerListPanelButton();
            isUpdate = false;
        }
        if (isClearPlayerList)
        {
            ClearPlayerList();
            isClearPlayerList = false;
        }
       
    }
    void ClearPlayerList()
    {
        for (int i = 0; i < m_PlayeritemList.Count; i++)
        {
            Destroy(m_PlayeritemList[i].gameObject);
        }
        if (m_PlayeritemList.Count > 0)
        {
            m_PlayeritemList.Clear();
        }
    }

    void UpdatePlayerListPanelButton()
    {
        if (GameFacade.Instance.GetUserData().UserName == GameFacade.Instance.GetRoomPlayerList().Username
          && GameFacade.Instance.GetRoomPlayerList().PlayerListData.Count > 1) //自己是房主
        {
            m_PlayGameBtn.gameObject.SetActive(true);
        }
        else
        {
            m_PlayGameBtn.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 更新房间中的玩家列表
    /// </summary>
    public void UpdateRoomPlayerNetCallBack()
    {
        isUpdate = true;
    }
    void QuitRoomBtnClick()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        EventDispatcher.Instance.TriggerEvent(GameConst.QuitRoomRequest);
       
    }

    public void QuitRoomCallBack()
    {
        m_UIMgr.PopPanel();
        isClearPlayerList = true;
    }
    void PlayerGameBtnClick()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
        EventDispatcher.Instance.TriggerEvent(GameConst.StartGameRequest);
    }

    void StartGameCallBack()
    {
        m_UIMgr.PopPanel();
    }
    void ReadyBtnClickCallBack()
    {
        GameFacade.Instance.m_AudioMgr.PlayNormalAudio(AudioKind.ButtonClick);
    }
    public override void OnEnter(params object[] paramData)
    {
       EventDispatcher.Instance.AddEventListener(GameConst.StartGameCallBack, StartGameCallBack);
        EventDispatcher.Instance.AddEventListener(GameConst.UpdateRoomPlayerList, UpdateRoomPlayerNetCallBack);
        EventDispatcher.Instance.AddEventListener(GameConst.QuitRoomCallBack, QuitRoomCallBack);
        isUpdate = true;
        tw.PlayForward();
    }

    public override void OnExit()
    {
        tw.PlayBackwards();
        EventDispatcher.Instance.RemoveEventListener(GameConst.StartGameCallBack, StartGameCallBack);
        EventDispatcher.Instance.RemoveEventListener(GameConst.UpdateRoomPlayerList, UpdateRoomPlayerNetCallBack);
        EventDispatcher.Instance.RemoveEventListener(GameConst.QuitRoomCallBack, QuitRoomCallBack); 
    }
}
