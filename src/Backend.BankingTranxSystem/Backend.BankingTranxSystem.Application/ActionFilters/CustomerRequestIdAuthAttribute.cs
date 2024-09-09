using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Backend.BankingTranxSystem.Application.ActionFilters;

public class CustomerRequestIdAuthAttribute : Attribute, IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        IConfiguration configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        IReadRepository<Customer> customerRepo = context.HttpContext.RequestServices.GetRequiredService<IReadRepository<Customer>>();
        ILogger<CustomerRequestIdAuthAttribute> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CustomerRequestIdAuthAttribute>>();
        if (!configuration.GetValue<bool>("Encryption:IsRequired"))
        {
            var result = await next(context);
            return result;
        }

        context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var value);
        var apiKey = value.ToString();
        var configApiKey = configuration.GetValue<string>("ApiKey");
        if (!configApiKey.Equals(apiKey))
        {
            logger.LogCritical(9824929, "Api key {id} not valid", apiKey);
            return Results.BadRequest(ResponsePayload.Rp("Request is not authorized, Invalid API key.", "400"));
        }
        string requestId = null;

        if (requestId.IsStringEmpty() && context.HttpContext.Request.Headers.TryGetValue("X-REQUEST-ID", out var reqId))
        {
            requestId = reqId;
        }

        if (!requestId.IsStringEmpty())
        {
            var isExistingCustomer = await customerRepo.GetAllAsync()
                                                       .AsNoTracking()
                                                       .Where(c => c.Password == requestId)
                                                       .AnyAsync();
            if (isExistingCustomer)
            {
                var result = await next(context);
                return result;
            }
        }

        logger.LogCritical(9824929, "Request id {id} not valid", requestId);
        return Results.BadRequest(ResponsePayload.Rp("Request is not authorized.", "400"));
    }

    private string Decrypt(string encryptedData, string key, string iv)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            byte[] bytes2 = Encoding.UTF8.GetBytes(iv);
            using Aes aes = Aes.Create();
            aes.Key = bytes;
            aes.IV = bytes2;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] buffer = Convert.FromBase64String(encryptedData);
            using MemoryStream stream = new MemoryStream(buffer);
            using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(stream2);
            string text = streamReader.ReadToEnd();
            Console.WriteLine(text);
            return text;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
