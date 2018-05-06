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
    class ParsePackage
    {
        public int DataBytesMaxLength = 5120;
        private byte[] data;
        public delegate void ReceiveData(byte[] data);
        public event ReceiveData GetReceiveData;
        public delegate void DataPacker(RequestCode rc, ActionCode ac, string data);

        public event DataPacker ParsedDataObjPacker;
        public List<byte> mBufferList;
        public byte[] Data { get { return data; } }
        //public int StartIndex { get { return startIndex; } }
        /// <summary>
        /// 读取的最大字节
        /// </summary>
        //public int RemainSize { get { return data.Length - startIndex; } }
        private MainServer mMainServer;
        public ParsePackage() { }
        public ParsePackage(ClientPeer client, MainServer _server)
        { 
            mBufferList = new List<byte>();
            GetReceiveData += ParseReceiveData;
            _server.GetData += GetReceiveBytesData;
            ParsedDataObjPacker += client.OnProcessMessage;

        }
        private byte[] mDataBytes;
        void GetReceiveBytesData(byte[] data)
        {
            mDataBytes = data;
        }
        /// <summary>
        /// 读取数据
        /// </summary> 
        public void ReadMessage(int newDataAmount) 
        { 
            try
            {
                if (newDataAmount > 0)
                {
                    //mBufferList.Clear();
                    lock (mBufferList)
                    {
                        mBufferList.AddRange(mDataBytes);
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

        private List<byte> ParsedList=new List<byte>();
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        void ParseReceiveData(byte[] data)
        {
            Console.WriteLine("datalength:" + data.Length);
            lock (ParsedList)
            {
                ParsedList.AddRange(data);
            }
            byte[] requestBytes = ParsedList.GetRange(0, 4).ToArray();
            byte[] actionBytes = ParsedList.GetRange(4, 8).ToArray();
            byte[] dataBytes = ParsedList.GetRange(8, data.Length - 8).ToArray();
            RequestCode requestCode = (RequestCode)BitConverter.ToInt32(requestBytes, 0);
            ActionCode actionCode = (ActionCode)BitConverter.ToInt32(actionBytes, 0); 

            string dataRc = Encoding.UTF8.GetString(dataBytes); 
            Console.WriteLine("dataRC:"+dataRc);
            if (ParsedDataObjPacker != null)
            {
                ParsedDataObjPacker(requestCode, actionCode, dataRc);
            }
            lock (ParsedList)
            {
                ParsedList.Clear();
            }
        }
        public static byte[] PackData(ActionCode actionCode, string data)
        {

            //MsgCallBack sendData = new MsgCallBack(actionCode, ProtoBufDataSerialize(data));
           // byte[] dataBytes = ProtoBufDataSerialize(sendData);
            byte[] requestCodeBytes = BitConverter.GetBytes((int)actionCode);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int dataAmount = requestCodeBytes.Length+dataBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
            return dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>().Concat(dataBytes).ToArray<byte>();
        }  
        /// <summary>
        /// 将请求类型和数据组拼到一起
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="data"></param>
        /// <returns></returns>
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
            //using (var ms = new MemoryStream(data))
            //{
            //    return Serializer.Deserialize<T>(ms);
            //}

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
