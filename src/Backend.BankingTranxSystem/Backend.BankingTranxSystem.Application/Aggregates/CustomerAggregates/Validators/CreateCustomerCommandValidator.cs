using Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.Commands;
using Backend.BankingTranxSystem.DataAccess.Enums;
using FluentValidation;

namespace Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.Validators;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty");

        RuleFor(c => c.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty");

        RuleFor(c => c.PermanentAddress)
            .NotEmpty().WithMessage("Permanent address cannot be empty");

        RuleFor(c => c.Gender)
            .IsInEnum().WithMessage("Gender is not valid");

        RuleFor(c => c.AccountType)
            .IsInEnum().WithMessage("Account type is not valid");

        RuleFor(c => c.Country)
            .NotEmpty().WithMessage("Country cannot be empty");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Password cannot be empty");

        RuleFor(c => c.EmailAddress)
            .EmailAddress().WithMessage("Not a valid email address")
            .NotEmpty().WithMessage("Email address cannot be empty");

        RuleFor(c => c.Gender)
            .IsInEnum().WithMessage("Gender is not valid")
            .When(c => c.AccountType == AccountType.Corporate && c.Gender != Gender.NA)
            .WithMessage("Gender should be nuetral for corporate accounts");


        RuleFor(c => c.AccountType)
             .NotEmpty().WithMessage("Account type cannot be empty")
             .Must(c => c == AccountType.Individual || c == AccountType.Corporate)
             .WithMessage("Invalid account type")
             .When(c => c.AccountType == AccountType.Individual && c.State == null)
             .WithMessage("Individual account must have state of origin");

        RuleFor(c => c.BusinessRegistrationNumber)
            .NotEmpty()
            .When(c => c.AccountType == AccountType.Corporate)
            .WithMessage("Corporate customer must supply business registration number");

        RuleFor(c => c.PermanentAddress)
            .NotEmpty().WithMessage("Permanent address cannot be empty");

        RuleFor(c => c.TelephoneNumber)
            .NotEmpty().WithMessage("Telephone number cannot be empty");
    }
}