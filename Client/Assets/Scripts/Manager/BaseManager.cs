using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager
{
    protected GameFacade m_Facede;

    public BaseManager(GameFacade facede)
    {
        this.m_Facede = facede;
    }
    public virtual void Init() { }
    public virtual void Update() { }
    public virtual void Destroy() { }

}
