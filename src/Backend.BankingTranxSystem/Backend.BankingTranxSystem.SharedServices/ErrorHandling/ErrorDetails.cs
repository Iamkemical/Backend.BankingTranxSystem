using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.BankingTranxSystem.SharedServices.ErrorHandling;

public class ErrorDetails
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("data")]
    public string? Data { get; set; }
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
