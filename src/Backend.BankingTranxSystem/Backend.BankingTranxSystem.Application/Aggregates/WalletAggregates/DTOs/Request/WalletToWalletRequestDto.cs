namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Request;

public record WalletToWalletRequestDto(string DestinationReference,
                                       decimal Amount,
                                       string Narration);