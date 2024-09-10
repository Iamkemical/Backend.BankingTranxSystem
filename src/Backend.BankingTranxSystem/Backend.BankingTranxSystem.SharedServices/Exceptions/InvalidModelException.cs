using System;

namespace Backend.BankingTranxSystem.SharedServices.Exceptions;
public class InvalidModelException(string message) : Exception(message)
{
}