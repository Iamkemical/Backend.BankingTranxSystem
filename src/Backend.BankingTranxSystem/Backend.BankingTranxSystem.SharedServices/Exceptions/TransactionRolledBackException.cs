using System;

namespace Backend.BankingTranxSystem.SharedServices.Exceptions;
public class TransactionRolledBackException(string message) : Exception(message)
{
}