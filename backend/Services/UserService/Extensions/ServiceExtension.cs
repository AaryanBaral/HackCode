

using UserService.Config;
using UserService.ExceptionHandling;
using UserService.Kafka;
using UserService.Kafka.Consumer;
using UserService.Kafka.Producer;

namespace UserService.Extensions
{
    public static class ServiceExtension
    {

        public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddCorsConfiguration();
            services.AddExceptionHandler<GlobalExceptionHandling>();
            services.AddKafkaService(configuration);


        }

        private static void AddKafkaService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<KafkaConsumerHostedService>();

            //  adding the kafka config to the di and mapping it to the section of the appsetting.json
            //  so that it can be accessed using Ioption anywahere over the app
            services.Configure<KafkaConfig>(configuration.GetSection("kafka"));

            services.AddSingleton<KafkaProducer>();
            services.AddSingleton<KafkaConsumer>();
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