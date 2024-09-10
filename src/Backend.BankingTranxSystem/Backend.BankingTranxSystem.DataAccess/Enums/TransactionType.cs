using System.ComponentModel;

namespace Backend.BankingTranxSystem.DataAccess.Enums;

public enum TransactionType
{
    [Description("Deposit")]
    Deposit,
    [Description("Withdrawal")]
    Withdrawal,
    [Description("Transfer")]
    Transfer
}