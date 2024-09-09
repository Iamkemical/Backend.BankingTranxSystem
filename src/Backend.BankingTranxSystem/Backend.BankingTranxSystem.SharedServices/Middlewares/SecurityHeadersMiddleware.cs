using Backend.BankingTranxSystem.SharedServices.ErrorHandling;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace Backend.BankingTranxSystem.SharedServices.Middlewares;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private ErrorDetails _error;
    private const int StatusCode = (int)HttpStatusCode.BadRequest;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext httpContext)
    {
        // X-Frame-Options
        httpContext.Response.Headers.TryAdd("X-Frame-Options", "DENY");

        // X-Xss-Protection
        httpContext.Response.Headers.TryAdd("X-XSS-Protection", "1; mode=block");

        // X-Content-Type-Options
        httpContext.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");

        // Referrer-Policy
        httpContext.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");

        // X-Permitted-Cross-Domain-Policies
        httpContext.Response.Headers.TryAdd("X-Permitted-Cross-Domain-Policies", "none");

        // Permissions-Policy
        httpContext.Response.Headers.TryAdd("Permissions-Policy", "accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'");

        // Content-Security-Policy
        httpContext.Response.Headers.TryAdd("Content-Security-Policy", "form-action 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval' https://www.google.com https://code.jquery.com; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://fonts.gstatic.com https://cdn.jsdelivr.net");

        // Check XSS in URL
        if (!string.IsNullOrWhiteSpace(httpContext.Request.Path.Value))
        {
            var url = httpContext.Request.Path.Value;

            if (CrossSiteScriptingValidation.IsDangerousString(url))
            {
                await RespondWithAnError(httpContext);
                return;
            }
        }

        await _next.Invoke(httpContext);
    }

    private async Task RespondWithAnError(HttpContext context)
    {
        context.Response.Clear();
        context.Response.Headers.TryAdd("P3P", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = StatusCode;

        _error ??= new ErrorDetails
        {
            Message = "Error from AntiXssMiddleware",
            Status = "500"
        };

        await context.Response.WriteAsync(JsonConvert.SerializeObject(_error));
    }
}

/// <summary>
/// Imported from System.Web.CrossSiteScriptingValidation Class
/// </summary>
public static class CrossSiteScriptingValidation
{
    private static readonly char[] StartingChars = { '<', '&' };
    public static bool IsDangerousString(string s)
    {
        //bool inComment = false;

        for (var i = 0; ;)
        {
            // Look for the start of one of our patterns 
            var n = s.IndexOfAny(StartingChars, i);

            // If not found, the string is safe
            if (n < 0) return false;

            // If it's the last char, it's safe 
            if (n == s.Length - 1) return false;

            switch (s[n])
            {
                case '<':
                    // If the < is followed by a letter or '!', it's unsafe (looks like a tag or HTML comment)
                    if (char.IsLetter(s[n + 1]) || s[n + 1] == '!' || s[n + 1] == '/' || s[n + 1] == '?') return true;
                    break;
                case '&':
                    // If the & is followed by a #, it's unsafe (e.g. S) 
                    if (s[n + 1] == '#') return true;
                    break;
            }

            // Continue searching
            i = n + 1;
        }
    }
}