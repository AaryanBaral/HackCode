using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Net;
using Confluent.Kafka;
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

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        _logger.LogError(exception,
            "Could not process a request on machine {MachineName}, TraceId:{TraceId}",
            Environment.MachineName,
            traceId);

        var (statusCode, title, errorCode) = MapException(exception);

        await Results.Problem(
            title: title,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>
            {
                { "traceId", traceId },
                { "errorCode", errorCode }
            }
        ).ExecuteAsync(context);
        return true;
    }

    private static (int statusCode, string title, string errorCode) MapException(Exception exception)
    {
        return exception switch
        {
            ConsumeException ex when (ex.Error.Code == ErrorCode.UnknownTopicId) =>
     ((int)HttpStatusCode.BadRequest,
     $"Kafka topic not available: {ex.Error.Reason}",
     "KAFKA_TOPIC_UNAVAILABLE"),
            ConsumeException ex when (ex.Error.IsFatal) =>
                ((int)HttpStatusCode.ServiceUnavailable,
                $"Kafka broker unavailable: {ex.Message}",
                "KAFKA_UNAVAILABLE"),
            ArgumentOutOfRangeException => ((int)HttpStatusCode.BadRequest, exception.Message, "ARG_OUT_OF_RANGE"),
            ArgumentNullException => ((int)HttpStatusCode.BadRequest, exception.Message, "ARG_NULL"),
            ArgumentException => ((int)HttpStatusCode.BadRequest, exception.Message, "ARG_INVALID"),
            UnauthorizedAccessException => ((int)HttpStatusCode.Forbidden, exception.Message, "UNAUTHORIZED"),
            InvalidOperationException => ((int)HttpStatusCode.BadRequest, exception.Message, "INVALID_OPERATION"),
            TimeoutException => ((int)HttpStatusCode.RequestTimeout, "Request timed out", "TIMEOUT"),
            DbUpdateException => ((int)HttpStatusCode.BadRequest, "Database update failed", "DB_UPDATE"),
            InvalidCastException => ((int)HttpStatusCode.BadRequest, exception.Message, "INVALID_CAST"),
            FormatException => ((int)HttpStatusCode.BadRequest, exception.Message, "FORMAT_ERROR"),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, exception.Message, "NOT_FOUND"),
            AuthenticationFailureException => ((int)HttpStatusCode.Unauthorized, "Authentication failed", "AUTH_FAILED"),
            ValidationException => ((int)HttpStatusCode.BadRequest, exception.Message, "VALIDATION_ERROR"),
            DuplicateNameException => ((int)HttpStatusCode.BadRequest, exception.Message, "DUPLICATE_NAME"),
            PostgresException sqlEx => HandlePostgresException(sqlEx),
            _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred", "INTERNAL_SERVER_ERROR"),
        };
    }

    private static (int statusCode, string title, string errorCode) HandlePostgresException(PostgresException pgEx)
    {
        return pgEx.SqlState switch
        {
            "23505" => ((int)HttpStatusCode.BadRequest, "Duplicate entry, unique constraint violation", "UNIQUE_CONSTRAINT"),
            "22003" => ((int)HttpStatusCode.BadRequest, "Arithmetic overflow error", "ARITHMETIC_OVERFLOW"),
            "23503" => ((int)HttpStatusCode.BadRequest, "Foreign key constraint violation", "FOREIGN_KEY"),
            "40001" => ((int)HttpStatusCode.Conflict, "Deadlock detected", "DEADLOCK"),
            "08003" => ((int)HttpStatusCode.ServiceUnavailable, "Cannot open database", "DB_CONNECTION"),
            "08006" => ((int)HttpStatusCode.ServiceUnavailable, "Cannot connect to the server", "SERVER_CONNECTION"),
            "28P01" => ((int)HttpStatusCode.Unauthorized, "Login failed for user", "DB_AUTH_FAILED"),
            "57014" => ((int)HttpStatusCode.RequestTimeout, "SQL query timeout", "QUERY_TIMEOUT"),
            "23502" => ((int)HttpStatusCode.BadRequest, "Cannot insert NULL value", "NULL_VALUE"),
            "53P01" => ((int)HttpStatusCode.InternalServerError, "Transaction log is full", "LOG_FULL"),
            _ => ((int)HttpStatusCode.InternalServerError, "Database error occurred", "DB_ERROR"),
        };
    }
}