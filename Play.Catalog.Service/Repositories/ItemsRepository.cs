using MongoDB.Bson;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public class ItemsRepository
{
    private const string collectionName = "items";
    private readonly IMongoCollection<Item> dbCollection;
    private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

    public ItemsRepository()
    {
       
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("Catalog");

        dbCollection = database.GetCollection<Item>(collectionName);
    }
    
    public async Task<IReadOnlyCollection<Item>> GetAllAsync()
    {
        return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
    }

    public async Task<Item> GetByIdAsync(Guid id)
    {
        var filter = filterBuilder.Eq(x => x.Id, id);
        return await dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Item entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await dbCollection.InsertOneAsync( entity);
    }
    
    public async Task UpdateAsync(Item entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var filter = filterBuilder.Eq(x => x.Id, entity.Id);
        await dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var filter = filterBuilder.Eq(x => x.Id, id);
        await dbCollection.DeleteOneAsync(filter);
    }
}