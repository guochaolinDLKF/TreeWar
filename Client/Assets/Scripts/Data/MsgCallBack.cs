using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using ProtoBuf;

[ProtoContract]
class MsgCallBack
{
    public MsgCallBack() { }

    public MsgCallBack(RequestCode requestCode, ActionCode actionCode, byte[] msg)
    {
        RequestCode = requestCode;
        ActionCode = actionCode;
        MsgDataByte = msg;
    }
    public MsgCallBack(ActionCode actionCode, byte[] msg)
    {
        ActionCode = actionCode;
        MsgDataByte = msg;
    }
    public MsgCallBack(ReturnCode returnCode, byte[] data)
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
