using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

public class HService : NetBase, IHService
{

    Queue<HttpListenerContext> mClientList = new Queue<HttpListenerContext>();
    private HttpListener mListener;
    private HttpListenerRequest mRequest;
    private List<string> mQueryValue;
    private HttpNetWork mHttp;
    private MethodInfo mMethod;
    private Dictionary<string, string> DataDic;

    List<string> mMsgList
    {
        get
        {
            return new List<string>()
            {
                NotiConst.SEND_LOGIN_FOR_WEB
            };
        }
    }

    public virtual void Execute(INotification message)
    {
        Debug.Log("接收Control消息");

        switch (message.Name)
        {
            case NotiConst.SEND_LOGIN_FOR_WEB:
                SendLogin((HttpCallBackForWeb)message.Body);
                break;
        }


    }

    protected override void OnStart()
    {

        #region  注册命令
        mFacade.RegisterHNCommand(this, mMsgList);
        #endregion
        //获取httpnetwork实例
        if (mHttp == null)
        {
            mHttp = HttpNetWork.Instance;
        }

        ListenWedClient();
    }
    #region  开启监听
    void ListenWedClient()
    {
        //httpListener提供一个简单，可通过编程方式控制的Http协议侦听器。此类不能被继承。
        if (!HttpListener.IsSupported)
        {
            //该类只能在Windows xp sp2或者Windows server 200 以上的操作系统中才能使用，因为这个类必须使用Http.sys系统组件才能完成工作
            //。所以在使用前应该先判断一下是否支持该类
            Debug.Log("Windows xp sp2 or server 2003 is required to use the HttpListener class");
        }
        //设置前缀，必须以‘/’结尾
        string[] prefixes = new string[] { "http://192.168.100.106:8854/" };
        ////初始化监听器
        mListener = new HttpListener();
        ////将前缀添加到监听器
        foreach (var item in prefixes)
        {
            mListener.Prefixes.Add(item);
        }
        ////判断是否已经启动了监听器,如果没有则开启
        if (!mListener.IsListening)
        {
            mListener.Start();
        }
        ////提示
        Debug.Log("服务器已经启动，开始监听....");
        ////等待传入的请求，该方法将阻塞进程，直到收到请求

        mListener.BeginGetContext(new AsyncCallback(ConnectCallBack), mListener);
    }
    #endregion
    /// <summary>
    /// 链接回调
    /// </summary>
    /// <param name="ar"></param>
    void ConnectCallBack(IAsyncResult ar)
    {

        try
        {
            mListener = ar.AsyncState as HttpListener;
            HttpListenerContext context = mListener.EndGetContext(ar);

            ////取得请求的对象
            mRequest = context.Request;
            string[] reCmd = mRequest.Url.AbsolutePath.Split('/');
            Debug.Log("reCmd:" + reCmd[1]);
            mMethod = mHttp.GetType().GetMethod(reCmd[1]);
            if (mMethod == null)
            {
                Debug.LogWarning("没有在HttpNetWork中找到[" + reCmd[1] + "]方法");
            }
            else
            {
                DataDic = new Dictionary<string, string>();
                for (int i = 0; i < mRequest.QueryString.Count; i++)
                {
                    DataDic[mRequest.QueryString.Keys[i]] = mRequest.QueryString[mRequest.QueryString.Keys[i]];
                }


                mClientList.Enqueue(context);
            }
            
            mRequest.QueryString.Clear();
            mListener.BeginGetContext(new AsyncCallback(ConnectCallBack), mListener);

        }
        catch (Exception ex)
        {
            Debug.LogWarning("服务器出错：" + ex);
            CloseHttp();
        }
    }
    #region 向WEB发送数据
    void Send(HttpListenerContext context, string _sendData)
    {     //取得响应对象
        HttpListenerResponse response = context.Response;
        response.AddHeader("Access-Control-Allow-Origin", "*");
        response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
        response.ContentLength64 = System.Text.Encoding.UTF8.GetByteCount(_sendData);
        //设置响应头部内容，长度及编码
        response.ContentType = "text/html; Charset=UTF-8";
        System.IO.Stream output = response.OutputStream;
        //输出响应内容
        System.IO.StreamWriter sw = new System.IO.StreamWriter(output);
        sw.Write(_sendData);
        sw.Dispose();
    }
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="_sendData"></param>
    void SendForWed(string _sendData)
    {
        Debug.Log("要发送的Json数据：" + _sendData);
        if (mClientList.Count > 0)
        {
            Send(mClientList.Dequeue(), _sendData);
        }
    }
    #region   向Web发送数据
    void SendLogin(HttpCallBackForWeb _sendData)
    {
        AccountProxy acc;
        if (_sendData==HttpCallBackForWeb.Success)
        {
            acc = (AccountProxy)mFacade.GetProxy(NotiConst.ACCOUNT_PROXY);
            acc.Account.Username = acc.AccountEn.Username;
            acc.Account.CmdIdx = acc.AccountEn.CmdIdx;
            acc.Account.Code = 0;
            acc.Account.Message =Enum.GetName(typeof(HttpCallBackForWeb), _sendData) ;
        }
        else
        {
            acc = (AccountProxy)mFacade.GetProxy(NotiConst.ACCOUNT_PROXY);
            acc.Account.Username = acc.AccountEn.Username;
            acc.Account.CmdIdx = acc.AccountEn.CmdIdx;
            acc.Account.Code = 1;
            acc.Account.Message = Enum.GetName(typeof(HttpCallBackForWeb), _sendData);
        }

        SendForWed(JsonAnalyseTool.ClassToJson(acc.Account));
    }
    #endregion


    void Update()
    {
        if (mMethod != null)
        {
            if (DataDic != null && DataDic.Count > 0)
            {
                object[] parameters = new object[] { DataDic };
                mMethod.Invoke(mHttp, parameters);
            }
            DataDic = null;
            mMethod = null;
        }
    }


    #endregion


    void OnDestroy()
    {
        CloseHttp();
    }

    void CloseHttp()
    {
        if (mListener != null)
        {
            mListener.Stop();
            mListener.Close();
        }
    }
}
