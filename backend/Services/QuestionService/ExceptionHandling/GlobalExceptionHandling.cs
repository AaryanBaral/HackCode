using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace QuestionService.ExceptionHandling;
public class GlobalExceptionHandling : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandling> _logger;

    public GlobalExceptionHandling(ILogger<GlobalExceptionHandling> logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
    CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        _logger.LogError(exception,
            "Could not process a request on machine {MachineName}, TraceId:{TraceId}",
            Environment.MachineName,
            traceId);

        var (statusCode, title) = MapException(exception);

        await Results.Problem(
            title: title,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>
            {
                { "traceId", traceId },
            }
        ).ExecuteAsync(context);
        return true;
    }

    private static (int statusCode, string title) MapException(Exception exception)
    {
        return exception switch
        {

            ArgumentOutOfRangeException => ((int)HttpStatusCode.BadRequest, exception.Message),
            ArgumentNullException => ((int)HttpStatusCode.BadRequest, exception.Message),
            ArgumentException => ((int)HttpStatusCode.BadRequest, exception.Message),
            UnauthorizedAccessException => ((int)HttpStatusCode.Forbidden, exception.Message),
            InvalidOperationException => ((int)HttpStatusCode.BadRequest, exception.Message),
            TimeoutException => ((int)HttpStatusCode.InternalServerError, exception.Message),
            DbUpdateException => ((int)HttpStatusCode.BadRequest, exception.Message),
            InvalidCastException => ((int)HttpStatusCode.BadRequest, exception.Message),
            FormatException => ((int)HttpStatusCode.BadRequest, exception.Message),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, exception.Message),
            AuthenticationFailureException => ((int)HttpStatusCode.Unauthorized, exception.Message),
            ValidationException => ((int)HttpStatusCode.BadRequest, exception.Message),
            DuplicateNameException => ((int)HttpStatusCode.BadRequest, exception.Message),
            PostgresException sqlEx => HandlePostgresException(sqlEx), // Direct handling of SqlException
            _ => ((int)HttpStatusCode.InternalServerError, exception.Message),
        };
    }

private static (int statusCode, string title) HandlePostgresException(PostgresException pgEx)
{
    // Handle specific PostgreSQL exception error codes
    return pgEx.SqlState switch
    {
        "23505" => ((int)HttpStatusCode.BadRequest, "Duplicate entry, unique constraint violation."), // Primary key or unique constraint violation
        "22003" => ((int)HttpStatusCode.BadRequest, "Arithmetic overflow error."), // Arithmetic overflow error
        "23503" => ((int)HttpStatusCode.BadRequest, "Foreign key constraint violation."), // Foreign key violation
        "40001" => ((int)HttpStatusCode.InternalServerError, "Deadlock detected."), // Deadlock
        "08003" => ((int)HttpStatusCode.InternalServerError, "Cannot open database requested by the login."), // Database connection issue
        "08006" => ((int)HttpStatusCode.InternalServerError, "Cannot connect to the server."), // Server connection failure
        "28P01" => ((int)HttpStatusCode.Unauthorized, "Login failed for user."), // Authentication failure
        "57014" => ((int)HttpStatusCode.RequestTimeout, "SQL query timeout."), // Query timeout
        "23502" => ((int)HttpStatusCode.BadRequest, "Cannot insert NULL value."), // NULL value violation
        "53P01" => ((int)HttpStatusCode.InternalServerError, "Transaction log is full."), // Log issue
        _ => ((int)HttpStatusCode.InternalServerError, "PostgreSQL Error: " + pgEx.Message), // Default PostgreSQL error
    };
}
}