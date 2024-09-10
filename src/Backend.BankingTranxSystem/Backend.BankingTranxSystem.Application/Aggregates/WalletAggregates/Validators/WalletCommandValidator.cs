using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Commands;
using FluentValidation;

namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Validators;

public class WalletCommandValidator : AbstractValidator<WalletCommand>
{
    public WalletCommandValidator()
    {
        RuleFor(c => c.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Amount cannot be less than or equal to zero");

        RuleFor(c => c.Narration)
            .NotEmpty().WithMessage("Narration cannot be empty");

        RuleFor(c => c.RequestId)
            .NotEmpty().WithMessage("Request id cannot be empty");

        RuleFor(c => c.TransactionType)
            .IsInEnum().WithMessage("Transaction type is not valid");
    }
}