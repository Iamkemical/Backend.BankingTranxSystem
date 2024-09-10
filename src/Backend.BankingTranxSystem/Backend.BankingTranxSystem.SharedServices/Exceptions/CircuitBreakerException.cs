using System;

namespace Backend.BankingTranxSystem.SharedServices.Exceptions;
public class CircuitBreakerException(string message) : Exception(message)
{
}