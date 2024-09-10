using MediatR;
using System;

namespace Backend.BankingTranxSystem.SharedServices.Domain;
public class BaseIntegrationEvent : INotification
{
    public DateTimeOffset DateOccured { get; protected set; } = DateTime.UtcNow;
    string EventType { get; }
}
