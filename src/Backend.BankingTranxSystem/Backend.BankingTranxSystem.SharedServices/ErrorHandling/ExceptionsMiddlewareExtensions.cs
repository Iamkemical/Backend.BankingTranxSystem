using Microsoft.AspNetCore.Builder;

namespace Backend.BankingTranxSystem.SharedServices.ErrorHandling;
public static class ExceptionsMiddlewareExtensions   
{
    public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}