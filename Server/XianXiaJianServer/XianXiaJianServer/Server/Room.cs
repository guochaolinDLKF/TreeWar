using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MySql.Data.MySqlClient.Memcached;
using XianXiaJianServer.Model;

namespace XianXiaJianServer.Server
{
    enum RoomState
    {
        WaitingJoin, 
        WaitingBattle,
        Battle,
        End 

    }
    class Room
    {
        private MainServer mServer;
        public Room(MainServer server)
        {
            mServer = server;
        }
        //房间中的玩家列表
        private List<ClientPeer> clientRoom=new List<ClientPeer>();
        private List<ScoreData> playerDataList=new List<ScoreData>();   
        private RoomState state=RoomState.WaitingJoin;//默认为等待加入
        /// <summary>
        /// 房间列表
        /// </summary>
        private List<Room> m_Roomlist;
        /// <summary>
        /// 房间数据列表
        /// </summary>
        //private List<RoomAntity> m_RoomDataList;
        /// <summary>
        /// 单个房间实体
        /// </summary>
        public RoomAntity roomAntity = new RoomAntity();// 
        public bool IsWaitingJoin()
        {
            return state == RoomState.WaitingJoin;
        }
        /// <summary>
        /// 在房间添加玩家
        /// </summary>
        /// <param name="client"></param>
        public void AddClient(ClientPeer client,ScoreData playerData)
        {
            //roomAntity.PlayerListData = playerDataList;
            clientRoom.Add(client);
            playerDataList.Add(playerData);
            client.GetRoom = this;
            roomAntity.PlayerListData = playerDataList;
        }
        public void SetRoomList(List<Room> roomlist)
        {
            m_Roomlist = roomlist;
        }
        public List<Room> GetRoomList()
        {
            return m_Roomlist;
        }
        public bool IsHouseOwner(ClientPeer client)
        {
            return client == clientRoom[0];
        }
        public List<ScoreData> GetRoomPlayerList()
        {
            return playerDataList;
        } 
        public void QuitRoom(ClientPeer client)
        { 
            if (client == clientRoom[0])//如果是房主
            {
                Close();
            }
            else
            {
                clientRoom.Remove(client);
                playerDataList.Remove(client.CurPlayerData);
                RoomAntity sendAntity = new RoomAntity(ReturnCode.Success, client.CurPlayerData.UserName, GetRoomPlayerList());
                client.GetRoom.RemoveClient(client);
                BroadcastMessage(client, ActionCode.UpdateRoomPlayerList, ParsePackage.JSONDataSerialize(sendAntity));
            }
           
        }
        public void Close()
        {
            foreach (ClientPeer client in clientRoom)
            {
                client.GetRoom = null;
            }
            mServer.RemoveRoom(this);
        }
        /// <summary>
        /// 向其他客户端广播数据
        /// </summary>
        /// <param name="curentClient">当前登录此账户的客户端</param>
        /// <param name="actionCode"></param>
        /// <param name="data"></param>
        public void BroadcastMessage(ClientPeer curentClient, ActionCode actionCode, string data)
        {
            foreach (ClientPeer client in clientRoom)
            {
                if (client != curentClient)
                {
                    mServer.SendResponse(client, actionCode, data);
                }
            }
        }
        public void RemoveClient(ClientPeer client)
        {
            client.GetRoom = null;
            clientRoom.Remove(client);
            playerDataList.Remove(client.CurPlayerData);
            if (clientRoom.Count >= 2)
            {
                state = RoomState.WaitingBattle;
            }
            else
            {
                state = RoomState.WaitingJoin;
            }
        }
        /// <summary>
        /// 获取房主ID
        /// </summary>
        /// <returns></returns>
        public string GetId()
        {
            if (playerDataList.Count > 0)
            {
                return playerDataList[0].UserName; 
            }
            return null;
        }

        public void ShowTimer()
        {
            new Thread(RunTimer).Start();
        }

        void RunTimer()
        {
            //MsgCallBack sendMsg;
            for (int i = 5; i >0; i--)
            {
               // sendMsg=new MsgCallBack(ReturnCode.Success,ParsePackage.ProtoBufDataSerialize(i));
                BroadcastMessage(null,ActionCode.ShowTimer, ParsePackage.JSONDataSerialize(i));
                Thread.Sleep(1000);
            }
            //sendMsg = new MsgCallBack(ReturnCode.Success, null);
            BroadcastMessage(null,ActionCode.StartPlay, ParsePackage.JSONDataSerialize("startGame"));
        }
    }
}
