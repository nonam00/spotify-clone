using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FluentValidation;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

using Application.Shared.Exceptions;

namespace WebAPI.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private const string UnhandledExceptionMsg = "An unhandled exception has occurred while executing the request.";

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            LoginException => (int)HttpStatusCode.BadRequest,
            OwnershipException => (int)HttpStatusCode.Forbidden,
            _ => context.Response.StatusCode
        };

        var problemDetails = CreateProblemDetails(context, exception);
        var json = ToJson(problemDetails);

        const string contentType = "application/problem+json";
        context.Response.ContentType = contentType;
        await context.Response.WriteAsync(json, cancellationToken);

        return true;
    }
    private ProblemDetails CreateProblemDetails(in HttpContext context, in Exception exception)
    {
        var reasonPhrase = ReasonPhrases.GetReasonPhrase(context.Response.StatusCode);
        
        if (string.IsNullOrEmpty(reasonPhrase))
        {
            reasonPhrase = UnhandledExceptionMsg;
        }

        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = reasonPhrase,
            Detail = exception.Message,
            Extensions =
            {
                ["traceId"] = Activity.Current?.Id,
                ["requestId"] = context.TraceIdentifier,
                ["data"] = exception.Data
            }
        };

        return problemDetails;
    }

    private static string ToJson(in ProblemDetails problemDetails)
    {
        try
        {
            return JsonSerializer.Serialize(problemDetails);
        }
        catch
        {
            return "An exception has occurred while serializing error to JSON";
        }
    }
}