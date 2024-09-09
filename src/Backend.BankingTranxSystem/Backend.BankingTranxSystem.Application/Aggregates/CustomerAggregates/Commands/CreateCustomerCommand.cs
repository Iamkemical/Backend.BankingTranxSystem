using Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.DTOs.Response;
using Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.Specifications;
using Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.Validators;
using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.Commands;

public class CreateCustomerCommand : IRequest<RepositoryActionResult<CreateCustomerResponseDto>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string OtherNames { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PermanentAddress { get; set; }
    public string BusinessRegistrationNumber { get; set; }
    public string TelephoneNumber { get; set; }
    public string Bvn { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public Gender Gender { get; set; }
    public AccountType AccountType { get; set; }
}

public class CreateCustomerHandler(IRepository<Customer> customerRepo,
                                   ILogger<CreateCustomerHandler> logger) : IRequestHandler<CreateCustomerCommand, RepositoryActionResult<CreateCustomerResponseDto>>
{
    private static UniqueSemaphoreSlim semaphoreLock = new();
    public async Task<RepositoryActionResult<CreateCustomerResponseDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        await semaphoreLock.WaitAsync(request.Bvn);
        try
        {
            var validator = new CreateCustomerCommandValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new(null, RepositoryActionStatus.ValidationError, new Exception(String.Join(" | ", validationResult.Errors.Select(c => c.ErrorMessage))));
            }

            var isExistingUserAccount = await customerRepo
               .AnyAsync(new ValidateCreateCustomerSpec(request.Bvn,
                                                        request.EmailAddress,
                                                        request.TelephoneNumber,
                                                        request.BusinessRegistrationNumber), cancellationToken);

            if (isExistingUserAccount)
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.CustomerMsg.CustomerExists));
            }

            Customer customer = new(request.FirstName,
                                    request.LastName,
                                    request.OtherNames,
                                    request.Password,
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

            await customerRepo.AddAsync(customer, cancellationToken);
            await customerRepo.SaveChangesAsync(cancellationToken);

            return new(new CreateCustomerResponseDto(CustomerId: customer.Id), RepositoryActionStatus.Ok);
        }
        catch (Exception ex)
        {
            logger.LogError($"Create Customer failed with logs in {ex.StackTrace} with {ex.Message}");
            return new(null, RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.ErrorOccured));
        }
        finally
        {
            semaphoreLock.Release(request.Bvn);
        }
    }
}