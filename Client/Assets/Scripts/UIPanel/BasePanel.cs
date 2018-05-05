using UnityEngine;
using System.Collections;

public class BasePanel : MonoBehaviour
{


    protected UIManager m_UIMgr;

    public UIManager UImgr
    {
        set { m_UIMgr = value; }
    }
   // protected PlayerManager
    public virtual void OnInit()
    {
        
    }
    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter(params object[] paramData)
    {

    } 

    /// <summary>
    /// 界面暂停
    /// </summary>
    public virtual void OnPause()
    {

    }

    /// <summary>
    /// 界面继续
    /// </summary>
    public virtual void OnResume()
    {

    }

    /// <summary>
    /// 界面不显示,退出这个界面，界面被关系
    /// </summary>
    public virtual void OnExit()
    {

    }
}
