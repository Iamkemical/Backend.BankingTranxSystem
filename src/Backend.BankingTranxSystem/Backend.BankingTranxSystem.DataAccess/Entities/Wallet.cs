using Ardalis.GuardClauses;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Models;

namespace Backend.BankingTranxSystem.DataAccess.Entities;

public class Wallet : SharedBase
{
    public Wallet() { }

    public Wallet(Guid userId,
                  bool isPoolWallet = false,
                  decimal balance = 0)
    {
        UserId = Guard.Against.Default(userId);
        Reference = Utility.GenerateRandomNumber(10);
        IsPoolWallet = isPoolWallet;
        Balance = balance;
        Id = Guid.NewGuid();

        CreatedAt = DateTime.UtcNow;
        CreatedBy = "SYSTEM";

        Audit();
    }

    public void WalletTransactionAction(decimal amount,
                                        TransactionType transactionType)
    {
        if(transactionType == TransactionType.Withdrawal)
        {
            Balance -= amount;
        }

        if (transactionType == TransactionType.Deposit)
        {
            Balance += amount;
        }
    }

    public void Audit(string modifiedBy = "SYSTEM")
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = modifiedBy;
    }

    public Guid UserId { get; private set; }
    public string Reference { get; private set; }
    public decimal Balance { get; private set; }
    public bool IsPoolWallet { get; private set; }

    //User
    private readonly User _user;
    public User User => _user;
}
