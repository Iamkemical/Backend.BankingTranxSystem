using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Commands;
using FluentValidation;

namespace Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Validators;

public class WalletToWalletTransferCommandValidator : AbstractValidator<WalletToWalletTransferCommand>
{
    public WalletToWalletTransferCommandValidator()
    {
        RuleFor(c => c.RequestId)
            .NotEmpty().WithMessage("Request id cannot be empty");

        RuleFor(c => c.DestinationReference)
            .NotEmpty().WithMessage("Destination reference cannot be empty");

        RuleFor(c => c.Amount)
            .GreaterThan(0).WithMessage("Amount cannot be empty");
    }
}
