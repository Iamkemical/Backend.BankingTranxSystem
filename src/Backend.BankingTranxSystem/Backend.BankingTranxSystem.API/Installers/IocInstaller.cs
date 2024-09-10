using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Commands;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.MappingFactory;
using Backend.BankingTranxSystem.SharedServices.Helper.LazyInitialization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backend.BankingTranxSystem.API.Installers;

public class IocInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(GetType().Assembly, typeof(WalletAggregateMappingFactory).Assembly);
        services.AddHttpContextAccessor();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));
        services.TryAddTransient(typeof(Lazy<>), typeof(LazyInstance<>));
    }
}