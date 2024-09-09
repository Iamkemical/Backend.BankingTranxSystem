using Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.Commands;
using Backend.BankingTranxSystem.SharedServices.Helper.LazyInitialization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backend.BankingTranxSystem.API.Installers;

public class IocInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        //services.AddAutoMapper(GetType().Assembly, typeof(CustomerAggregateMappingFactory).Assembly);
        services.AddHttpContextAccessor();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCustomerCommand).Assembly));
        services.TryAddTransient(typeof(Lazy<>), typeof(LazyInstance<>));
    }
}