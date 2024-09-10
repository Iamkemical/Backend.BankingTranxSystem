using Ardalis.GuardClauses;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Models;

namespace Backend.BankingTranxSystem.DataAccess.Entities;

public class User : SharedBase
{
    public User() { }

    public User(string firstName,
                string lastName,
                string otherNames,
                string password,
                string emailAddress,
                DateTime? dateOfBirth,
                string permanentAddress,
                string telephoneNumber,
                string bvn,
                string country,
                string state,
                string businessRegistrationNumber,
                Gender gender,
                AccountType accountType = AccountType.Individual,
                bool isSystemAdmin = false)
    {
        FirstName = Guard.Against.NullOrWhiteSpace(firstName);
        LastName = Guard.Against.NullOrWhiteSpace(lastName);
        OtherNames = otherNames;
        EmailAddress = Guard.Against.NullOrWhiteSpace(emailAddress);
        Password = Guard.Against.NullOrWhiteSpace(password);
        DateOfBirth = dateOfBirth;
        PermanentAddress = Guard.Against.NullOrWhiteSpace(permanentAddress);
        TelephoneNumber = Guard.Against.NullOrWhiteSpace(telephoneNumber);
        Bvn = Guard.Against.NullOrWhiteSpace(bvn);
        Country = Guard.Against.NullOrWhiteSpace(country);
        BusinessRegistrationNumber = businessRegistrationNumber;
        State = state;
        Gender = Guard.Against.Null(gender);
        AccountType = Guard.Against.Default(accountType);
        CreatedBy = "SYSTEM";
        IsSystemAdmin = isSystemAdmin;

        Id = Guid.NewGuid();

        Audit();
    }

    public void UpdatedPassword(string password)
    {
        Password = Guard.Against.NullOrWhiteSpace(password);
    }

    public void Audit(string modifiedBy = "SYSTEM")
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = modifiedBy;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string OtherNames { get; private set; }
    public string EmailAddress { get; private set; }
    public string Password { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string PermanentAddress { get; private set; }
    public string TelephoneNumber { get; private set; }
    public string BusinessRegistrationNumber { get; private set; }
    public string Bvn { get; private set; }
    public string Country { get; private set; }
    public string State { get; private set; }
    public Gender Gender { get; private set; }
    public AccountType AccountType { get; private set; }

    public bool IsSystemAdmin { get; private set; }

    //Wallet
    private readonly Wallet _wallet;
    public Wallet Wallet => _wallet;
}