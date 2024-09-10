using AutoMapper;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Request.ResourceParameters;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;
using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.SharedServices.Cache;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Queries;

public record GetWalletAndTransactionHistoryQuery(string RequestId,
                                                  GetWalletAndTransactionHistoryResourceParameter Parameter) : IRequest<PagedList<GetWalletTransactionDto>>;


public class GetWalletAndTransactionHistoryHandler(IReadRepository<User> userRepo,
                                                   IReadRepository<WalletTransaction> walletTranxRepo,
                                                   IMapper mapper,
                                                   IResponseCacheService responseCache) : IRequestHandler<GetWalletAndTransactionHistoryQuery, PagedList<GetWalletTransactionDto>>
{
    public async Task<PagedList<GetWalletTransactionDto>> Handle(GetWalletAndTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        string cacheKey;
        if(request.Parameter.IsMonthlyStatement == 1 && request.Parameter.Month.HasValue)
        {
            cacheKey = $"wallet-history-{request.RequestId}-{request.Parameter.IsMonthlyStatement}-{request.Parameter.Month.HasValue}";
        }
        else if (request.Parameter.StartDate.HasValue && request.Parameter.EndDate.HasValue)
        {
            cacheKey = $"wallet-history-{request.RequestId}-{request.Parameter.StartDate.Value}-{request.Parameter.EndDate.Value}";
        }
        else
        {
            cacheKey = $"wallet-history-{request.RequestId}-{request.Parameter.PageNumber}-{request.Parameter.PageSize}";
        }

        var res = await responseCache.GetCachedResponseAsync<CachePagedList<GetWalletTransactionDto>>(cacheKey);

        if (res is null)
        {
            var existingUser = await userRepo.GetAllAsync()
                                             .AsNoTracking()
                                             .Include(c => c.Wallet)
                                             .Where(c => c.Password == request.RequestId)
                                             .FirstOrDefaultAsync(cancellationToken);

            if (existingUser is not null)
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
                else if (request.Parameter.StartDate.HasValue && request.Parameter.EndDate.HasValue)
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

                PagedList<GetWalletTransactionDto> pagedList = new(walletTransactionsDto,
                                                                   totalCount,
                                                                   request.Parameter.PageNumber,
                                                                   request.Parameter.PageSize,
                                                                   existingUser.Wallet.Balance,
                                                                   existingUser.Wallet.Reference);

                await responseCache.CachePagedListResponseAsync(cacheKey,
                                                                pagedList,
                                                                TimeSpan.FromMinutes(5));

                return pagedList;
            }
        }
        return new(res.Items.ToList(),
                   res.TotalCount,
                   res.CurrentPage,
                   res.PageSize,
                   res.Balance,
                   res.Reference);
    }
}