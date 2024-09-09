using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;

namespace Backend.BankingTranxSystem.SharedServices.Helper;
public static class Utility
{
    public const string STAGING = "Staging";
    public const string DEVELOPMENT = "Development";
    public const string PRODUCTION = "Production";

    static Random r = new Random();

    public static string GenerateRandomNumber(int size)
    {
        var a = "";
        for (int i = 0; i < size; i++)
        {
            a += r.Next(0, 9);
        }

        return a;
    }
    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string ToLocalNigerianPhone(this string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;
        phone = phone.Replace("+", string.Empty);
        if (phone.Substring(0, 3) == "234")
        {
            phone = phone.Remove(0, 3);
            phone = 0.ToString() + phone;
        }

        return phone;
    }
    public static string GenerateRandomChars(int size)
    {
        var a = "";
        for (int i = 0; i < size; i++)
        {
            //is even get lower alphabets else upper
            a += r.Next() % 2 == 0 ? (char)r.Next(97, 122) : (char)r.Next(65, 90);
        }

        return a;
    }

    public static string GetEnvironmentName()
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.IsNullOrEmpty(environment) ? "Development" : environment;
    }

    public static bool IsDevelopment()
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.IsNullOrEmpty(environment) || environment == "Development" ? true : false;
    }

    public static string FriendlyErrorMessageFromException(Exception ex)
    {
        if (ex == null)
            return "Something went wrong";

        if (ex.InnerException != null)
        {
            return ex.InnerException.Message.Contains("Cannot insert duplicate key row in object") ?
                "Cannot insert duplicate key. Please verify the data provided" : ex.InnerException?.Message;
        }
        return ex?.Message;
    }

    

    public static string GetTimestamp(DateTime dt)
    {
        return dt.ToString("yyyyMMddHHmmssffff");
    }

    public static string[] GetDefaultVaryByHeader()
    {
        return new string[] { "Accept", "Accept-Charset", "Accept-Encoding", "Accept-Language" };
    }

    public static object SerializeObjectToBeCached<T>(T obj)
    {
        JsonSerializer _camelCaseSerializer = JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        try
        {
            return JObject.FromObject(obj, _camelCaseSerializer);
        }
        catch (Exception)
        {
            return JToken.FromObject(obj, _camelCaseSerializer);
        }
    }

    public static JsonSerializer GetJsonSerializerForCamelCase()
    {
        JsonSerializer _camelCaseSerializer = JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

        return _camelCaseSerializer;
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }

    public static DateTime UnixTimeStampToDateTimev2(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }

    public static Dictionary<string, string> ObjectToDictionaryExcJsonIgnore(object obj)
    {
        var o = obj.GetType()
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Any() == false)
                .ToDictionary(p => p.Name, p => p.GetValue(obj)?.ToString());

        var env = GetEnvironmentName();

        o.Add("Environment", env != "Production" ? $"[{env}]" : "");

        return o;
    }

    public static string GenerateCacheKeyFromRequest(HttpRequest request, string userId, bool isPrivateResponse)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"{request.Path}");

        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        if (isPrivateResponse)
        {
            keyBuilder.Append(userId);
        }

        return keyBuilder.ToString();
    }

    public static bool TimeStillValid(DateTimeOffset dte, int paymentWindow)
    {
        dte = dte.ToUniversalTime().AddMinutes((int)paymentWindow);
        return DateTimeOffset.Now.ToUniversalTime() > dte;
    }

    public static BasicAuthAuthorizationFilter GetHangfireBasicAuthFilter()
    {
        return new BasicAuthAuthorizationFilter(
            new BasicAuthAuthorizationFilterOptions
            {
                // Require secure connection for dashboard
                RequireSsl = false,
                // Case sensitive login checking
                LoginCaseSensitive = true,
                // Users
                Users = new[]
                {
                    new BasicAuthAuthorizationUser
                    {
                        Login = "Coronation-Admin",
                        // Password as plain text, SHA1 will be used
                        PasswordClear = "Coronation"
                    },
                    new BasicAuthAuthorizationUser
                    {
                        Login = "Coronation-Admin2",
                        // Password as SHA1 hash
                        Password = new byte[]{0xa9,
                            0x4a, 0x8f, 0xe5, 0xcc, 0xb1, 0x9b,
                            0xa6, 0x1c, 0x4c, 0x08, 0x73, 0xd3,
                            0x91, 0xe9, 0x87, 0x98, 0x2f, 0xbb,
                            0xd3}
                    }
                }
            });
    }

    public static DashboardOptions GetBasicHangfireDashboardOptions()
    {
        var options = new DashboardOptions
        {
            Authorization = new[]
            {
                 GetHangfireBasicAuthFilter()
            }
        };

        return options;
    }

    public static IConfigurationRoot GetConfiguration()
    {
        var environmentName = Utility.GetEnvironmentName();
        //var basePath = PlatformServices.Default
        //    .Application.ApplicationBasePath;
        var configurationBuilder = new ConfigurationBuilder()
            //.SetBasePath(basePath)
            .AddJsonFile($"appsettings.{environmentName}.json");

        return configurationBuilder.Build();
    }

    public static string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public static string GetQueryString(object obj)
    {
        var properties = from p in obj.GetType().GetProperties()
                         where p.GetValue(obj, null) != null
                         select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

        return String.Join("&", properties.ToArray());
    }

    public static bool IsValidUrl(string url)
    {
        bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
            && Uri.IsWellFormedUriString(url, UriKind.Absolute);
        return result;
    }

    public static string HashText(string text)
    {
        return BCrypt.Net.BCrypt.HashPassword(text);
    }

    public static bool VerifyHash(string hash, string text)
    {
        return BCrypt.Net.BCrypt.Verify(text, hash);
    }

    public static (string appId, string appSecret, string apiKey) GenerateCredentials()
    {
        byte[] randomBytes = new byte[64]; // Adjust the size as needed (128 bits = 16 bytes)
#pragma warning disable SYSLIB0023
        using var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(randomBytes);
        string apiKey = Convert.ToBase64String(randomBytes);
#pragma warning restore SYSLIB0023

        using Aes aes = Aes.Create();
        aes.KeySize = 128; // Set the key size to 128 bits
        aes.GenerateKey(); // Generate a new key

        // Get the first 16 bytes (128 bits) of the generated key
        byte[] key = new byte[16];
        Buffer.BlockCopy(aes.Key, 0, key, 0, 16);

        string appSecretBase = Convert.ToBase64String(key);
        string appSecret;
        if (appSecretBase.Length >= 16)
        {
            appSecret = appSecretBase.Substring(appSecretBase.Length - 16);
        }
        else
        {
            appSecret = appSecretBase;
        }
        aes.GenerateIV();
        byte[] iv = aes.IV;
        string appIdBase = Convert.ToBase64String(iv);
        string appId;
        if (appIdBase.Length >= 16)
        {
            appId = appIdBase.Substring(appIdBase.Length - 16);
        }
        else
        {
            appId = appIdBase;
        }

        return (appId, appSecret, apiKey);
        // Now 'base64Key' contains the 128-bit key
    }


    //public static ValueTuple<string, DateTime?, DateTime?> GetDateFilter(DateTime? startPeriod, DateTime? endPeriod, string field)
    //{
    //    if (startPeriod == null && endPeriod == null)
    //        return (String.Empty, startPeriod, endPeriod);


    //    if (startPeriod > endPeriod)
    //    {
    //        endPeriod = startPeriod.Value.AddMonths(1);
    //    }

    //    if (startPeriod.HasValue && endPeriod.HasValue)
    //    {
    //        var dteSpan = DateTimeSpan.CompareDates(startPeriod.Value.Date, endPeriod.Value.Date);

    //        //if not exactly one month
    //        //if ((dteSpan.Months <= 1 && dteSpan.Years == 0 && dteSpan.Days == 0) == false)
    //        //{
    //        //    endPeriod = startPeriod.Value.AddMonths(1);
    //        //}

    //        if ((dteSpan.Months == 1 && dteSpan.Years == 0 && dteSpan.Days == 0) == false &&
    //            (dteSpan.Months < 1 && dteSpan.Years == 0 && dteSpan.Days > 0) == false)
    //        {
    //            endPeriod = startPeriod.Value.AddMonths(1);
    //        }
    //    }
    //    else if (startPeriod.HasValue && endPeriod.HasValue == false)
    //    {
    //        endPeriod = startPeriod.Value.AddMonths(1);
    //    }
    //    else if (startPeriod.HasValue == false && endPeriod.HasValue)
    //    {
    //        startPeriod = endPeriod.Value.AddMonths(-1);
    //    }

    //    return ($" and {field} >= @startPeriod and {field} <= @endPeriod ", startPeriod, endPeriod);
    //}

}