using Ardalis.GuardClauses;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Domain;

namespace Backend.BankingTranxSystem.DataAccess.Entities;

public class WalletTransaction : BaseEntity<Guid>
{
    public WalletTransaction() { }

    public WalletTransaction(string sourceReference,
                             string destinationReference,
                             string narration,
                             decimal amount,
                             TransactionType transactionType = TransactionType.Deposit)
    {
        SourceReference = Guard.Against.NullOrWhiteSpace(sourceReference);
        DestinationReference = Guard.Against.NullOrWhiteSpace(destinationReference);
        Amount = Guard.Against.NegativeOrZero(amount);
        Narration = narration;
        CreatedAt = DateTime.UtcNow;
        TransactionType = transactionType;
        Id = Guid.NewGuid();
    }

    public string SourceReference { get; private set; }
    public string DestinationReference { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public string Narration { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}