using QuestionService.ExceptionHandling;
using QuestionService.Repositories;

namespace QuestionService.Extensions
{
    public static class ServiceExtension
    {

        public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddCorsConfiguration();
            services.AddExceptionHandler<GlobalExceptionHandling>();
            services.AddRepositories();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IQuestionRepository, QuestionRepository>();
        }

        private static void AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {

                options.AddPolicy("AllowAny", builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "http://localhost:3001")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
        }
    }
}