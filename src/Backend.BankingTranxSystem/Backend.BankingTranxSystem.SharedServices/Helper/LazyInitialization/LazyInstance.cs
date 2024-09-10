using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BankingTranxSystem.SharedServices.Helper.LazyInitialization;

public class LazyInstance<T>(IServiceProvider serviceProvider) 
    : Lazy<T>(serviceProvider.GetRequiredService<T>())
{
}
