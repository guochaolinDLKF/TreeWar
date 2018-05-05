using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using ProtoBuf;
using UnityEngine;

public class ParsePackage
{
    //创建字节数组
    private byte[] data = new byte[1024];
    //开始字节
    private int startIndex = 0;
    //长度递增
    public void AddCount(int count)
    {
        startIndex += count;
    }
    private byte[] m_ReceiveData = null;
    public byte[] Data { get { return data; } }
    public int StartIndex { get { return startIndex; } }
    /// <summary>
    /// 读取的最大字节
    /// </summary>
    public int RemainSize { get { return data.Length - startIndex; } }

    public delegate void GetReceiveDataCallBack(ActionCode action, string data);

    public static event GetReceiveDataCallBack GetDataCallBack;
    private ActionCode actionCode;
    /// <summary>
    /// 读取数据
    /// </summary>
    public void GetReceiveData(int newDataAmount)
    {
        startIndex += newDataAmount;
        while (true)
        {
            if (startIndex <= 4)
            {
                break;
            }
            int count = BitConverter.ToInt32(data, 0);
            if ((startIndex - 4) >= count)
            {
                m_ReceiveData = new byte[count];
                //Console.WriteLine(startIndex);
                //Console.WriteLine(count);
                //string s = Encoding.UTF8.GetString(data, 4, count);
                //Console.WriteLine("解析出来一条数据：" + s);
                actionCode = (ActionCode)BitConverter.ToInt32(data, 4);
                string s = Encoding.UTF8.GetString(data, 8, count - 4);
                if (GetDataCallBack != null)
                {
                    GetDataCallBack(actionCode, s);
                }

                //Array.Copy(data, 8, m_ReceiveData, 0, 0);
                Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);
                startIndex -= (count + 4);

            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 将请求类型和数据组拼到一起
    /// </summary>
    /// <param name="requestData"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] PackData(RequestCode requestData, ActionCode actionCode, string data)
    {
        //MsgCallBack Send = new MsgCallBack(requestData, actionCode, data);
        //byte[] dataS = ProtoBufDataSerialize(Send);
        //return dataS;
        //byte[] dataBytes = ProtoBufDataSerialize(Send);  

        byte[] actionCodeBytes = BitConverter.GetBytes((int)actionCode);
        byte[] requestCodeBytes = BitConverter.GetBytes((int)requestData);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        int dataAmount = requestCodeBytes.Length + actionCodeBytes.Length + dataBytes.Length;//数据长度
        byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);

        return dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>().Concat(actionCodeBytes).ToArray<byte>()
            .Concat(dataBytes).ToArray<byte>();
    }
    public static byte[] ProtoBufDataSerialize<T>(T data)
    {
        byte[] bytes = null;
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize<T>(ms, data);
            bytes = new byte[ms.Position];
            var fullBytes = ms.GetBuffer();
            Array.Copy(fullBytes, bytes, bytes.Length);
        }
        return bytes;
    }
    public static T ProtoBufDataDeSerialize<T>(byte[] data)
    {
        using (var ms = new MemoryStream(data))
        {
            return Serializer.Deserialize<T>(ms);
        }

    }
    public static string JSONDataSerialize(object data)
    {
        return JsonMapper.ToJson(data);
    }
    public static T JSONDataDeSerialize<T>(string data)
    {
        return JsonMapper.ToObject<T>(data);
    }
}
