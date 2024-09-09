using Backend.BankingTranxSystem.Application.ActionFilters;
using Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.Commands;
using Backend.BankingTranxSystem.Application.Aggregates.CustomerAggregates.DTOs.Response;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Response;
using CamMgt.Api.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.BankingTranxSystem.API.Endpoints.Client.v1.Customer;

public class CreateCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Customer.CreateCustomer, async (
                    CreateCustomerCommand command,
                    IMediator mediator,
                    ILogger<CreateCustomer> logger,
                    CancellationToken cancellationToken) =>
        {
            logger.LogInformation($"Create Customer Request => {JsonSerializer.Serialize(command)}");
            var res = await mediator.Send(command, cancellationToken);
            if (res.Status == RepositoryActionStatus.Ok)
            {
                var rsp = ResponsePayload.Rp(res.Entity);
                logger.LogInformation($"Create Customer Response => {JsonSerializer.Serialize(rsp)}");
                return Results.Ok(rsp);
            }
            var badRsp = ResponsePayload.Rp(res.Entity, ResponseCode.ValidationError, Utility.FriendlyErrorMessageFromException(res.Exception));
            logger.LogInformation($"Create Customer Response => {JsonSerializer.Serialize(badRsp)}");
            return Results.BadRequest(badRsp);
        })
            .Produces<CreateCustomerResponseDto>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<CustomerRequestIdAuthAttribute>()
            .WithTags(Tags.Customers);
    }
}
