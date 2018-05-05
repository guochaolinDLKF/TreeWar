using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BaseManager
{
    private ScoreData m_UserData;
    private RoomData m_RoomData;
    private RoomAntity m_RoomAntity;
    private GameFacade m_Facade;
    public PlayerManager(GameFacade facede) : base(facede)
    {
        m_Facade = facede;
    }

    public RoomData SetRoomData
    {
        set { m_RoomData = value; }
        get { return m_RoomData; } 
    }
    public ScoreData SetUserData
    {
        set { m_UserData = value; }
        get { return m_UserData;}
    }

    public RoomAntity SetPlayerList
    {
        get { return m_RoomAntity;}
        set { m_RoomAntity = value; }
    }
    public void Shoot(GameObject arrowPrefab, Vector3 pos, Quaternion rotation)
    {
    }
}
