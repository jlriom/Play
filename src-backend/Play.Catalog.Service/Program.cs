using Play.Catalog.Service.Endpoints;
using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddMongo()
    .AddMongoRepository<Item>("items");

builder.Services
    .AddMassTransitWithRabbitMq();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(corsBuilder => corsBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapItemsEndpoints();

app.Run();