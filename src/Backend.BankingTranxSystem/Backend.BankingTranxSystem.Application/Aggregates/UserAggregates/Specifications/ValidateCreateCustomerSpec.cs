using Ardalis.Specification;
using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.SharedServices.Helper;

namespace Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Specifications;

public class ValidateCreateUserSpec : Specification<User>
{
    public ValidateCreateUserSpec(string bvn, string emailAddress, string telephone, string businessRegNo = null)
    {
        if (!businessRegNo.IsStringEmpty())
        {
            Query
            .Where(c => c.Bvn == bvn || c.TelephoneNumber == telephone || c.BusinessRegistrationNumber == businessRegNo || c.EmailAddress == emailAddress);
        }
        else
        {
            Query
            .Where(c => c.Bvn == bvn || c.TelephoneNumber == telephone || c.EmailAddress == emailAddress);
        }
    }
}