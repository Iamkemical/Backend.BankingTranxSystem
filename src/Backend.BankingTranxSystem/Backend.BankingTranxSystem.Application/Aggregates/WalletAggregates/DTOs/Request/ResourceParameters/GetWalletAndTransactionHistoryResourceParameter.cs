using Backend.BankingTranxSystem.SharedServices.Helper;

namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Request.ResourceParameters;

public class GetWalletAndTransactionHistoryResourceParameter : BaseResourceWithDateParameter
{
    public int IsMonthlyStatement { get; set; }
    public int? Month { get; set; }
}