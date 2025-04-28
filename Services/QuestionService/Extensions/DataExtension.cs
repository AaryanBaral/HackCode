
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;

namespace QuestionService.Extensions
{
    public static class DataExtension
    {
        public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        public static IServiceCollection AddRepositories(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var postgresConnString = configuration.GetConnectionString("PostgresConnection");
            Console.WriteLine(postgresConnString);
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseNpgsql(postgresConnString, npgsqlOptions =>
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(60),
                            errorCodesToAdd: null
                        )
                )
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging(); // Enable sensitive data logging here
            });



            return services;
        }
    }
}