using Backend.BankingTranxSystem.SharedServices.Exceptions;
using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Backend.BankingTranxSystem.SharedServices.ErrorHandling;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong: {0}", Utility.FriendlyErrorMessageFromException(ex));
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context is not null && exception is not null)
        {
            context.Response.ContentType = "application/json";
            if (exception.GetType() == typeof(NotFoundException))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return context.Response.WriteAsync(new ErrorDetails()
                {
                    Status = ResponseCode.NotFound,
                    Data = exception.Message,
                    Message = exception.Message
                }.ToString());
            }

            if (exception.GetType() == typeof(NotUserRecordException))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return context.Response.WriteAsync(new ErrorDetails()
                {
                    Status = ResponseCode.Forbidden,
                    Data = "You do not own this record",
                    Message = "You do not own this record"
                }.ToString());
            }

            if (exception.GetType() == typeof(TransactionRolledBackException))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(new ErrorDetails()
                {
                    Status = ResponseCode.Error,
                    Data = StandardMessagesBase.ErrorOccured,
                    Message = StandardMessagesBase.ErrorOccured
                }.ToString());
            }

            if (exception.GetType() == typeof(InvalidModelException))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(new ErrorDetails()
                {
                    Status = ResponseCode.ValidationError,
                    Data = Utility.FriendlyErrorMessageFromException(exception),
                    Message = Utility.FriendlyErrorMessageFromException(exception)
                }.ToString());
            }

            if (exception.GetType() == typeof(TaskCanceledException) || exception.GetType() == typeof(OperationCanceledException))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(new ErrorDetails()
                {
                    Status = ResponseCode.Error,
                    Data = StandardMessagesBase.ErrorOccured,
                    Message = StandardMessagesBase.ErrorOccured
                }.ToString());
            }


            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(new ErrorDetails()
            {
                Status = ResponseCode.Error,
                Data = StandardMessagesBase.ErrorOccured,
                Message = StandardMessagesBase.ErrorOccured
            }.ToString());
        }

        return Task.CompletedTask;
    }

    private string GetResponseCode(Exception ex)
    {
        if (ex.GetType() == typeof(NotFoundException))
            return "404";
        if (ex.GetType() == typeof(NotUserRecordException))
            return "403";
        return "500";
    }
}