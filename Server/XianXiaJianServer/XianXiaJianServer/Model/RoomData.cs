﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using ProtoBuf;

namespace XianXiaJianServer.Model
{
    [ProtoContract]
    class RoomData
    {
        public RoomData() { }
        public RoomData(ReturnCode returnCode, List<RoomAntity> roomList)
        { 
            ReturnCode = returnCode;  
            RoomListData = roomList;
        }
        [ProtoMember(1)]
        public ReturnCode ReturnCode { get; set; }
        //房间的列表
        [ProtoMember(2)]
        public List<RoomAntity> RoomListData { get; set; } 
    }
    /// <summary>
    /// 单个房间实体类
    /// </summary>
    [ProtoContract]
    class RoomAntity
    {
        public RoomAntity() { }

        public RoomAntity(ReturnCode returnCode,string username, List<ScoreData> playerListData)
        {
            ReturnCode = returnCode;
            Username = username;
            PlayerListData = playerListData;
        }
        [ProtoMember(1)]
        public ReturnCode ReturnCode { get; set; }
        [ProtoMember(2)]
        public string Username { get; set; }//房主名称
        [ProtoMember(3)]
        public List<ScoreData> PlayerListData { get; set; }//玩家列表
    }
}
