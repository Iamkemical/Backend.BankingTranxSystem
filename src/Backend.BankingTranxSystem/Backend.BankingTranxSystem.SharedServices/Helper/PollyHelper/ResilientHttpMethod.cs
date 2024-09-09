using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using Polly.CircuitBreaker;
using Polly;
using Polly.Wrap;
using Backend.BankingTranxSystem.SharedServices.Exceptions;
using System.Text.Json;
using System.Text;
using Marvin.StreamExtensions;
using Microsoft.ApplicationInsights;
using System.Net.Http.Headers;

namespace Backend.BankingTranxSystem.SharedServices.Helper.PollyHelper;

public class ResilientHttpMethod
{
    private readonly HttpClient _httpClient;
    private readonly TelemetryClient _logger;

    //private static readonly AsyncRetryPolicy<HttpResponseMessage> TransientErrorRetryPolicy =
    //    Policy.HandleResult<HttpResponseMessage>(message =>
    //               ((int)message.StatusCode) == 429 || (int)message.StatusCode >= 500)
    //        .WaitAndRetryAsync(5, retryAttempt =>
    //        {
    //            Console.WriteLine($"Retrying because of transient error. Attempt {retryAttempt}");
    //            return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
    //                   + TimeSpan.FromMilliseconds(retryAttempt * 100);
    //        });

    private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy =
        Policy.HandleResult<HttpResponseMessage>(message => (int)message.StatusCode == 503)
            .CircuitBreakerAsync(10, TimeSpan.FromMinutes(1));

    private readonly AsyncPolicyWrap<HttpResponseMessage> _resilientPolicy;// = CircuitBreakerPolicy.WrapAsync(TransientErrorRetryPolicy);

    public ResilientHttpMethod(HttpClient httpClient, TelemetryClient logger, AsyncPolicyWrap<HttpResponseMessage> a)
    {
        _httpClient = httpClient;
        _logger = logger;

        _resilientPolicy = a;
    }

    public async Task<TResponse> ResilientPut<TRequest, TResponse>(string path, TRequest req, CancellationToken cancellationToken) where TResponse : new()
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        TResponse res = new TResponse();

        var response = await _resilientPolicy.ExecuteAsync(
                            async () =>
                            {
                                var request = new HttpRequestMessage(HttpMethod.Put, path);
                                var reqs = JsonSerializer.Serialize(req);

                                request.Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

                                using (var response = await _httpClient.SendAsync(request,
                                        HttpCompletionOption.ResponseHeadersRead,
                                        cancellationToken))
                                {
                                    var streamoutput = await response.Content.ReadAsStringAsync();
                                    var stream = await response.Content.ReadAsStreamAsync();
                                    var data = await response.Content.ReadAsStringAsync();
                                    if (response.IsSuccessStatusCode)
                                    {
                                        try
                                        {
                                            res = stream.ReadAndDeserializeFromJson<TResponse>();
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }

                                    return response;
                                }
                            }
                        );
        return res;
    }

    public async Task<TResponse> ResilientPut<TResponse>(string path, CancellationToken cancellationToken) where TResponse : new()
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        TResponse res = new TResponse();

