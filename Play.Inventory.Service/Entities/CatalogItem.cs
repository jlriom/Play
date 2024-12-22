using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Play.Common;

namespace Play.Inventory.Service.Entities;

public class CatalogItem: IEntity
{
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}