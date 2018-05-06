using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using ProtoBuf;

[ProtoContract]
public class MsgCallBack
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
