using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Backend.BankingTranxSystem.API.Data;

public class BankingTranxSystemContextFactory : IDesignTimeDbContextFactory<BankingTranxSystemContext>
{
    string localDb = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BankingTranxMiddlewareDB;Integrated Security=false;Connect Timeout=30;Encrypt=false;MultipleActiveResultSets=true";

    public BankingTranxSystemContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BankingTranxSystemContext>();
        optionsBuilder.UseSqlServer(localDb);

        return new BankingTranxSystemContext(null, optionsBuilder.Options);
    }
}
