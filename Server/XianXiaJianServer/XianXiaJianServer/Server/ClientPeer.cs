using Common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XianXiaJianServer.Model;
using XianXiaJianServer.Tool;

namespace XianXiaJianServer.Server
{
    class ClientPeer
    {
        public ClientPeer() { }
        private ParsePackage msg = new ParsePackage();
        //开启监听
        public void Start()
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            //参数为（字节数组，开始索引，读取的最大字节，Socket标签，接收回调，数据）
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceviceCallBack, null); //开始接收数据
        }
        private Socket clientSocket;//持有对Socket的引用
        private MainServer server;//持有对Server的引用
        private MySqlConnection mysqlConn;
        private Room room;
        public MySqlConnection MySqlConn 
        {
            get { return mysqlConn; }
        }

        public Room GetRoom
        { 
            set { room = value; }
            get { return room; }
        }
        //public List<ScoreData> playerDataList = new List<ScoreData>();
        private ScoreData m_CurPlayerData;

        public ScoreData CurPlayerData
        {
            get { return m_CurPlayerData;}
        }
        /// <summary>
        /// 是否是房主
        /// </summary>
        /// <returns></returns>
        public bool IsHouseOwner()
        {
            return room.IsHouseOwner(this);
        }
        public void SetCurPlayerData(ScoreData curData)
        {
            m_CurPlayerData = curData;
        }
        public ClientPeer(Socket clientSocket, MainServer server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();//建立连接
        }
        public void Send(ClientPeer client,ActionCode actionCode, string data)
        {
            //byte[] actionCodeDataBytes = BitConverter.GetBytes((int)actionCode);
            //byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            //server.SendMessage(client, actionCodeDataBytes.Concat(dataBytes).ToArray<byte>());
            byte[] bytes = ParsePackage.PackData(actionCode, data);
            Console.WriteLine("给客户端响应的数据长度为：" + bytes.Length);
            clientSocket.Send(bytes);//向客户端发送数据将数组作为参数传入
        }
        void ReceviceCallBack(IAsyncResult ar)
        {
            try
            {
                if (clientSocket == null || clientSocket.Connected == false) return;
                int count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    Console.WriteLine("没有数据");
                    Close();
                }
                Console.WriteLine("数据长度--" + count);
                msg.ReceiveData(count, OnProcessMessage);//读取数据
                Start();//循环接收数据
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                 Close();
            }
        }
        void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data)
        {
            Console.WriteLine("得到客户端请求的数据--"+ data); 
            server.HandleRequest(requestCode, actionCode, data, this);
        }
        /// <summary>
        /// 断开连接关闭数据库操作，并从列表中移除自身
        /// </summary>
        public void Close()
        {
            ConnHelper.CloseConnection(mysqlConn);//关闭数据库
            if (clientSocket != null)
            {
                clientSocket.Close();
            }
            if (room != null)
            {
                room.QuitRoom(this);
            }
            server.RemoveClient(this);//移除自身

        }

    }
}
