using MediatR;
using System;

namespace Backend.BankingTranxSystem.SharedServices.Domain;
public class BaseDomainEvent : INotification
{
    public DateTimeOffset DateOccured { get; protected set; } = DateTime.UtcNow;
}
