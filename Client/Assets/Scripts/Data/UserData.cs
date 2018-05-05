using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using ProtoBuf;

[ProtoContract]
public class UserData
{
    public UserData() { }
    public UserData(ReturnCode returnCode, string userid, string username, string password)
    {
        ReturnCode = returnCode;
        UserId = userid;
        UserName = username;
        Password = password;
    }
    [ProtoMember(1)]
    public ReturnCode ReturnCode { get; set; }
    [ProtoMember(2)]
    public string UserId { get; private set; }
    [ProtoMember(3)]
    public string UserName { get; private set; }
    [ProtoMember(4)]
    public string Password { get; set; }
}
