using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    [SerializeField]
    private Text m_PlayerName;
    [SerializeField]
    private Text m_TotalCount;
    [SerializeField]
    private Text m_WinCount;
    private int id;

    public void SetUI(int id, string palyerName, int totalCount, int winCount)
    {
        this.id = id;
        this.m_PlayerName.text = palyerName;
        this.m_TotalCount.text = "总场数："+totalCount.ToString();
        this.m_WinCount.text ="胜场数："+ winCount.ToString();
    }
}
