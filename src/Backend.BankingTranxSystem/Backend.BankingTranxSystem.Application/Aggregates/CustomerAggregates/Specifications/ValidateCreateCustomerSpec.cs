using Ardalis.Specification;
using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.SharedServices.Helper;

namespace Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.Specifications;

public class ValidateCreateCustomerSpec : Specification<Customer>
{
    public ValidateCreateCustomerSpec(string bvn, string emailAddress, string telephone, string businessRegNo = null)
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