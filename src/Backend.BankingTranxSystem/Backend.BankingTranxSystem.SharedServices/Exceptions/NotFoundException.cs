using System;

namespace Backend.BankingTranxSystem.SharedServices.Exceptions;
public class NotFoundException(string message) : Exception(message)
{
}