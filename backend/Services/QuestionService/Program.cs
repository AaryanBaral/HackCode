using Microsoft.AspNetCore.Diagnostics;
using QuestionService.Extensions;
using QuestionService.Kafka;
using QuestionService.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddRepositories(builder.Configuration);



var app = builder.Build();
app.UseCors("AllowAny");
app.UseMiddleware<DbTransactionMiddleware>(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapGet("/", () => "HackCode is working bitchessssss!");
app.UseAuthorization();

app.UseExceptionHandler(handler =>
{
    handler.Run(async context =>
    {
        var exceptionHandler = context.RequestServices.GetRequiredService<IExceptionHandler>();
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (!context.Response.HasStarted && exception != null)
        {
            context.Response.Clear(); // Ensure no partial response has been written
            context.Response.StatusCode = StatusCodes.Status500InternalServerError; // Set appropriate status
            await exceptionHandler.TryHandleAsync(context, exception, context.RequestAborted);
        }
    });
});

await app.Services.InitializeDbAsync();
app.MapControllers();


// ✅ Delay Kafka startup
var lifetime = app.Lifetime;
var kafkaConsumer = app.Services.GetRequiredService<KafkaConsumer>();

lifetime.ApplicationStarted.Register(() =>
{
    Task.Run(async () =>
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Kafka consumer starting after application is ready.");

        try
        {
            await kafkaConsumer.ConsumeAsync(async message =>
            {
                Console.WriteLine($"Handling extra logic for: {message}");
                // You can add more logic here
            }, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kafka consumer crashed.");
        }
    });
});
app.Run();
