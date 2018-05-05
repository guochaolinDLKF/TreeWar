using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    UIPanel,
    UIItem,
    Sound,
    Prefab,
}
public class ResourcesManager : BaseManager {

    private Dictionary<string, GameObject> m_pDictionary = new Dictionary<string, GameObject>();

    public ResourcesManager(GameFacade facede) : base(facede)
    {
    }

    public GameObject Load(ResourceType type,string name,Transform parentTr = null,Vector3 dir = new Vector3(), bool isCache = true, bool isInstantiate = true)
    {  
        GameObject go = null;
        string path = "";
        switch (type)
        {
            case ResourceType.UIPanel:
                path += "UIPanel/";
                break;
            case ResourceType.UIItem:
                path += "UIItem/";
                break;
            case ResourceType.Prefab:
                path += "Prefab/";
                break;
        }
        path += name;
        if (m_pDictionary.ContainsKey(path))
        {
            go = m_pDictionary[path];
        }
        else
        {
            go = Resources.Load<GameObject>(path);
            if (go == null)
            {
                Debug.LogWarning("资源" + path + "为空");
                return null;
            }
            if (isCache)
            {
                m_pDictionary.Add(path, go);
            }
        }
        if (isInstantiate)
        {
            if (parentTr!=null)
            {
                return Object.Instantiate(go,parentTr.position, Quaternion.LookRotation(dir));
            }
            return Object.Instantiate(go);
        }
        else
        {
            return go;
        }
    }
}
