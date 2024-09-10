using Backend.BankingTranxSystem.API;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Backend.BankingTranxSystem.API.Installers;
public class FluentValidationInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<FluentValidationEntryPoint>();
    }
}
