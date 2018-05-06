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
    private byte[] data = new byte[5120];
    private int startIndex = 0;//我们存取了多少个字节的数据在数组里面

    public delegate void ReceiveData(byte[] data);

    public event ReceiveData GetReceiveData;

    public delegate void DataPacker(ActionCode ac, string data);

    public event DataPacker ParsedDataPacker;
    public List<byte> mBufferList;
    public byte[] Data
    {
        get { return data; }
    }
    public int StartIndex
    {
        get { return startIndex; }
    }
    public int RemainSize
    {
        get { return data.Length - startIndex; }
    }

    public ParsePackage()
    { 
        GetReceiveData += ParseReceiveData;

    }
    /// <summary>
    /// 读取数据
    /// </summary>
    public void ReadMessage(int newDataAmount, bool connected)
    {
        try
        {
            if (newDataAmount > 0 && connected)
            {
                //mBufferList.Clear();
                lock (mBufferList)
                {
                    mBufferList.AddRange(data);
                }
                while (mBufferList.Count > 4)
                {
                    byte[] lenBytes = mBufferList.GetRange(0, 4).ToArray();
                    int packageLen = BitConverter.ToInt32(lenBytes, 0);
                    if (packageLen <= mBufferList.Count - 4)
                    {
                        //包够长时,则提取出来,交给后面的程序去处理  
                        byte[] rev = mBufferList.GetRange(4, packageLen).ToArray();
                        if (GetReceiveData != null)
                        {
                            GetReceiveData(rev);
                        }
                        //从数据池中移除这组数据,为什么要lock,你懂的  
                        lock (mBufferList)
                        {
                            mBufferList.RemoveRange(0, packageLen + 4);
                            mBufferList.Clear();
                        }
                    }
                    else
                    {
                        //长度不够,还得继续接收,需要跳出循环  
                        break;
                    }
                }
            }

        }
        catch (Exception xe)
        {
            Console.WriteLine(xe);
        }
    }

    private List<byte> ParsedDataList = new List<byte>();
    /// <summary>
    /// 解析数据
    /// </summary>
    /// <param name="data"></param>
    void ParseReceiveData(byte[] data)
    {
        lock (ParsedDataList)
        {
            ParsedDataList.AddRange(data);
        }
        byte[] actionBytes = ParsedDataList.GetRange(0, 4).ToArray();
        byte[] dataBytes = ParsedDataList.GetRange(4, data.Length - 4).ToArray();
        ActionCode actionCode = (ActionCode)BitConverter.ToInt32(actionBytes, 0);

        string dataRc = Encoding.UTF8.GetString(dataBytes);
        // MsgCallBack receiveData = ProtoBufDataDeSerialize<MsgCallBack>(data);
        if (ParsedDataPacker != null)
        {
            ParsedDataPacker(actionCode, dataRc);
        }
        lock (ParsedDataList)
        {
            ParsedDataList.Clear();
        }


    }


    public static byte[] PackData(RequestCode requestData, ActionCode actionCode, string data)
    {
        Debug.Log("data:"+data);
        //MsgCallBack msg = new MsgCallBack(requestData, actionCode, data);
        byte[] requestCodeBytes = BitConverter.GetBytes((int)requestData);
        byte[] actionCodeBytes = BitConverter.GetBytes((int)actionCode); 
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        int dataAmount = requestCodeBytes.Length+ actionCodeBytes.Length+ dataBytes.Length;
        byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount); 
        return dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>().Concat(actionCodeBytes).ToArray<byte>().Concat(dataBytes).ToArray<byte>();
    }

    public static byte[] ProtoBufDataSerialize<T>(T data)
    {
        try
        {
            //涉及格式转换，需要用到流，将二进制序列化到流中
            using (MemoryStream ms = new MemoryStream())
            {
                //使用ProtoBuf工具的序列化方法
                ProtoBuf.Serializer.Serialize<T>(ms, data);
                //定义二级制数组，保存序列化后的结果
                byte[] result = new byte[ms.Length];
                //将流的位置设为0，起始点
                ms.Position = 0;
                //将流中的内容读取到二进制数组中
                ms.Read(result, 0, result.Length);
                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("序列化失败: " + ex.ToString());
            return null;
        }
        //byte[] bytes = null;
        //using (var ms = new MemoryStream())
        //{
        //    Serializer.Serialize<T>(ms, data);
        //    bytes = new byte[ms.Position];
        //    var fullBytes = ms.GetBuffer();
        //    Array.Copy(fullBytes, bytes, bytes.Length);
        //}
        //return bytes;
    }
    public static T ProtoBufDataDeSerialize<T>(byte[] data)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //将消息写入流中
                ms.Write(data, 0, data.Length);
                //将流的位置归0
                ms.Position = 0;
                //使用工具反序列化对象
                T result = ProtoBuf.Serializer.Deserialize<T>(ms);
                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("反序列化失败: " + ex.ToString());
            return default(T);
        }
    }
    //using (var ms = new MemoryStream(data))
    //{
    //    return Serializer.Deserialize<T>(ms);
    //}
    public static string JSONDataSerialize(object data)
    {
        return JsonMapper.ToJson(data);
    }
    public static T JSONDataDeSerialize<T>(string data)
    {
        return JsonMapper.ToObject<T>(data);
    }
}


