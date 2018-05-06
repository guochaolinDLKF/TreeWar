using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using ProtoBuf;

namespace XianXiaJianServer.Model
{
    [ProtoContract]
    class MsgCallBack
    {
        public MsgCallBack() { }
        public MsgCallBack(RequestCode request, ActionCode action, byte[] dataList)
        {
            RequestCode = request;
            ActionCode = action;
            DataList = dataList;
        }
        public MsgCallBack(ActionCode action, byte[] dataList) 
        {
            ActionCode = action;
            DataList = dataList;
        }
        [ProtoMember(1)]
        public RequestCode RequestCode { get; set; }
        [ProtoMember(2)]
        public ActionCode ActionCode { get; set; }
        [ProtoMember(3)]
        public byte[] DataList;
    }
    
}
