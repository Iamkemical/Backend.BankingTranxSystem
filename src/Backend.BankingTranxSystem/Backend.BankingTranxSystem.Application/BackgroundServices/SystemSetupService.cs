using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend.BankingTranxSystem.Application.BackgroundServices;

public class SystemSetupService(IServiceScopeFactory serviceScope) : BackgroundService
{

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetupAdminProfile(stoppingToken);
        return Task.CompletedTask;
    }

    private async void SetupAdminProfile(CancellationToken stoppingToken)
    {
        using (var scope = serviceScope.CreateScope())
        {
            var userRepo = scope.ServiceProvider.GetService<IRepository<User>>();
            var walletRepo = scope.ServiceProvider.GetService<IRepository<Wallet>>();

            if (userRepo.GetAllAsync().Any(c => c.IsSystemAdmin)) return;

            var passwordHash = Utility.HashText("BankingAdmin");
            User user = new(
                      "Banking",
                      "Admin",
                      "",
                      passwordHash,
                      "bankingadmin@bank.com",
                      null,
                      "Lagos",
                      "2349060880000",
                      "1234567891",
                      "Nigeria",
                      "Lagos",
                      "RC101",
                      Gender.NA,
                      AccountType.Corporate,
                      true);
            await userRepo.AddAsync(user, stoppingToken);

            Wallet wallet = new(user.Id,
                                true,
                                100000000);

            await walletRepo.AddAsync(wallet, stoppingToken);

            await userRepo.SaveChangesAsync(stoppingToken);
        }
        
    }
}
