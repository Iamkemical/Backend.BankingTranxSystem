using Backend.BankingTranxSystem.API.Extensions;
using System.Reflection;

namespace Backend.BankingTranxSystem.API.Installers;

public class EndpointsInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddEndpoints(Assembly.GetExecutingAssembly());
    }
}