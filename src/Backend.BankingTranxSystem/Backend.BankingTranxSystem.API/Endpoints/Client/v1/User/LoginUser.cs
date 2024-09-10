using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.Commands;
using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.DTOs.Request;
using Backend.BankingTranxSystem.Application.Aggregates.UserAggregates.DTOs.Response;
using Backend.BankingTranxSystem.SharedServices;
using Backend.BankingTranxSystem.SharedServices.Contracts;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Response;
using MediatR;
using System.Text.Json;

namespace Backend.BankingTranxSystem.API.Endpoints.Client.v1.Customer;

public class LoginUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.User.LoginUser, async (
                    LoginUserRequestDto request,
                    ISender sender,
                    ILogger<CreateUser> logger,
                    CancellationToken cancellationToken) =>
        {
            logger.LogInformation($"Login User Request => {JsonSerializer.Serialize(request)}");
            LoginUserCommand command = new()
            {
                EmailAddress = request.EmailAddress,
                Password = request.Password,
            };
            var res = await sender.Send(command, cancellationToken);
            if (res.Status == RepositoryActionStatus.Ok)
            {
                var rsp = ResponsePayload.Rp(res.Entity);
                logger.LogInformation($"Login User Response => {JsonSerializer.Serialize(rsp)}");
                return Results.Ok(rsp);
            }
            var badRsp = ResponsePayload.Rp(res.Entity, ResponseCode.ValidationError, Utility.FriendlyErrorMessageFromException(res.Exception));
            logger.LogInformation($"Login User Response => {JsonSerializer.Serialize(badRsp)}");
            return Results.BadRequest(badRsp);
        })
            .Produces<CreateUserResponseDto>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags(Tags.User);
    }
}
