using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using Serilog;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Backend.BankingTranxSystem.SharedServices.Extensions;

public static class HostedServicesExtensions
{
    public static IHostBuilder UseCustomSerilLog(this IHostBuilder builder, string projectName)
    {
        builder.UseSerilog((hostingContext, loggerConfig) =>
        {
            var config = hostingContext.Configuration;
            loggerConfig.MinimumLevel.Verbose()
                .Enrich.WithProperty("Project", projectName)
                .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .WriteTo.Console();
            if (!string.IsNullOrWhiteSpace(config.GetValue<string>("CustomLogging:FilePath")))
                loggerConfig.WriteTo.File(config.GetValue<string>("CustomLogging:FilePath"),
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    rollingInterval: RollingInterval.Day);
            if (!string.IsNullOrWhiteSpace(config.GetValue<string>("CustomLogging:SeqUrl")))
                loggerConfig.WriteTo.Seq(config.GetValue<string>("CustomLogging:SeqUrl"));
        });
        return builder;
    }
    public static IServiceCollection AddHostedServices(this IServiceCollection services,
        params Assembly[] workersAssemblies)
    {
        MethodInfo methodInfo =
            typeof(ServiceCollectionHostedServiceExtensions)
                .GetMethods()
                .FirstOrDefault(p => p.Name == nameof(ServiceCollectionHostedServiceExtensions.AddHostedService));

        if (methodInfo == null)
            throw new Exception(
                $"Impossible to find the extension method '{nameof(ServiceCollectionHostedServiceExtensions.AddHostedService)}' of '{nameof(IServiceCollection)}'.");

        IEnumerable<Type> hostedServices_FromAssemblies = workersAssemblies.SelectMany(a => a.DefinedTypes)
            .Where(x => !x.IsAbstract && typeof(BackgroundService).IsAssignableFrom(x)).Select(p => p.AsType()).ToList();

        foreach (Type hostedService in hostedServices_FromAssemblies)
        {
            var genericMethod_AddHostedService = methodInfo.MakeGenericMethod(hostedService);
            _ = genericMethod_AddHostedService.Invoke(obj: null,
                parameters: new object[]
                {
                     services
                });
        }

        return services;
    }

}