using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace UserService.ExceptionHandling
{

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
                SqlException sqlEx => HandlePostgresException(sqlEx),
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred", "INTERNAL_SERVER_ERROR"),
            };
        }

        private static (int statusCode, string title, string errorCode) HandlePostgresException(SqlException sqlEx)
        {
            return sqlEx.Number switch
            {
                2627 => ((int)HttpStatusCode.BadRequest, "Duplicate entry, unique constraint violation", "UNIQUE_CONSTRAINT"), // Violation of unique index
                547 => ((int)HttpStatusCode.BadRequest, "Foreign key constraint violation", "FOREIGN_KEY"), // Constraint check violation
                2601 => ((int)HttpStatusCode.BadRequest, "Cannot insert duplicate key row", "DUPLICATE_KEY"), // Unique index constraint
                1205 => ((int)HttpStatusCode.Conflict, "Deadlock detected", "DEADLOCK"), // Deadlock victim
                4060 => ((int)HttpStatusCode.ServiceUnavailable, "Cannot open database requested by the login", "DB_CONNECTION"), // Cannot open database
                18456 => ((int)HttpStatusCode.Unauthorized, "Login failed for user", "DB_AUTH_FAILED"), // Login failed
                233 => ((int)HttpStatusCode.ServiceUnavailable, "Connection to SQL Server failed", "SERVER_CONNECTION"), // Connection issues
                -2 => ((int)HttpStatusCode.RequestTimeout, "SQL query timeout", "QUERY_TIMEOUT"), // Timeout expired
                515 => ((int)HttpStatusCode.BadRequest, "Cannot insert NULL value", "NULL_VALUE"), // Cannot insert NULL
                9002 => ((int)HttpStatusCode.InternalServerError, "Transaction log is full", "LOG_FULL"), // Log file full
                _ => ((int)HttpStatusCode.InternalServerError, "Database error occurred", "DB_ERROR"), // Default case
            };
        }
    }
}