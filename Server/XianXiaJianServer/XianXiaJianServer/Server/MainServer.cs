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
        /// <summary>
        /// 服务器程序允许的最大客户端连接数
        /// </summary>
        private int mMaxClient;

        /// <summary>
        /// 当前的连接的客户端数
        /// </summary>
        private int mClientCount;
        /// <summary>
        /// 服务器使用的异步socket
        /// </summary>
        private Socket mServerSock;
        private IPEndPoint mIpEndPoint;
        private Socket mServerSocket;
        /// <summary>
        /// 客户端会话列表
        /// </summary>
        public List<ClientPeer> mClientList;
        public Dictionary<string,UserData> mUserDataList=new Dictionary<string, UserData>(); 
        //private List<Room> roomList = new List<Room>();
        // private ControllerManager controllerManager;
        /// <summary>
        /// 服务器是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// 监听的IP地址
        /// </summary>
        public IPAddress Address { get; private set; }
        /// <summary>
        /// 监听的端口
        /// </summary>
        public int Port { get; private set; }
        private ParsePackage mMsg;

        public delegate void StorageGetData(byte[] data);

        public event StorageGetData GetData;

        private ControllerManager controllerManager;

        public List<Room> roomList = new List<Room>();
        public List<RoomAntity> roomDataList;
        //public List<ScoreData> playerDataList = new List<ScoreData>();
        public ScoreData RoomItemData = null;
        // private IPEndPoint ipEndPoint;
        //private Socket serverSocket;
        public MainServer() { }
        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="listenPort">监听的端口</param>
        public MainServer(int listenPort) : this(IPAddress.Any, listenPort, 1024)
        {

        }
        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="localEP">监听的终结点</param>
        public MainServer(IPEndPoint localEP) : this(localEP.Address, localEP.Port, 1024)
        {

        }
        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="localIPAddress">监听的IP地址</param>
        /// <param name="listenPort">监听的端口</param>
        /// <param name="maxClient">最大客户端数量</param>
        public MainServer(IPAddress localIPAddress, int listenPort, int maxClient)
        {
            this.Address = localIPAddress;
            this.Port = listenPort;
            mMaxClient = maxClient;
            mClientList = new List<ClientPeer>();
            mServerSock = new Socket(localIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            roomDataList = new List<RoomAntity>();
        }
        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                mServerSock.Bind(new IPEndPoint(this.Address, this.Port));
                mServerSock.Listen(1024);
                mMaxClient = 1024;
                mServerSock.BeginAccept(new AsyncCallback(AcceptCallBack), mServerSock);
                controllerManager = new ControllerManager(this);
            }
        }
        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="backlog">
        /// 服务器所允许的挂起连接序列的最大长度
        /// </param>
        public void Start(int backlog)
        {
            if (!IsRunning)
            {
                IsRunning = true;
                mServerSock.Bind(new IPEndPoint(this.Address, this.Port));
                mServerSock.Listen(backlog);
                mServerSock.BeginAccept(new AsyncCallback(AcceptCallBack), mServerSock);
                controllerManager = new ControllerManager(this);
            }
        }
        //处理请求
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, ClientPeer client)
        {
            controllerManager.HandleRequest(requestCode, actionCode, data, client);//调用ControllerManager中的处理请求的函数
        }
        private void AcceptCallBack(IAsyncResult ar)
        {

            if (ar.AsyncWaitHandle.WaitOne(5000))
            {
                if (IsRunning)
                {

                    if (mClientCount <= mMaxClient)
                    {
                        Socket server = (Socket)ar.AsyncState;
                        Socket clientSocket = server.EndAccept(ar);
                        ClientPeer client = new ClientPeer(clientSocket, this);

                        lock (mClientList)
                        {
                            mClientList.Add(client);
                            mClientCount++;
                            Console.WriteLine("一个客户端连接进来了");
                            mMsg = new ParsePackage(client, this);

                        }
                        client.RecvDataBuffer = new byte[mMsg.DataBytesMaxLength];
                        //开始接受来自该客户端的数据 
                        clientSocket.BeginReceive(client.RecvDataBuffer, 0, client.RecvDataBuffer.Length, SocketFlags.None,
                         new AsyncCallback(HandleDataReceived), client);
                        mServerSock.BeginAccept(new AsyncCallback(AcceptCallBack), mServerSock);
                    }
                    else
                    {
                        Console.WriteLine("服务器爆满");
                    }

                }
            }
            else
            {
                Console.WriteLine("超时");
            }
        }
        /// <summary>
        /// 处理客户端数据
        /// </summary>
        /// <param name="ar"></param>
        private void HandleDataReceived(IAsyncResult ar)
        {
            if (IsRunning)
            {
                if (!ar.AsyncWaitHandle.WaitOne(5000))
                {
                    Console.WriteLine("超时");
                    return;
                }
                ClientPeer state = (ClientPeer)ar.AsyncState;
                Socket client = state.ClientSocket;

                try
                {

                    int count = client.EndReceive(ar);
                    if (count == 0)
                    {
                        Close(state);
                        return;
                    }
                    if (GetData != null)
                    {
                        GetData(state.RecvDataBuffer);
                    }
                    mMsg.ReadMessage(count);
                    client.BeginReceive(state.RecvDataBuffer, 0, state.RecvDataBuffer.Length, SocketFlags.None,
                     new AsyncCallback(HandleDataReceived), state);
                }
                catch (Exception e)
                {
                    Close(state);
                }
            }
        }
        /// <summary>
        /// 关闭一个与客户端之间的会话
        /// </summary>
        /// <param name="state">需要关闭的客户端会话对象</param>
        public void Close(ClientPeer state)
        {
            if (state != null)
            {
                state.RecvDataBuffer = null;
                lock (mClientList)
                {
                    mClientList.Remove(state);
                }
                lock (mUserDataList)
                {
                    mUserDataList.Remove(state.GetCurAccount.UserName);
                }
                Console.WriteLine("一个客户端断开连接:");
                mClientCount--;
                //触发关闭事件
                state.Close();
            }
        }
        /// <summary>
        /// 关闭所有的客户端会话,与所有的客户端连接会断开
        /// </summary>
        public void CloseAllClient()
        {
            foreach (ClientPeer client in mClientList)
            {
                Close(client);
            }
            mClientCount = 0;
            mClientList.Clear();
        }

        public void Dispose()
        {
            CloseAllClient();
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
            client.SendResponse(actionCode, data);
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

                roomDataList.Remove(room.roomAntity);
                roomList.Remove(room);
            }
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
