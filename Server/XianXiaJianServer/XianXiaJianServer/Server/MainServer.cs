using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ProtoBuf;
using XianXiaJianServer.Controller;
using XianXiaJianServer.Model;

namespace XianXiaJianServer.Server
{
    class MainServer
    {
        private ControllerManager controllerManager;
        public List<ClientPeer> clientList = new List<ClientPeer>();//创建List集合中来管理Client类
        public List<Room> roomList = new List<Room>();
        public List<RoomAntity> roomDataList = new List<RoomAntity>(); 
        //public List<ScoreData> playerDataList = new List<ScoreData>();
        public ScoreData RoomItemData = null; 
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        public MainServer() { }

        public MainServer(string ip, int port)
        {
            controllerManager = new ControllerManager(this);
            SetIpAndPort(ip, port);
        }

        public void SetIpAndPort(string ip, int port)//绑定ip和端口号
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public void Start()//初始化，新建Socket并进行监听
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);
            //开始接受客户端连接
            serverSocket.BeginAccept(AcceptCallBack, null);//先将数据设为null

        }
        //处理请求
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, ClientPeer client)
        {
            controllerManager.HandleRequest(requestCode, actionCode, data, client);//调用ControllerManager中的处理请求的函数
        }
        void AcceptCallBack(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            ClientPeer client = new ClientPeer(clientSocket, this);
            client.Start();//开启监听
            clientList.Add(client);
            serverSocket.BeginAccept(AcceptCallBack, null);//先将数据设为null
        }
        /// <summary>
        /// 向客户端发起相应
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestCode"></param>
        /// <param name="data"></param>
        public void SendResponse(ClientPeer client, ActionCode actionCode, string data)
        {
            //响应客户端
            client.Send(client, actionCode, data);
        }
        /// <summary>
        /// 当一个客户端断开连接后就从Client列表中移除断开的Client
        /// </summary>
        /// <param name="client"></param>
        public void RemoveClient(ClientPeer client)
        {
            lock (clientList)//将列表锁定后再移除，防止发生异常
            {
                clientList.Remove(client);
            }
        }
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="client"></param>
        public void CreatRoom(ClientPeer client)
        {
            Room room = new Room(this);
            room.AddClient(client, client.CurPlayerData);//在房间中添加房主
            roomList.Add(room);//添加房间
            room.roomAntity.Username = client.CurPlayerData.UserName;//房间ID就是房主username
            roomDataList.Add(room.roomAntity); //添加房间
        }
        /// <summary>
        /// 移除房间
        /// </summary>
        /// <param name="room"></param>
        /// <param name="antity"></param>
        public void RemoveRoom(Room room)
        {
           
            if (roomList != null && room != null && roomDataList != null)
            {
                Console.WriteLine("移除前房间列表数据--：" + roomDataList.Count);
                Console.WriteLine("移除前房间--：" + room.roomAntity);

                roomDataList.Remove(room.roomAntity);
                roomList.Remove(room);
            }
            Console.WriteLine("房间列表--：" + roomList.Count); 
            Console.WriteLine("房间列表数据--：" + roomDataList.Count); 
        }
        /// <summary>
        /// 通过房主得到要加入的房间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Room GetRoomById(string id)
        {
            foreach (Room room in roomList)
            {
                if (room.GetId() == id) return room;
            }
            return null;
        }

    }
}
