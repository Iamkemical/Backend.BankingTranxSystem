using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Response;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backend.BankingTranxSystem.SharedServices.SharedFilters;

public class RequestIdAuthAttribute : Attribute, IAsyncActionFilter, IFilterMetadata
{
    private const string FlowIdHeaderName = "Flow-ID";

    private const string RequestIdHeaderName = "X-REQUEST-ID";

    private readonly bool _isExempted;

    public RequestIdAuthAttribute(bool isExempted)
    {
        _isExempted = isExempted;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        IConfiguration configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        ILogger<RequestIdAuthAttribute> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RequestIdAuthAttribute>>();
        if (!configuration.GetValue<bool>("Encryption:IsRequired") || _isExempted)
        {
            await next();
            return;
        }

        string apiKey = configuration.GetValue<string>("ApiKey");
        string key = configuration.GetValue<string>("Encryption:Key");
        string iv = configuration.GetValue<string>("Encryption:Iv");
        int timeLapse = configuration.GetValue<int>("Encryption:SecondLapse");
        string requestId = null;
        if (context.HttpContext.Request.Headers.TryGetValue("Flow-ID", out var flowId))
        {
            requestId = flowId;
        }

        if (requestId.IsStringEmpty() && context.HttpContext.Request.Headers.TryGetValue("X-REQUEST-ID", out var reqId))
        {
            requestId = reqId;
        }

        if (!requestId.IsStringEmpty())
        {
            string decryptedData = Decrypt(requestId, key, iv);
            if (!decryptedData.IsStringEmpty())
            {
                string[] requestParams = decryptedData.ToString().Split(',', StringSplitOptions.TrimEntries);
                if (apiKey.Equals(requestParams.FirstOrDefault()) /*&& long.TryParse(requestParams.Last(), out var tick) && RequestTickIsWithinRange(tick, timeLapse)*/)
                {
                    await next();
                    return;
                }
            }
        }

        logger.LogCritical(9824929, "Request id {id} not valid", requestId);
        context.Result = new BadRequestObjectResult(ResponsePayload.Rp("Request is not valid. Please try again.", "400"));
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

    private bool RequestTickIsWithinRange(long tick, int timeLapseInSeconds)
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTime dateTime = utcNow.AddSeconds(-timeLapseInSeconds);
        DateTime dateTime2 = utcNow;
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(tick);
        if (dateTimeOffset >= dateTime && dateTimeOffset <= dateTime2)
        {
            Console.WriteLine("The timestamp is within the allowed time range.");
            return true;
        }

        Console.WriteLine("The timestamp is outside the allowed time range.");
        return false;
    }
}