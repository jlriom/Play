using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using Play.Common;
using Play.Common.MongoDb;

namespace Play.Catalog.Service.Repositories;

public static class Extensions
{
    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
        where T : IEntity
    {
        services.AddScoped<IRepository<Item>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<Item>(database!, collectionName);
        });
        return services;
    }
}