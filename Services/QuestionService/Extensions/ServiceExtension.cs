using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuestionService.Configurations;
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
            services.AddJwtAuthentication(configuration);
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
        private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("JwtConfig").Get<JwtConfig>() ?? throw new NullReferenceException("JwtScret is null");
            var secret = config.SecretKey;
            if (string.IsNullOrEmpty(secret))
            {
                throw new InvalidOperationException("JWT secret is missing in configuration");
            }
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenValidationParameters = new TokenValidationParameters()
            {
                //used to validate token using different options
                IssuerSigningKey = new SymmetricSecurityKey(key), // we compare if it matches our key or not
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config.Issuer, 
                ValidAudience = config.Audience,
            };

            // Add the Authentication scheme and configurations
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                // Add the jwt configurations as what should be done and how to do it
                .AddJwtBearer(jwt =>
                {
                    jwt.SaveToken = true; // saves the generated token to http context
                    jwt.TokenValidationParameters = tokenValidationParameters;
                    jwt.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.Headers.Append("Token-Expired", "true");
                            }

                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            var response = new
                            {
                                success = false,
                                message = context.Exception.Message
                            };

                            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            var response = new
                            {
                                success = false,
                                message = "You are not authorized."
                            };

                            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";

                            var response = new
                            {
                                success = false,
                                message = "You do not have permission to access this resource."
                            };

                            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        }
                    };
                });
            services.AddSingleton(tokenValidationParameters);
        }
    }
}