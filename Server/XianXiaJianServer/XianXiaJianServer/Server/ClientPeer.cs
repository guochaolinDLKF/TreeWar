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
        private MainServer mServer;
        private ParsePackage msg;
        /// <summary>
        /// 接收数据缓冲区
        /// </summary>
        private byte[] _recvBuffer;
        /// <summary>
        /// 接收数据缓冲区 
        /// </summary>
        public byte[] RecvDataBuffer
        {
            get
            {
                return _recvBuffer;
            }
            set
            {
                _recvBuffer = value;
            }
        }
        /// <summary>
        /// 客户端的Socket
        /// </summary>
        private Socket _clientSock;
        /// <summary>
        /// 获得与客户端会话关联的Socket对象
        /// </summary>
        public Socket ClientSocket
        {
            get
            {
                return _clientSock;

            }
        }
        public ClientPeer() { }
        public ClientPeer(Socket Socket, MainServer _server)
        {
            msg = new ParsePackage();
            _clientSock = Socket;
            this.mServer = _server;
            mysqlConn = ConnHelper.Connect();//建立连接
        }
     
        //private MainServer server;//持有对Server的引用
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

       private UserData m_CurAccount;

       public UserData GetCurAccount
       {
            get { return m_CurAccount; }
       }
        /// <summary>
        /// 是否是房主
        /// </summary>
        /// <returns></returns>
        public bool IsHouseOwner()
        {
            return room.IsHouseOwner(this);
        }

       public void SetCurAccountData(UserData curdata)
       {
           m_CurAccount = curdata;
       }
        public void SetCurPlayerData(ScoreData curData)
        {
            m_CurPlayerData = curData;
        }
        public void SendResponse(ActionCode actionCode, string data)
        {
            Send(actionCode, data);
        }
        public void Send(ActionCode actionCode, string data)
        {
            try
            {
                byte[] bytes = ParsePackage.PackData(actionCode, data);
                _clientSock.Send(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine("无法发送消息:" + e);
            }
        }
       
       public void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data)
        {
            Console.WriteLine("得到客户端请求的数据--"+ data);
            this.mServer.HandleRequest(requestCode, actionCode, data, this);
        }
        /// <summary>
        /// 断开连接关闭数据库操作，并从列表中移除自身
        /// </summary>
        public void Close()
        {
            ConnHelper.CloseConnection(mysqlConn);//关闭数据库
            if (_clientSock != null)
            {
                _clientSock.Close();
            }
            if (room != null)
            {
                room.QuitRoom(this);
            }

        }

    }
}
