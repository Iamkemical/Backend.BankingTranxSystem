using Backend.BankingTranxSystem.SharedServices.Exceptions;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.BankingTranxSystem.SharedServices.Helper.PollyHelper;

public class AuxResilientHttpMethod
{
    private readonly HttpClient _httpClient;

    private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy =
        Policy.HandleResult<HttpResponseMessage>(message => (int)message.StatusCode == 503)
            .CircuitBreakerAsync(10, TimeSpan.FromMinutes(1));

    private readonly AsyncPolicyWrap<HttpResponseMessage> _resilientPolicy;

    public AuxResilientHttpMethod(HttpClient httpClient, AsyncPolicyWrap<HttpResponseMessage> a)
    {
        _httpClient = httpClient;
        _resilientPolicy = a;
    }

    public async Task<HttpResponseMessage> ResilientPost<TRequest>(string path, TRequest req, CancellationToken cancellationToken, string choreoAuthToken = "",
                                                                   string redemptionAppId = "", string redemptionAuthToken = "")
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }
        var response = await _resilientPolicy.ExecuteAsync(
                            async () =>
                            {
                                var request = new HttpRequestMessage(HttpMethod.Post, path);
                                var reqs = JsonSerializer.Serialize(req);
                                if (!choreoAuthToken.IsStringEmpty())
                                {
                                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {choreoAuthToken}");
                                }
                                if(!redemptionAppId.IsStringEmpty() && !redemptionAuthToken.IsStringEmpty())
                                {
                                    _httpClient.DefaultRequestHeaders.Add("app_id", $"{redemptionAppId}");
                                    _httpClient.DefaultRequestHeaders.Add("auth_token", $"{redemptionAuthToken}");
                                }
                                request.Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

                                var response = await _httpClient.SendAsync(request,
                                         HttpCompletionOption.ResponseHeadersRead,
                                         cancellationToken);
                                return response;
                            }
                        );
        return response;
    }

    public async Task<HttpResponseMessage> ResilientGet(string path, CancellationToken cancellationToken, string choreoAuthToken = "", string redemptionAppId = "")
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        var response = await _resilientPolicy.ExecuteAsync(
            async () =>
            {
                var request = new HttpRequestMessage(
                HttpMethod.Get, path);
                if (!choreoAuthToken.IsStringEmpty())
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {choreoAuthToken}");
                }
                if(!redemptionAppId.IsStringEmpty())
                {
                    _httpClient.DefaultRequestHeaders.Add("app-id", $"{redemptionAppId}");
                }
                var response = await _httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken);
                return response;
            }
        );

        return response;
    }
}