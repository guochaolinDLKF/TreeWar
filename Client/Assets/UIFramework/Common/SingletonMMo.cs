using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMMo<T> : MonoBehaviour where T : MonoBehaviour
{ 
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));//根据类型实例化
                if (_instance == null)
                {
                    GameObject singleton = new GameObject("(single)" + typeof(T));
                    _instance = singleton.AddComponent<T>();
                    DontDestroyOnLoad(singleton);//防止在场景跳转时删除
                }
            }
            return _instance;
        }
    }
}
