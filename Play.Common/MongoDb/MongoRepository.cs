using System.Linq.Expressions;
using MongoDB.Driver;

namespace Play.Common.MongoDb;

public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> dbCollection;
    private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbCollection.Find(predicate).ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var filter = filterBuilder.Eq(x => x.Id, id);
        return await dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbCollection.Find(predicate).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
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