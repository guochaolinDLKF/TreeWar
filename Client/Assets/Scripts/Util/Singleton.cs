using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : class, new()
{
    private static object _syncobj = new object();
    private static volatile T _instance = default(T);

    public static T Instance
    {
        get
        {
            if ((object)_instance == null)
            {
                lock (_syncobj)
                {
                    if ((object)_instance == null)
                        _instance = Activator.CreateInstance<T>();
                }
            }
            return _instance;
        }
        set
        {
            _instance = default(T);
        }
    }
}
