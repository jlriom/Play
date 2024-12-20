using Play.Catalog.Service.Endpoints;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;
using Play.Common.MongoDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddMongo()
    .AddMongoRepository<Item>("items");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapItemsEndpoints();

app.Run();