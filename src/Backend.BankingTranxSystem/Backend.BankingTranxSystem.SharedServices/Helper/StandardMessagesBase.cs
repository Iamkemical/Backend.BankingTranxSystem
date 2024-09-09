using Backend.BankingTranxSystem.SharedServices.Response;

namespace Backend.BankingTranxSystem.SharedServices.Helper;
public class StandardMessagesBase
{
    public static ResponsePayload EmptyRequestBody { get; } =
        new ResponsePayload("Request body is empty or does not match the data contract", ResponseCode.ValidationError);
    public static ResponsePayload SomethingWentWrong { get; } =
        new ResponsePayload("Something went wrong. Please try again", ResponseCode.Error);

    public static ResponsePayload OtpValidationError { get; } =
        new ResponsePayload("Could not validate OTP. Please try again.", ResponseCode.OtpValidationError);

    public static ResponsePayload AuthorizeRequestFailed(object reason) =>
        ResponsePayload.Rp(reason, ResponseCode.OtpValidationError);

    public const string ErrorOccured = "It seems something went wrong. Please try again.";

}
