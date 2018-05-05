using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField]
    private Text m_HouseOwner;
    [SerializeField]
    private Text m_TotalCount;
    [SerializeField]
    private Text m_WinCount;
    [SerializeField]
    private Button m_JionRoomBtn;
    
    private string m_PlayerName; 

    private Action<string> CallBack;
    void Start()
    {
        m_JionRoomBtn.onClick.AddListener(BtnClick);
    }

    void BtnClick()
    {
        if (CallBack != null)
        {
            CallBack(m_PlayerName);
        }
    }
    public void SetUIItem(string ownerName, int totalCount, int winCount, Action<string> jionBtnCallBack)
    {
        m_PlayerName = ownerName;
        m_HouseOwner.text = ownerName;
        m_TotalCount.text = totalCount.ToString();
        m_WinCount.text = winCount.ToString();
        CallBack = jionBtnCallBack;
    }
}
