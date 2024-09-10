using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Backend.BankingTranxSystem.SharedServices.Helper;
public class UniqueSemaphoreSlim
{
    private ConcurrentDictionary<object, SemaphoreSlim> semaphores;

    public UniqueSemaphoreSlim()
    {
        semaphores = new ConcurrentDictionary<object, SemaphoreSlim>();
    }

    public async Task WaitAsync(string reference)
    {
        if (reference == null)
            return;

        SemaphoreSlim semaphore = semaphores.GetOrAdd(reference, _ => new SemaphoreSlim(1));
        await semaphore.WaitAsync();
    }

    public void Release(string reference)
    {
        if (reference == null)
            return;

        if (!semaphores.TryGetValue(reference, out SemaphoreSlim semaphore))
            return;


        semaphore.Release();
        if (semaphore.CurrentCount == 1)
        {
            // Clean up the semaphore when it's no longer needed
            semaphores.TryRemove(reference, out _);
        }
    }
}