
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Net;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace ConstrainService.ExceptionHandling;
public class GlobalException : IExceptionHandler
{
    private readonly ILogger<GlobalException> _logger;

    public GlobalException(ILogger<GlobalException> logger)
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
            MySqlException sqlEx => HandleMySqlException(sqlEx), // Direct handling of SqlException
            _ => ((int)HttpStatusCode.InternalServerError, exception.Message),
        };
    }

private static (int statusCode, string title) HandleMySqlException(MySqlException mySqlEx)
{
    return mySqlEx.Number switch
    {
        1062 => ((int)HttpStatusCode.BadRequest, "Duplicate entry, unique constraint violation."),
        1216 or 1217 => ((int)HttpStatusCode.BadRequest, "Foreign key constraint violation."),
        1205 => ((int)HttpStatusCode.InternalServerError, "Lock wait timeout exceeded."),
        1049 => ((int)HttpStatusCode.InternalServerError, "Unknown database."),
        1045 => ((int)HttpStatusCode.Unauthorized, "Access denied for user."),
        2002 => ((int)HttpStatusCode.InternalServerError, "Can't connect to MySQL server."),
        1048 => ((int)HttpStatusCode.BadRequest, "Column cannot be null."),
        1153 => ((int)HttpStatusCode.RequestEntityTooLarge, "Packet too large."),
        1206 => ((int)HttpStatusCode.ServiceUnavailable, "Too many connections."),
        1317 => ((int)HttpStatusCode.RequestTimeout, "Query execution was interrupted."),
        1064 => ((int)HttpStatusCode.BadRequest, "SQL syntax error."),
        _ => ((int)HttpStatusCode.InternalServerError, "MySQL Error: " + mySqlEx.Message),
    };
}
}
