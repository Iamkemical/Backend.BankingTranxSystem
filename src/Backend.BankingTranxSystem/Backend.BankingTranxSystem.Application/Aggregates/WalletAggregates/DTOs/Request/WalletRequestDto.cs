using Backend.BankingTranxSystem.DataAccess.Enums;

namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Request;

public record WalletRequestDto(decimal Amount,
                               string Narration,
                               TransactionType TransactionType);