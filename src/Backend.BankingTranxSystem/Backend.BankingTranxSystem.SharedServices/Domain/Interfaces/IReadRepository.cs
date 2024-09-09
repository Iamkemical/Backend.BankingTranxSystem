using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class
{
    IQueryable<T> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
