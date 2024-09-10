namespace Backend.BankingTranxSystem.API.Installers;

public class HttpInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
    }
}