﻿using System;
using System.Collections.Generic;


namespace Common
{
    public enum ActionCode
    {
        None,//默认
        HeartBeat,
        Login,
        Regiest,
        GetRoomList, 
        CreatRoom,
        JionRoom, 
        UpdateRoomPlayerList,
        QuitRoom,
        StartGame,
        ShowTimer,
        StartPlay,
    }
}
