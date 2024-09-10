namespace Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.DTOs.Response;

public record CreateUserResponseDto(Guid UserId,
                                    string RequestId);