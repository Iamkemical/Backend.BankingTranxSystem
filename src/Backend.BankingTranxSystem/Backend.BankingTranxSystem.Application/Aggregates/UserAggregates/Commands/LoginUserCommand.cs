using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.DTOs.Response;
using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Validators;
using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Commands;

public class LoginUserCommand : IRequest<RepositoryActionResult<LoginUserResponseDto>>
{
    public string EmailAddress { get; set; }
    public string Password { get; set; }
}

public class LoginUserHandler(IReadRepository<User> userRepo,
	                          ILogger<LoginUserHandler> logger) : IRequestHandler<LoginUserCommand, RepositoryActionResult<LoginUserResponseDto>>
{
    public async Task<RepositoryActionResult<LoginUserResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
		try
		{
            var validator = new LoginUserCommandValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new(null, RepositoryActionStatus.ValidationError, new Exception(String.Join(" | ", validationResult.Errors.Select(c => c.ErrorMessage))));
            }

            var existingUser = await userRepo.GetAllAsync()
                                             .AsNoTracking()
                                             .Where(c => c.EmailAddress == request.EmailAddress)
                                             .FirstOrDefaultAsync(cancellationToken);

            if (existingUser is null)
            {
                return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.UserMsg.UserDoesNotExist));
            }

            if(Utility.VerifyHash(existingUser.Password, request.Password))
            {
                return new(new LoginUserResponseDto(RequestId: existingUser.Password), RepositoryActionStatus.Ok);
            }

            return new(null,
                    RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.UserMsg.InvalidEmailOrPassword));
        }
		catch (Exception ex)
		{
            logger.LogError($"Login User failed with logs in {ex.StackTrace} with {ex.Message}");
            return new(null, RepositoryActionStatus.ValidationError, new Exception(BankingTranxSystemMessageConstants.ErrorOccured));
        }
    }
}