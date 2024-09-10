namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;

public record WalletResponseDto(Guid TransactionReference,
                                decimal Amount,
                                string WalletReference);