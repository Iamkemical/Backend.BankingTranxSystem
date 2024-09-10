namespace Backend.BankingTranxSystem.SharedServices.Contracts;

public class ApiRoutes
{
    public const string Root = "api";

    public const string Version = "v1";

    public const string Base = Root + "/" + Version;

    public const string AdminBase = Root + "/" + Version + "/" + "admin";

    public static class User
    {
        public const string CreateUser = Base + "/user";
        public const string LoginUser = Base + "/user/login";
    }

    public static class Wallet
    {
        public const string ProcessWalletTransaction = Base + "/wallet/process-wallet-transaction";
        public const string ProcessWalletToWalletTransaction = Base + "/wallet/wallet-to-wallet-transfer";
        public const string GetWalletAndTransactionHistory = Base + "/wallet/wallet-transaction-history";
    }
}