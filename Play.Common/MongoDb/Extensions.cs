using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Play.Catalog.Service.Settings;
using Play.Common.Settings;

namespace Play.Common.MongoDb;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            var mongoClient = new MongoClient(mongoDbSettings?.ConnectionString);
            return mongoClient.GetDatabase(serviceSettings?.ServiceName);
        });
        return services;
    }


}