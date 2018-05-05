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

        public MsgCallBack(ActionCode actionCode, byte[] msg)
        {
            ActionCode = actionCode;
            MsgDataByte = msg;
        }
        public MsgCallBack(RequestCode requestCode,ActionCode actionCode, byte[] msg)
        {
            RequestCode = requestCode; 
            ActionCode = actionCode;
            MsgDataByte = msg;
        }

        public MsgCallBack(ReturnCode returnCode,byte[] data)
        {
            ReturnCode = returnCode;
            MsgDataByte = data;
        }
        [ProtoMember(1)]
        public RequestCode RequestCode { get; set; }
        [ProtoMember(2)]
        public ActionCode ActionCode { get; set; }
        [ProtoMember(3)]
        public ReturnCode ReturnCode { get; set; }
        [ProtoMember(4)]
        public byte[] MsgDataByte { get; set; }
    }
    
}
