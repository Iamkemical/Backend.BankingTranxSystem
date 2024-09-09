namespace Coronation.Infrastructure.Services;

public class Constants
{
    public const string PDFCompressUrl = "https://pdfcomptessor.azurewebsites.net/api/v1/File/CompressPdf";

    public const string FileSizeLimit = nameof(FileSizeLimit);

    public const string Slash = "/";
    public const string Pipe = "|";

    public const string BaseUrlHash = nameof(BaseUrlHash);
    public const string SessionIdHash = nameof(SessionIdHash);

    public const string Appname = "Coronation Wealth";
    public const string ImageFile = nameof(ImageFile);

    public const decimal MaxRedeemableAmountLimit = 1000000;
    public const int MaxRedeemableDailyLimit = 2;
    public class FileTypes
    {
        public const string PDF = ".pdf";
        public const string PDFMIME = "application/pdf";
    }

    public class APIEncoding
    {
        public const string Json = "application/json";
        public const string Form = "multipart/form-data";
    }

    public const string Mode = nameof(Mode);
    public class AccountType
    {
        public const string Individual = "IND";
        public const string Corporation = "CORP";
    }

    public class EnviromentType
    {
        public const string Test = nameof(Test);
        public const string Live = nameof(Live);
    }

    public class EntityCode
    {
        public const string CAM = nameof(CAM);
    }

    public const string DefaultDob = "0001-01-01";
    public const string ErrorOccured = "Error Occured";

}
