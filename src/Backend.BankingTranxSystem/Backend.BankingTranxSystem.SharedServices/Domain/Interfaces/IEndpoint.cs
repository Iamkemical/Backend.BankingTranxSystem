using Microsoft.AspNetCore.Routing;

namespace Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}