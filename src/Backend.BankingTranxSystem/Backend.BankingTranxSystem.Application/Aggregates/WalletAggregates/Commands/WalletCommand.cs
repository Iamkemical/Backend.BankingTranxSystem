using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Validators;
using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Extensions;
using Backend.BankingTranxSystem.SharedServices.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Commands;

public class WalletCommand : IRequest<RepositoryActionResult<WalletResponseDto>>
{
    public string RequestId { get; set; }
    public TransactionType TransactionType { get; set; }
    public string Narration { get; set; }
    public decimal Amount { get; set; }
}

public class WalletHandler(IReadRepository<User> userRepo,
                           IRepository<Wallet> walletRepo,
                           IRepository<WalletTransaction> walletTranxRepo,
                           ILogger<WalletHandler> logger) : IRequestHandler<WalletCommand, RepositoryActionResult<WalletResponseDto>>
{
    private static UniqueSemaphoreSlim semaphoreLock = new();
    public async Task<RepositoryActionResult<WalletResponseDto>> Handle(WalletCommand request, CancellationToken cancellationToken)
    {
        await semaphoreLock.WaitAsync(request.RequestId);
        try
        {
            var validator = new WalletCommandValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new(null, RepositoryActionStatus.ValidationError, new Exception(String.Join(" | ", validationResult.Errors.Select(c => c.ErrorMessage))));
            }

            var existingUser = await userRepo.GetAllAsync()
                                                     .AsNoTracking()
                                                     .Where(c => c.Password == request.RequestId)
                                                     .FirstOrDefaultAsync(cancellationToken);

            if (existingUser is null)
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.UserMsg.UserDoesNotExist));
            }

            var existingWallet = await walletRepo.GetAllAsync()
                                                 .Where(c => c.UserId == existingUser.Id)
                                                 .FirstOrDefaultAsync(cancellationToken);

            if (existingWallet is null)
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.WalletMsg.WalletDoesNotExist));
            }

            var existingPoolWallet = await walletRepo.GetAllAsync()
                                                     .Where(c => c.IsPoolWallet)
                                                     .FirstOrDefaultAsync(cancellationToken);

            if (existingPoolWallet is null)
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.WalletMsg.WalletDoesNotExist));
            }


            if (existingPoolWallet.Balance > request.Amount)
            {
                if(existingWallet.Balance < request.Amount && request.TransactionType == TransactionType.Withdrawal)
                {
                    return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.WalletMsg.InsufficientFundsInWallet));
                }
                existingWallet.WalletTransactionAction(request.Amount,
                                                       request.TransactionType);

                existingPoolWallet.WalletTransactionAction(request.Amount,
                                                           request.TransactionType == TransactionType.Deposit ? 
                                                           TransactionType.Withdrawal : TransactionType.Deposit);

                WalletTransaction walletTranx = new(existingWallet.Reference,
                                                    existingPoolWallet.Reference,
                                                    request.Narration,
                                                    request.Amount,
                                                    request.TransactionType);

                await walletTranxRepo.AddAsync(walletTranx, cancellationToken);
                await walletTranxRepo.SaveChangesAsync(cancellationToken);
                return new(new WalletResponseDto(walletTranx.Id,
                                                 request.Amount,
                                                 existingWallet.Reference), RepositoryActionStatus.Ok);
            }
            else
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.WalletMsg.InsufficientFundsInWallet));
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Wallet {request.TransactionType.GetDescription()} failed with logs in {ex.StackTrace} with {ex.Message}");
            return new(null, RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.ErrorOccured));
        }
        finally
        {
            semaphoreLock.Release(request.RequestId);
        }

    }
}