        var response = await _resilientPolicy.ExecuteAsync(
                            async () =>
                            {
                                //"/api/exchange-rates?currency={currencypair.ToUpper()}")
                                var request = new HttpRequestMessage(HttpMethod.Put, path);

                                //request.Content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                                using (var response = await _httpClient.SendAsync(request,
                                        HttpCompletionOption.ResponseHeadersRead,
                                        cancellationToken))
                                {
                                    var stream = await response.Content.ReadAsStreamAsync();
                                    var data = await response.Content.ReadAsStringAsync();

                                    try
                                    {
                                        res = stream.ReadAndDeserializeFromJson<TResponse>();
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    return response;
                                }
                            }
                        );
        return res;
    }

    public async Task<TResponse> ResilientPost<TRequest, TResponse>(string path, TRequest req, CancellationToken cancellationToken, string authToken = "",
        string choreoBearerToken = "") where TResponse : new()
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        TResponse res = new TResponse();

        var response = await _resilientPolicy.ExecuteAsync(
                            async () =>
                            {
                                var request = new HttpRequestMessage(HttpMethod.Post, path);
                                var reqs = JsonSerializer.Serialize(req);
                                if(!authToken.IsStringEmpty())
                                {
                                    _httpClient.DefaultRequestHeaders.Clear();
                                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                                    _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                                    _httpClient.DefaultRequestHeaders.Add("authorization", authToken);
                                }
                                if(!choreoBearerToken.IsStringEmpty())
                                {
                                    _httpClient.DefaultRequestHeaders.Clear();
                                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                                    _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {choreoBearerToken}");
                                }
                                request.Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

                                using (var response = await _httpClient.SendAsync(request,
                                        HttpCompletionOption.ResponseHeadersRead,
                                        cancellationToken))
                                {
                                    var data = await response.Content.ReadAsStringAsync();
                                    var stream = await response.Content.ReadAsStreamAsync();

                                    try
                                    {
                                        res = stream.ReadAndDeserializeFromJson<TResponse>();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.TrackEvent($"{ex.StackTrace} {ex.Message}");
                                        _logger.TrackException(ex);
                                    }

                                    return response;
                                }
                            }
                        );
        return res;
    }

    //overload for some post that will be passed thru the path
    public async Task<TResponse> ResilientPost<TRequest, TResponse>(string path, CancellationToken cancellationToken) where TResponse : new()
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        TResponse res = new TResponse();

        var response = await _resilientPolicy.ExecuteAsync(
                            async () =>
                            {
                                //"/api/exchange-rates?currency={currencypair.ToUpper()}")
                                var request = new HttpRequestMessage(HttpMethod.Post, path);

                                //request.Content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                                using (var response = await _httpClient.SendAsync(request,
                                        HttpCompletionOption.ResponseHeadersRead,
                                        cancellationToken))
                                {
                                    var stream = await response.Content.ReadAsStreamAsync();

                                    try
                                    {
                                        res = stream.ReadAndDeserializeFromJson<TResponse>();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.TrackEvent($"{ex.StackTrace} {ex.Message}");
                                        _logger.TrackException(ex);
                                    }

                                    return response;
                                }
                            }
                        );
        return res;
    }

    public async Task<TResponse> ResilientPatch<TRequest, TResponse>(string path, TRequest req, CancellationToken cancellationToken) where TResponse : new()
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        TResponse res = new TResponse();

        var response = await _resilientPolicy.ExecuteAsync(
                            async () =>
                            {
                                //"/api/exchange-rates?currency={currencypair.ToUpper()}")
                                var request = new HttpRequestMessage(HttpMethod.Patch, path);

                                request.Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

                                using (var response = await _httpClient.SendAsync(request,
                                        HttpCompletionOption.ResponseHeadersRead,
                                        cancellationToken))
                                {
                                    var stream = await response.Content.ReadAsStreamAsync();

                                    try
                                    {
                                        res = stream.ReadAndDeserializeFromJson<TResponse>();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.TrackEvent($"{ex.StackTrace} {ex.Message}");
                                        _logger.TrackException(ex);
                                    }


                                    return response;
                                }
                            }
                        );
        return res;
    }

    public async Task<TResponse> ResilientGet<TResponse>(string path, CancellationToken cancellationToken, string authToken = "", string choreoBearerToken = "", bool isSettlementApi = false, bool isLive = false) where TResponse : new()
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        TResponse res = new TResponse();

        var response = await _resilientPolicy.ExecuteAsync(
            async () =>
            {
                var request = new HttpRequestMessage(
                HttpMethod.Get, path);
                if (!authToken.IsStringEmpty())
                {
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    _httpClient.DefaultRequestHeaders.Add("authorization", authToken);
                }
                if (!choreoBearerToken.IsStringEmpty())
                {
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {choreoBearerToken}");
                }
                using (var response = await _httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken))
                {
                    var amc = await response.Content.ReadAsStringAsync();
                    var stream = await response.Content.ReadAsStreamAsync();

                    try
                    {
                        res = stream.ReadAndDeserializeFromJson<TResponse>();
                    }
                    catch (Exception ex)
                    {
                        _logger.TrackEvent($"{ex.StackTrace} {ex.Message}");
                        _logger.TrackException(ex);
                    }

                    return response;
                }
            }
        );

        return res;
    }

    public async Task<TResponse> ResilientGet<TRequest, TResponse>(string path, TRequest req, CancellationToken cancellationToken) where TResponse : new()
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        TResponse res = new TResponse();

        var response = await _resilientPolicy.ExecuteAsync(
                            async () =>
                            {
                                //"/api/exchange-rates?currency={currencypair.ToUpper()}")
                                var request = new HttpRequestMessage(HttpMethod.Get, path);

                                request.Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

                                using (var response = await _httpClient.SendAsync(request,
                                        HttpCompletionOption.ResponseHeadersRead,
                                        cancellationToken))
                                {
                                    var stream = await response.Content.ReadAsStreamAsync();

                                    try
                                    {
                                        res = stream.ReadAndDeserializeFromJson<TResponse>();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.TrackEvent($"{ex.StackTrace} {ex.Message}");
                                        _logger.TrackException(ex);
                                    }

                                    return response;
                                }
                            }
                        );
        return res;
    }

    public async Task<TResponse> ResilientDelete<TRequest, TResponse>(string path, TRequest req, CancellationToken cancellationToken) where TResponse : new()
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        TResponse res = new TResponse();

        var response = await _resilientPolicy.ExecuteAsync(
                            async () =>
                            {
                                var request = new HttpRequestMessage(HttpMethod.Delete, path);
                                if (req is not null)
                                {
                                    request.Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");
                                }

                                using (var response = await _httpClient.SendAsync(request,
                                        HttpCompletionOption.ResponseHeadersRead,
                                        cancellationToken))
                                {
                                    var stream = await response.Content.ReadAsStreamAsync();

                                    try
                                    {
                                        res = stream.ReadAndDeserializeFromJson<TResponse>();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.TrackEvent($"{ex.StackTrace} {ex.Message}");
                                        _logger.TrackException(ex);
                                    }


                                    return response;
                                }
                            }
                        );
        return res;
    }

    public async Task<TResponse> ResilientDelete<TResponse>(string path, CancellationToken cancellationToken) where TResponse : new()
    {
        if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
        {
            throw new CircuitBreakerException("Service is currently unavailable");
        }

        TResponse res = new TResponse();

        var response = await _resilientPolicy.ExecuteAsync(
            async () =>
            {
                //"/api/exchange-rates?currency={currencypair.ToUpper()}")
                var request = new HttpRequestMessage(
                HttpMethod.Delete, path);

                using (var response = await _httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken))
                {
                    var amc = await response.Content.ReadAsStringAsync();
                    var stream = await response.Content.ReadAsStreamAsync();

                    try
                    {
                        res = stream.ReadAndDeserializeFromJson<TResponse>();
                    }
                    catch (Exception ex)
                    {
                        _logger.TrackEvent($"{ex.StackTrace} {ex.Message}");
                        _logger.TrackException(ex);
                    }

                    return response;
                }
            }
        );

        return res;
    }
}


