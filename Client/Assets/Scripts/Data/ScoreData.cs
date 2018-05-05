using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using ProtoBuf;

[ProtoContract]
public class ScoreData
{
    public ScoreData() { }
    public ScoreData(ReturnCode returnCode, int id, string username, int totalCount, int winCount)
    {
        ReturnCode = returnCode;
        this.Id = id;
        this.UserName = username;
        this.TotalCount = totalCount;
        this.WinCount = winCount;
    }
    [ProtoMember(1)]
    public ReturnCode ReturnCode { get; set; }
    [ProtoMember(2)]
    public int Id { get; private set; }
    [ProtoMember(3)]
    public string UserName { get; set; }
    [ProtoMember(4)]
    public int TotalCount { get; set; }
    [ProtoMember(5)]
    public int WinCount { get; set; }
}
