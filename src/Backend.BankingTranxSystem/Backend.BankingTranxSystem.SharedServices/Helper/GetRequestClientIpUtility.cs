using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Backend.BankingTranxSystem.SharedServices.Helper;
public static class GetRequestClientIpUtility
{
    //public static async ValueTask<ValueTuple<string, string>> GetRequestIPnLocationAsync(HttpContext httpContext, bool getLocation = true,
    //    bool tryUseXForwardHeader = true)
    //{
    //    string ip = null;

    //    // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

    //    // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
    //    // for 99% of cases however it has been suggested that a better (although tedious)
    //    // approach might be to read each IP from right to left and use the first public IP.
    //    // http://stackoverflow.com/a/43554000/538763
    //    //
    //    if (tryUseXForwardHeader)
    //        ip = GetHeaderValueAs<string>(httpContext, "X-Forwarded-For").SplitCsv().FirstOrDefault();

    //    // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
    //    if (ip.IsStringEmpty() && httpContext.Connection?.RemoteIpAddress != null)
    //        ip = httpContext.Connection.RemoteIpAddress.ToString();

    //    if (ip.IsStringEmpty())
    //        ip = GetHeaderValueAs<string>(httpContext, "REMOTE_ADDR");

    //    // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

    //    if (ip.IsStringEmpty())
    //        throw new Exception("Unable to determine caller's IP.");

    //    return (ip, getLocation ? await GetIpLocation(ip) : null);
    //}

    //public static async Task<string> GetIpLocation(string ipAddress)
    //{
    //    // Sync
    //    using (var client = new WebServiceClient(694422, "qQ4UU0Z80JiXIpiG", host: "geolite.info"))
    //    {
    //        // You can also use `client.City` or `client.Insights`
    //        // `client.Insights` is not available to GeoLite2 users
    //        var response = await client.CityAsync(ipAddress);

    //        //Console.WriteLine(response.Country.IsoCode);        // 'US'
    //        //Console.WriteLine(response.Country.Name);          // 'United States'
    //        //Console.WriteLine(response.Country.Names["zh-CN"]); // '美国'

    //        return response.City.Name + ", " + response.Country.Name;
    //    }

    //}

    //public static async Task<IpLocationDto> GetIpLocationCountryAsync(string ipAddress)
    //{
    //    // Sync
    //    using (var client = new WebServiceClient(694422, "qQ4UU0Z80JiXIpiG", host: "geolite.info"))
    //    {
    //        // You can also use `client.City` or `client.Insights`
    //        // `client.Insights` is not available to GeoLite2 users
    //        var response = await client.CountryAsync(ipAddress);

    //        //Console.WriteLine(response.Country.IsoCode);        // 'US'
    //        //Console.WriteLine(response.Country.Name);          // 'United States'
    //        //Console.WriteLine(response.Country.Names["zh-CN"]); // '美国'

    //        return new IpLocationDto(response.Country.Name, response.Country.IsoCode);
    //    }
    //}

    private static T GetHeaderValueAs<T>(HttpContext httpContext, string headerName)
    {
        StringValues values;

        if (httpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
        {
            string rawValues = values.ToString();   // writes out as Csv when there are multiple.

            if (!rawValues.IsStringEmpty())
                return (T)Convert.ChangeType(values.ToString(), typeof(T));
        }
        return default(T);
    }

    public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
    {
        if (string.IsNullOrWhiteSpace(csvList))
            return nullOrWhitespaceInputReturnsNull ? null : [];

        return csvList
            .TrimEnd(',')
            .Split(',')
            .AsEnumerable<string>()
            .Select(s => s.Trim())
            .ToList();
    }

    public static bool IsStringEmpty(this string s)
    {
        return String.IsNullOrWhiteSpace(s) || String.IsNullOrEmpty(s);
    }
}