using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Commands;
using FluentValidation;

namespace Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Validators;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(c => c.EmailAddress)
            .NotEmpty().WithMessage("Email address cannot be empty")
            .EmailAddress().WithMessage("Email address not valid");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Password cannot be empty");
    }
}
