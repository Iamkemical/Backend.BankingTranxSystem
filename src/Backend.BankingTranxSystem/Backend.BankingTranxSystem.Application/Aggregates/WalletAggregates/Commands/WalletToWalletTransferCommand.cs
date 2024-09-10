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

public class WalletToWalletTransferCommand : IRequest<RepositoryActionResult<WalletToWalletResponseDto>>
{
    public string RequestId { get; set; }
    public string DestinationReference { get; set; }
    public string Narration { get; set; }
    public decimal Amount { get; set; }
}

public class WalletToWalletTransferHandler(ILogger<WalletToWalletTransferHandler> logger,
                                           IRepository<WalletTransaction> walletTranxRepo,
                                           IReadRepository<User> userRepo,
                                           IRepository<Wallet> walletRepo) : IRequestHandler<WalletToWalletTransferCommand, RepositoryActionResult<WalletToWalletResponseDto>>
{
    private static UniqueSemaphoreSlim semaphoreLock = new();
    public async Task<RepositoryActionResult<WalletToWalletResponseDto>> Handle(WalletToWalletTransferCommand request, CancellationToken cancellationToken)
    {
        await semaphoreLock.WaitAsync(request.RequestId);
        try
        {
            var validator = new WalletToWalletTransferCommandValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new(null, RepositoryActionStatus.ValidationError, new Exception(String.Join(" | ", validationResult.Errors.Select(c => c.ErrorMessage))));
            }

            var existingUser = await userRepo.GetAllAsync()
                                                     .AsNoTracking()
                                                     .Where(c => c.Password == request.RequestId)
                                                     .FirstOrDefaultAsync(cancellationToken);

            if(existingUser is null)
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.UserMsg.UserDoesNotExist));
            }

            var existingDestinationWallet = await walletRepo.GetAllAsync()
                                                            .Where(c => c.Reference == request.DestinationReference)
                                                            .FirstOrDefaultAsync(cancellationToken);

            if(existingDestinationWallet is null)
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.WalletMsg.WalletDoesNotExist));
            }

            var existingWallet = await walletRepo.GetAllAsync()
                                                 .Where(c => c.UserId == existingUser.Id)
                                                 .FirstOrDefaultAsync(cancellationToken);

            if (existingWallet is null)
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.WalletMsg.WalletDoesNotExist));
            }
            

            if (existingWallet.Balance >= request.Amount)
            {
                existingWallet.WalletTransactionAction(request.Amount,
                                                       TransactionType.Withdrawal);

                existingDestinationWallet.WalletTransactionAction(request.Amount,
                                                                  TransactionType.Deposit);
            }
            else
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.WalletMsg.InsufficientFundsInWallet));
            }

            WalletTransaction walletTranx = new(existingWallet.Reference,
                                                request.DestinationReference,
                                                request.Narration,
                                                request.Amount,
                                                TransactionType.Transfer);

            await walletTranxRepo.AddAsync(walletTranx, cancellationToken);
            await walletTranxRepo.SaveChangesAsync(cancellationToken);

            return new(new WalletToWalletResponseDto(existingWallet.Reference,
                                                     request.DestinationReference), RepositoryActionStatus.Ok);
        }
        catch (Exception ex)
        {
            logger.LogError($"Wallet {TransactionType.Transfer.GetDescription()} failed with logs in {ex.StackTrace} with {ex.Message}");
            return new(null, RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.ErrorOccured));
        }
        finally
        {
            semaphoreLock.Release(request.RequestId);
        }
    }
}