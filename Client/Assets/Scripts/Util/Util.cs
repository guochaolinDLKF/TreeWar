using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util  {

    public static string GuidTo16String()
    {
        long num1 = 1;
        foreach (int num2 in Guid.NewGuid().ToByteArray())
            num1 *= (long)(num2 + 1);
        return string.Format("{0:x}", (object)(num1 - DateTime.Now.Ticks));
    }

    public static void AddScriptsComponent<T>(T data) where T : Component
    {
        GameObject go = GameObject.Find("RequestController");
        if (go == null)
        {
            go = new GameObject("RequestController");
            go.AddComponent<T>();

        }
        else
        {
            go.AddComponent<T>();
        }
    }
}
