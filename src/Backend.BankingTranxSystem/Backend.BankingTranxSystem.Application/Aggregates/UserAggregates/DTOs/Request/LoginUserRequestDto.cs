namespace Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.DTOs.Request;

public record LoginUserRequestDto(string EmailAddress,
                                  string Password);