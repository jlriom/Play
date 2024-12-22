using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Endpoints;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddMongo()
    .AddMongoRepository<InventoryItem>("inventoryItems")
    .AddMongoRepository<CatalogItem>("catalogItems");

builder.Services
    .AddMassTransitWithRabbitMq();

AddCatalogClient(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapItemsEndpoints();

app.Run();

void AddCatalogClient(WebApplicationBuilder webApplicationBuilder)
{
    var jitterer = new Random();

    webApplicationBuilder.Services.AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5000");
        })
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5, 
            retryAttempt =>  TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
            onRetry: (outcome, timespan, retryAttempt) =>
            {
                var serviceProvider = webApplicationBuilder.Services.BuildServiceProvider();
                serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning(
                    "Delaying for {totalSeconds} seconds, then making retry {retryAttempt}", timespan.TotalSeconds,
                    retryAttempt);
            }))
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            5, 
            TimeSpan.FromSeconds(30),
            onBreak: (outcome, timespan) =>
            {
                var serviceProvider = webApplicationBuilder.Services.BuildServiceProvider();
                serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning(
                    "Opening the circuit for {totalSeconds} seconds...", timespan.TotalSeconds);
            },
            onReset: () =>
            {
                var serviceProvider = webApplicationBuilder.Services.BuildServiceProvider();
                serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning(
                    "Closing the circuit...");
            }
        ))
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}
