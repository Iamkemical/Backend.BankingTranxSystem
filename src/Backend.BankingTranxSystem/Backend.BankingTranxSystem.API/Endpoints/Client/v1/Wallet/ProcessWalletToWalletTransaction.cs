using Backend.BankingTranxSystem.Application.ActionFilters;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Commands;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Request;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;
using Backend.BankingTranxSystem.SharedServices;
using Backend.BankingTranxSystem.SharedServices.Contracts;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Response;
using MediatR;
using System.Text.Json;

namespace Backend.BankingTranxSystem.API.Endpoints.Client.v1.Wallet;

public class ProcessWalletToWalletTransaction : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Wallet.ProcessWalletToWalletTransaction, async (
                    WalletToWalletRequestDto requestDto,
                    ISender sender,
                    ILogger<ProcessWalletToWalletTransaction> logger,
                    IHttpContextAccessor context,
                    CancellationToken cancellationToken) =>
        {
            logger.LogInformation($"Process Wallet To Wallet Transaction Request => {JsonSerializer.Serialize(requestDto)}");
            context.HttpContext.Request.Headers.TryGetValue("X-REQ-ID", out var value);
            var reqId = value.ToString();
            var command = new WalletToWalletTransferCommand
            {
                Amount = requestDto.Amount,
                Narration = requestDto.Narration,
                RequestId = reqId,
                DestinationReference = requestDto.DestinationReference,
            };
            var res = await sender.Send(command, cancellationToken);
            if (res.Status == RepositoryActionStatus.Ok)
            {
                var rsp = ResponsePayload.Rp(res.Entity);
                logger.LogInformation($"Process Wallet To Wallet Transaction Response => {JsonSerializer.Serialize(rsp)}");
                return Results.Ok(rsp);
            }
            var badRsp = ResponsePayload.Rp(res.Entity, ResponseCode.ValidationError, Utility.FriendlyErrorMessageFromException(res.Exception));
            logger.LogInformation($"Process Wallet To Wallet Transaction => {JsonSerializer.Serialize(badRsp)}");
            return Results.BadRequest(badRsp);
        })
            .Produces<WalletToWalletResponseDto>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<UserRequestIdAuthAttribute>()
            .WithTags(Tags.Wallet);
    }
}