namespace Backend.BankingTranxSystem.SharedServices.Response;

public class ResponseCode
{
    public const string KycIssue = "403";
    public const string CrossRegion = "413";
    public const string NotFound = "404";
    public const string LimitIssue = "407";
    public const string Success = "000";
    public const string UserDisabled = "401";
    public const string InsufficientBalance = "411";
    public const string Forbidden = "406";
    public const string Error = "500";
    public const string ClientLocation = "483";
    public const string ClientIpAddress = "463";
    public const string SignalRException = "467";
    public const string ValidationError = "400";
    public const string UnprocessableEntity = "422";
    public const string OtpValidationError = "419";
    public const string BadRequestHeader = "429";
}
