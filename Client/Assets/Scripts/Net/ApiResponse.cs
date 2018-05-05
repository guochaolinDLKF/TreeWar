using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

public class ApiResponse
{
    public ApiResponse() { }

    public ApiResponse(ActionCode code)
    {
        ApiCode = code;
    }
    public ActionCode ApiCode { get; set; }
    public Dictionary<string, object> DataDic { get; set; }
}

