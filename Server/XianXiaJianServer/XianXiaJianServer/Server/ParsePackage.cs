using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using XianXiaJianServer.Model;
using ProtoBuf;
using System.IO;

namespace XianXiaJianServer.Server
{
    public class ParsePackage
    {   //创建字节数组
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
        /// <summary>
        /// 读取数据
        /// </summary>
        public void ReceiveData(int newDataAmount, Action<RequestCode, ActionCode, string> processDataCallback)
        {
            startIndex += newDataAmount;
            while (true)
            {
                if (startIndex <= 4) return;
                int count = BitConverter.ToInt32(data, 0);
                if ((startIndex - 4) >= count)
                {
                    //Console.WriteLine(startIndex);
                    //Console.WriteLine(count);
                    //string s = Encoding.UTF8.GetString(data, 4, count);
                    //Console.WriteLine("解析出来一条数据：" + s);
                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(data, 4);
                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 8);
                    string s = Encoding.UTF8.GetString(data, 12, count - 8);
                    processDataCallback(requestCode, actionCode, s);
                    Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);
                    startIndex -= (count + 4);
                }
                else
                {
                    break;
                }
            }
        }

        //public void ParseData(int newDataAmount, Action<RequestCode, ActionCode, byte[]> processDataCallback)
        //{
        //    GetReceiveData(newDataAmount, out m_ReceiveData);
        //    if (m_ReceiveData != null)
        //    {
        //        MsgCallBack msg = ProtoBufDataDeSerialize<MsgCallBack>(m_ReceiveData);
        //        processDataCallback(msg.RequestCode, msg.ActionCode, msg.MsgDataByte);//将解析出来的数据放进回调中
        //        m_ReceiveData = null;
        //    }

        //}
        /// <summary>
        /// 将请求类型和数据组拼到一起
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] PackData(ActionCode actionCodeData, string data)
        {
            byte[] actionCodeDataBytes = BitConverter.GetBytes((int)actionCodeData);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int dataAmount = actionCodeDataBytes.Length + dataBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
            byte[] newBytes = dataAmountBytes.Concat(actionCodeDataBytes).ToArray<byte>();//Concat(dataBytes);
            return newBytes.Concat(dataBytes).ToArray<byte>();
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
}
