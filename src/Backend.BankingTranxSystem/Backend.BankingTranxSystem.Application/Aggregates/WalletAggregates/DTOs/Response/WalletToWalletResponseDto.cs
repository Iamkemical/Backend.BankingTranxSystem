namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;

public record WalletToWalletResponseDto(string SourceReference,
                                        string DestinationReference);