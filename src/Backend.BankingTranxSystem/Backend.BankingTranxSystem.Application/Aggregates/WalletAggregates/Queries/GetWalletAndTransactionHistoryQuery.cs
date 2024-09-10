using AutoMapper;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Request.ResourceParameters;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;
using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Queries;

public record GetWalletAndTransactionHistoryQuery(string RequestId,
                                                  GetWalletAndTransactionHistoryResourceParameter Parameter) : IRequest<PagedList<GetWalletTransactionDto>>;


public class GetWalletAndTransactionHistoryHandler(IReadRepository<User> userRepo,
                                                   IReadRepository<WalletTransaction> walletTranxRepo,
                                                   IMapper mapper) : IRequestHandler<GetWalletAndTransactionHistoryQuery, PagedList<GetWalletTransactionDto>>
{
    public async Task<PagedList<GetWalletTransactionDto>> Handle(GetWalletAndTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepo.GetAllAsync()
                                                 .AsNoTracking()
                                                 .Include(c => c.Wallet)
                                                 .Where(c => c.Password == request.RequestId)
                                                 .FirstOrDefaultAsync(cancellationToken);

        if(existingUser is not null)
        {
            var baseQuery = walletTranxRepo.GetAllAsync()
                                           .AsNoTracking()
                                           .Where(c => c.SourceReference == existingUser.Wallet.Reference ||
                                            c.DestinationReference == existingUser.Wallet.Reference);

            int totalCount = await baseQuery.CountAsync(cancellationToken);
            if (request.Parameter.IsMonthlyStatement == 1 && request.Parameter.Month.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.CreatedAt.Month == request.Parameter.Month.Value);
            }
            else if(request.Parameter.StartDate.HasValue && request.Parameter.EndDate.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.CreatedAt >= request.Parameter.StartDate.Value ||
                c.CreatedAt <= request.Parameter.EndDate.Value);
            }

            var existingWalletTransaction = await baseQuery
                                                           .OrderByDescending(c => c.CreatedAt)
                                                           .Skip((request.Parameter.PageNumber - 1) * request.Parameter.PageSize)
                                                           .Take(request.Parameter.PageSize)
                                                           .ToListAsync(cancellationToken);

            var walletTransactionsDto = mapper.Map<GetWalletTransactionDto[]>(existingWalletTransaction).ToList();

            return new(walletTransactionsDto,
                       totalCount,
                       request.Parameter.PageNumber,
                       request.Parameter.PageSize,
                       existingUser.Wallet.Balance,
                       existingUser.Wallet.Reference);
        }
        return null;
    }
}