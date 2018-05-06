using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;
using Common;
using LitJson;
using ProtoBuf;
using UnityEditor.VersionControl;

public class ClientManager : BaseManager
{
    private const string IP = "127.0.0.1";
    // Socket clientSocket;
    private const int PORT = 5807;
    private ParsePackage m_Massage;
    private static object locker = new object();
    public Socket clientSocket;
    private IPEndPoint ipEndP;
    private bool connected;
    public bool Connected
    {
        get
        {
            return clientSocket != null && clientSocket.Connected;
        }
        set { connected = value; }
    }
    public ClientManager(GameFacade facede) : base(facede)
    {
    }
    public override void Init()
    {
        m_Massage = new ParsePackage();
        ipEndP = new IPEndPoint(IPAddress.Parse(IP), PORT);
        m_Massage.ParsedDataPacker += OnProcessDataCallBack;
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_Massage.mBufferList = new List<byte>();
        clientSocket.BeginConnect(ipEndP, (delegate (IAsyncResult ar)
        {
            try
            {
                if (ar.AsyncWaitHandle.WaitOne(5000))
                {
                    clientSocket.EndConnect(ar);
                    Thread th = new Thread(new ThreadStart(StartReceive));
                    th.IsBackground = true;
                    th.Start();
                    //StartReceive();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("无法连接到服务器端，请检查您的网络！！" + e.Message);
            }

        }), clientSocket);
    }
    /// <summary>
    /// 开始监听
    /// </summary>
    private void StartReceive()
    {
        clientSocket.BeginReceive(m_Massage.Data, m_Massage.StartIndex, m_Massage.RemainSize, SocketFlags.None, ReceiveCallback, null);
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            if (ar.AsyncWaitHandle.WaitOne(5000))
            {
                int count = clientSocket.EndReceive(ar);
                m_Massage.ReadMessage(count, Connected);
               // StartReceive();
                Thread th = new Thread(new ThreadStart(StartReceive));
                th.IsBackground = true;
                th.Start();
            }

        }
        catch (Exception e)
        {
            Debug.LogError("服务器断开连接，请检查您的网络！！" + e.Message);
        }

    }
    public void OnProcessDataCallBack(ActionCode actionCode, string data)
    {
        Debug.Log("收到服务器回调解析完毕:" + actionCode);
        m_Facede.HandleResponse(actionCode, data);
    }
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        byte[] bytes = ParsePackage.PackData(requestCode, actionCode, data);//解析数据
        Debug.Log("开始发送actionCode:" + actionCode + "；data:" + data);
        clientSocket.Send(bytes);//发送给服务器
    }


    public override void Destroy()
    {
        try
        {
            m_Massage.ParsedDataPacker -= OnProcessDataCallBack;
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("无法关闭与服务器连接，请检查您的网络！！" + e.Message);
        }
    }
}
