
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;


namespace QuestionService.Middleware
{
    public class DbTransactionMiddleware
    {
        private readonly RequestDelegate _next;
    private readonly ILogger<DbTransactionMiddleware> _logger;

    public DbTransactionMiddleware(RequestDelegate next, ILogger<DbTransactionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var dbContext = httpContext.RequestServices.GetRequiredService<AppDbContext>();
        using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await _next(httpContext);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            // Log the exception
            _logger.LogError($"An error occurred while processing the request: {ex.Message}");

            throw; 
        }
    }
    }
}