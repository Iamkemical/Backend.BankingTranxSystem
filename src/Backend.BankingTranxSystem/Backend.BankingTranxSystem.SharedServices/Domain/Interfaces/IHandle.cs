using System.Threading.Tasks;

namespace Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
public interface IHandle<T> where T : class
{
    Task HandleAsync(T args);
}
