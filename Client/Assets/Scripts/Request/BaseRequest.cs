using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRequest : MonoBehaviour
{
    protected RequestCode m_RequestCode = RequestCode.None;//通过Request区别对应的Cotroller
    protected ActionCode m_ActionCode = ActionCode.None;//通过Action区别对应的Request
    protected GameFacade m_Facade;

    // Use this for initialization
    public virtual void Awake()
    {
        if (m_Facade == null)
        {
            m_Facade = GameFacade.Instance;
        }
        
    }
    public virtual void Update() { }
    protected virtual void AddRequestMothed()
    {
        m_Facade.AddResuest(m_ActionCode, this);
    }

    protected void SendRequest(string data)
    {
        Debug.Log("发送的数据："+data);
        m_Facade.SendRequest(m_RequestCode, m_ActionCode, data);
    }
    public virtual void SendRequest() { }
    public virtual void OnResponse(string data) { }   
    /// <summary>
    /// 当游戏对象被销毁时，便移除自身请求
    /// </summary>
    public virtual void OnDestroy()
    {
        GameFacade.Instance.RemoveRequest(m_ActionCode);
    }
}
