using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Models;
using System.Collections.Generic;
using System.Linq;

namespace Backend.BankingTranxSystem.SharedServices.Response;
public class ResponsePayload
{
    private const string DEFAULT_SUCCESS_MESSAGE = "Successful";
    private const string success = "success";
    public static string error = "error";

    public string Status { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
    public IEnumerable<LinkDto> Links { get; set; } = Enumerable.Empty<LinkDto>();

    public ResponsePayload(object data, string status = ResponseCode.Success, string msg = null)
    {
        Status = status;
        Data = data;
        Message = SetMessage(status, data, msg); ;
    }

    public ResponsePayload(object data, IEnumerable<LinkDto> links, string status = ResponseCode.Success, string msg = null)
    {
        Status = status;
        Links = links;
        Data = data;
        Message = SetMessage(status, data, msg);
    }

    public static ResponsePayload Rp(object data, string status = ResponseCode.Success, string msg = null)
    {
        return new ResponsePayload(data, status, msg);
    }

    public static ResponsePayload Rp(object data, IEnumerable<LinkDto> links, string status = ResponseCode.Success, string msg = null)
    {
        return new ResponsePayload(data, status, msg) { Links = links };
    }

    private static string SetMessage(string status, object data, string msg)
    {
        if (msg.IsStringEmpty())
        {
            if (status == ResponseCode.Success)
                return DEFAULT_SUCCESS_MESSAGE;

            if (data is string)
                return data?.ToString();
        }

        return msg;
    }
}