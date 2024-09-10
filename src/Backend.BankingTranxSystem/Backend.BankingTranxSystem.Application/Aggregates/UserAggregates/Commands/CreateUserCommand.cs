using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.DTOs.Response;
using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Specifications;
using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Validators;
using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Commands;

public class CreateUserCommand : IRequest<RepositoryActionResult<CreateUserResponseDto>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string OtherNames { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string PermanentAddress { get; set; }
    public string BusinessRegistrationNumber { get; set; }
    public string TelephoneNumber { get; set; }
    public string Bvn { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public Gender Gender { get; set; }
    public AccountType AccountType { get; set; }
}

public class CreateUserHandler(IRepository<User> userRepo,
                               IRepository<Wallet> walletRepo,
                               ILogger<CreateUserHandler> logger) : IRequestHandler<CreateUserCommand, RepositoryActionResult<CreateUserResponseDto>>
{
    private static UniqueSemaphoreSlim semaphoreLock = new();
    public async Task<RepositoryActionResult<CreateUserResponseDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await semaphoreLock.WaitAsync(request.Bvn);
        try
        {
            var validator = new CreateUserCommandValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new(null, RepositoryActionStatus.ValidationError, new Exception(String.Join(" | ", validationResult.Errors.Select(c => c.ErrorMessage))));
            }

            var isExistingUserAccount = await userRepo
               .AnyAsync(new ValidateCreateUserSpec(request.Bvn,
                                                    request.EmailAddress,
                                                    request.TelephoneNumber,
                                                    request.BusinessRegistrationNumber), cancellationToken);

            if (isExistingUserAccount)
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.UserMsg.UserExists));
            }
            var passwordHash = Utility.HashText(request.Password);
            User user = new(request.FirstName,
                            request.LastName,
                            request.OtherNames,
                            passwordHash,
                            request.EmailAddress,
                            request.DateOfBirth,
                            request.PermanentAddress,
                            request.TelephoneNumber,
                            request.Bvn,
                            request.Country,
                            request.State,
                            request.BusinessRegistrationNumber,
                            request.Gender,
                            request.AccountType);

            await userRepo.AddAsync(user, cancellationToken);

            Wallet wallet = new(user.Id);
            await walletRepo.AddAsync(wallet, cancellationToken);

            await userRepo.SaveChangesAsync(cancellationToken);

            return new(new CreateUserResponseDto(UserId: user.Id,
                                                 RequestId: passwordHash), RepositoryActionStatus.Ok);
        }
        catch (Exception ex)
        {
            logger.LogError($"Create User failed with logs in {ex.StackTrace} with {ex.Message}");
            return new(null, RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.ErrorOccured));
        }
        finally
        {
            semaphoreLock.Release(request.Bvn);
        }
    }
}