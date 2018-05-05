using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using MySql.Data.MySqlClient.Memcached;
using XianXiaJianServer.DAO;
using XianXiaJianServer.Model;
using XianXiaJianServer.Server;

namespace XianXiaJianServer.Controller
{
    class RoomController : BaseController
    {
        private ScoreDAO scoreDAO = new ScoreDAO();
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }

        private string mRoomTag;
        public string GetRoomList(string data, ClientPeer client, MainServer server)
        { 
            Console.WriteLine("获取房间：" + data.Length);
            
            RoomData sendData = new RoomData(ReturnCode.Success, server.roomDataList);
            Console.WriteLine("给客户端返回房间数据:"+ server.roomDataList.Count);
            return ParsePackage.JSONDataSerialize(sendData);

        }
        public string CreatRoom(string data, ClientPeer client, MainServer server)
        {
            Console.WriteLine("创建房间：" + data);
            server.CreatRoom(client);
            RoomData sendData = new RoomData(ReturnCode.Success, server.roomDataList);

            return ParsePackage.JSONDataSerialize(sendData);
        }

        public string JionRoom(string data, ClientPeer client, MainServer server) 
        {
            Console.WriteLine("加入房间：" + data);
            ScoreData roomdata = ParsePackage.JSONDataDeSerialize<ScoreData>(data);
            Room room = server.GetRoomById(roomdata.UserName);
            RoomAntity roomSend;
            if (room == null)
            {
                roomSend=new RoomAntity(ReturnCode.None, null,null);
                return ParsePackage.JSONDataSerialize(roomSend);
            }
            else if (room.IsWaitingJoin() == false)
            {
                roomSend = new RoomAntity(ReturnCode.Fail, null, null);
                return ParsePackage.JSONDataSerialize(roomSend);
            }
            else
            {
                room.AddClient(client, client.CurPlayerData);
                //发送给客户端要加入得房间的房主名称和房间里的玩家列表
                roomSend = new RoomAntity(ReturnCode.Success, roomdata.UserName, room.GetRoomPlayerList());
                //广播给其他客户端
                room.BroadcastMessage(client, ActionCode.UpdateRoomPlayerList, ParsePackage.JSONDataSerialize(roomSend));
                //返回给当前账户
                return ParsePackage.JSONDataSerialize(roomSend);
            }
        }
        public string QuitRoom(string data, ClientPeer client, MainServer server)
        {
            //RoomAntity reciveData = ParsePackage.JSONDataDeSerialize<RoomAntity>(data);
            bool isHouseOwner = client.IsHouseOwner();
            Room room = client.GetRoom;
        
            if (isHouseOwner)
            {
                Console.WriteLine("如果是房主===");
                room.Close();
                RoomAntity sendAntity = new RoomAntity(ReturnCode.Success, "", room.GetRoomPlayerList());
                
                room.BroadcastMessage(client, ActionCode.QuitRoom, ParsePackage.JSONDataSerialize(sendAntity));
                
                return ParsePackage.JSONDataSerialize(sendAntity);
            }
            else
            {
                RoomAntity sendAntity=new RoomAntity(ReturnCode.Success, "", room.GetRoomPlayerList());
                client.GetRoom.RemoveClient(client);
                room.BroadcastMessage(client, ActionCode.UpdateRoomPlayerList, ParsePackage.JSONDataSerialize(sendAntity));//其他客户端刷新
                return ParsePackage.JSONDataSerialize(sendAntity);//自己刷新
            }
        }
    }
}
