using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Common;
using LitJson;
using ProtoBuf;

public class ClientManager : BaseManager
{
    private const string IP = "127.0.0.1";
    Socket clientSocket;
    private const int PROT = 5807;
    private ParsePackage m_Massage;
    private static object locker = new object();
    public ClientManager(GameFacade facede) : base(facede)
    {
    }

    public override void Init()
    {

        
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_Massage = new ParsePackage();
        ParsePackage.GetDataCallBack += OnProcessDataCallBack;
        try
        {
            clientSocket.Connect(IP, PROT);
            //new Thread(new ThreadStart(Start));
            Start();
        }
        catch (Exception e)
        {
            m_Facede.ShowTipMessageAsync("服务器断开连接");
            //Debug.LogError();
        }
    }
    /// <summary>
    /// 开始监听
    /// </summary>
    private void Start()
    {
        clientSocket.BeginReceive(m_Massage.Data, m_Massage.StartIndex, m_Massage.RemainSize, SocketFlags.None, ReceiveCallback, null);
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        bool success = ar.AsyncWaitHandle.WaitOne(5000);
        if (!success)
        {
            Debug.LogError("网络超时");
            return;
        }
        try
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            int count = clientSocket.EndReceive(ar);
            m_Massage.GetReceiveData(count);
            Start();
        }
        catch (Exception e)
        {
            Debug.LogError("发生错误" + e);
        }

    }
    void OnProcessDataCallBack(ActionCode actionCode, string data)
    {
        Debug.Log("收到服务器回调解析完毕:" + actionCode);
        m_Facede.HandleResponse(actionCode, data);
    }
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        byte[] bytes = ParsePackage.PackData(requestCode, actionCode, data);//解析数据
        Debug.Log("开始发送:" + actionCode);
        clientSocket.Send(bytes);//发送给服务器
    }



    public override void Destroy()
    {
        try
        {
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法关闭跟服务器端的连接！！" + e);
        }
    }
}
