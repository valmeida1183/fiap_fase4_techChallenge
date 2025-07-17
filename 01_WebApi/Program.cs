using Infraestructure.DI;
using Prometheus;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.ConfigureLogger();
builder.ConfigureControllers();
builder.ConfigureHttpClient();
builder.ConfigureSwagger();
builder.ConfigureServices();
builder.ConfigurePersistanceApiUrls();

// Add Message Broker Configuration.
builder.Services.ConfigureMassTransit(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMetricServer();
app.UseHttpMetrics();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
