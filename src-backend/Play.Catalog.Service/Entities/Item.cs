using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Play.Common;

namespace Play.Catalog.Service.Entities;

public class Item : IEntity
{
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    [BsonRepresentation(BsonType.Double)] public decimal Price { get; set; }
    [BsonRepresentation(BsonType.String)] public DateTimeOffset CreatedDate { get; set; }
}