using Backend.BankingTranxSystem.API.Endpoints.Client.v1.Customer;
using Backend.BankingTranxSystem.Application.ActionFilters;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Request.ResourceParameters;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.DTOs.Response;
using Backend.BankingTranxSystem.Application.Aggregates.WalletAggregates.Queries;
using Backend.BankingTranxSystem.SharedServices;
using Backend.BankingTranxSystem.SharedServices.Contracts;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Backend.BankingTranxSystem.SharedServices.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.BankingTranxSystem.API.Endpoints.Client.v1.Wallet;

public class GetWalletAndTransactionHistory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Wallet.GetWalletAndTransactionHistory, async (
                   ISender sender,
                   ILogger<GetWalletAndTransactionHistory> logger,
                   IHttpContextAccessor context,
                   CancellationToken cancellationToken) =>
        {
            context.HttpContext.Request.Headers.TryGetValue("X-REQ-ID", out var value);
            var reqId = value.ToString();
            var query = context.HttpContext.Request.Query;
            var queryParameter = new GetWalletAndTransactionHistoryResourceParameter
            {
                EndDate = DateTime.TryParse(query["EndDate"], out var endDate) ? endDate : (DateTime?)null,
                StartDate = DateTime.TryParse(query["StartDate"], out var startDate) ? startDate : (DateTime?)null,
                IsMonthlyStatement = int.TryParse(query["IsMonthlyStatement"], out var isMonthlyStatement) ? isMonthlyStatement : 0,
                Month = int.TryParse(query["Month"], out var month) ? month : null,
                PageNumber = int.TryParse(query["PageNumber"], out var pageNumber) ? pageNumber : 1,
                PageSize = int.TryParse(query["PageSize"], out var pageSize) ? pageSize : 50,
            };
            var queryHandler = new GetWalletAndTransactionHistoryQuery(reqId,
                                                                       queryParameter);
            var res = await sender.Send(queryHandler, cancellationToken);
            var rsp = ResponsePayload.Rp(new GetWalletAndTransactionHistoryResponseDto(res.Balance,
                                                                                       res.Reference,
                                                                                       res,
                                                                                       res.TotalCount,
                                                                                       res.TotalPages,
                                                                                       res.CurrentPage,
                                                                                       res.PageSize,
                                                                                       res.PaginationData));
            logger.LogInformation($"Get Wallet And Transaction History Response => {JsonSerializer.Serialize(rsp)}");
            return Results.Ok(rsp);
        })
            .Produces<GetWalletTransactionDto>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<UserRequestIdAuthAttribute>()
            .WithTags(Tags.Wallet);
    }
}