using Backend.BankingTranxSystem.DataAccess.Enums;

namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;

public record GetWalletAndTransactionHistoryResponseDto(decimal Balance,
                                                        string Reference,
                                                        List<GetWalletTransactionDto> TransactionHistory,
                                                        int TotalCount,
                                                        int TotalPages,
                                                        int PageNumber,
                                                        int PageSize,
                                                        string PaginationData);

public class GetWalletTransactionDto
{
    public string SourceReference { get; set; }
    public string DestinationReference { get; set; }
    public TransactionType TransactionType { get; set; }
    public string Narration { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}