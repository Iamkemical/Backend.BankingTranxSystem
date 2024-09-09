using System;

namespace Backend.BankingTranxSystem.SharedServices.Exceptions;
public class NotUserRecordException(string message) : Exception(message)
{
}