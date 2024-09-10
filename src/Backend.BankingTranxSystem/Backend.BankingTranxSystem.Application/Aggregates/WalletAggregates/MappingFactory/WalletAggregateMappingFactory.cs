using AutoMapper;
using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Commands;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;
using Backend.BankingTranxSystem.DataAccess.Entities;

namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.MappingFactory;

public class WalletAggregateMappingFactory : Profile
{
    public WalletAggregateMappingFactory()
    {
        CreateMap<WalletTransaction, GetWalletTransactionDto>();
    }
}