using Application.Service;
using Application.Service.Interface;
using Core.Entity;
using Core.Repository.Interface;
using Infraestructure.Repository;
using Polly;
using Polly.Extensions.Http;
using System.Text.Json.Serialization;

namespace WebApi.Extensions;

public static class WebApiConfigurationExtension
{
    public static void ConfigureLogger(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();        
    }

    public static void ConfigureControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        

        builder.Services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            })
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            });
    }

    public static void ConfigureHttpClient(this WebApplicationBuilder builder)
    {
        var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger<Program>();
        var sharedCircuitBreakerPolicy = GetCircuitBreakerPolicy(logger);
        builder.Services.AddSingleton(sharedCircuitBreakerPolicy);

        builder.Services.AddHttpClient("PersistanceClientApi", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["PersistanceApiUrls:Http"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(GetCircuitBreakerPolicy(logger));
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IContactService, ContactService>();
        builder.Services.AddScoped<IDirectDistanceDialingService, DirectDistanceDialingService>();

        builder.Services.AddScoped<IContactHttpRepository, ContactHttpRepository>();
        builder.Services.AddScoped<IDirectDistanceDialingHttpRepository, DirectDistanceDialingHttpRepository>();        
    }    

    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void ConfigurePersistanceApiUrls(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<PersistanceApiUrlsOptions>(
            builder.Configuration.GetSection("PersistanceApiUrls"));
    }

    //private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
    //{
    //    return HttpPolicyExtensions
    //        .HandleTransientHttpError()
    //        .WaitAndRetryAsync(3,
    //            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
    //            onRetry: (outcome, timespan, retryAttempt, context) =>
    //            {
    //                logger.LogWarning($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds due to: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
    //            });
    //}

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, breakDelay) =>
                {
                    logger.LogWarning($"Circuit broken for {breakDelay.TotalSeconds} seconds due to: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                },
                onReset: () => logger.LogInformation("Circuit closed. Requests flow normally."),
                onHalfOpen: () => logger.LogInformation("Circuit is half-open. Testing health.")
            );
    }

    //private static IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy(ILogger logger)
    //{
    //    return Policy<HttpResponseMessage>
    //    .Handle<BrokenCircuitException>()
    //    .OrResult(r => !r.IsSuccessStatusCode) // handle 4xx, 5xx
    //    .FallbackAsync(
    //        fallbackAction: (ct) =>
    //        {
    //            logger.LogError("Fallback triggered. Returning fallback response.");
    //            var fallbackResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
    //            {
    //                Content = new StringContent("Fallback response due to circuit breaker or network failure.")
    //            };
    //            return Task.FromResult(fallbackResponse);
    //        });

    //}

    //private static IAsyncPolicy<HttpResponseMessage> GetResiliencePolicy(IServiceProvider services, HttpRequestMessage requestMessage)
    //{
    //    var logger = services.GetRequiredService<ILogger<Program>>();

    //    //var retryPolicy = GetRetryPolicy(logger);
    //    //var circuitBreakerPolicy = GetCircuitBreakerPolicy(logger);
    //    var fallbackPolicy = GetFallbackPolicy(logger);

    //    return Policy.WrapAsync(fallbackPolicy, circuitBreakerPolicy, retryPolicy);
    //}
}
