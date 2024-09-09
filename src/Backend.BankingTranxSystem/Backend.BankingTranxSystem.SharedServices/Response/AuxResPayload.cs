using Backend.BankingTranxSystem.SharedServices.Helper;

namespace Backend.BankingTranxSystem.SharedServices.Response;
public class AuxResPayload
{
    private const string DEFAULT_SUCCESS_MESSAGE = "Successful";
    private const string success = "success";
    public static string error = "error";

    public string Status { get; set; }
    public string Message { get; set; }
    public object Result { get; set; }

    public AuxResPayload(object result, string status = "200", string msg = null)
    {
        Status = status;
        Result = result;
        Message = SetMessage(status, result, msg);
    }

    public static AuxResPayload Rp(object result, string status = "200", string msg = null)
    {
        return new AuxResPayload(result, status, msg);
    }

    private static string SetMessage(string status, object result, string msg)
    {
        if (msg.IsStringEmpty())
        {
            if (status == ResponseCode.Success)
                return DEFAULT_SUCCESS_MESSAGE;

            if (result is string)
                return result?.ToString();
        }
        return msg;
    }
}