using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Play.Catalog.Service.Entities;

public class Item
{
    [BsonId(IdGenerator = typeof(GuidGenerator)), BsonRepresentation(BsonType.String)]  
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [BsonRepresentation(BsonType.Double)]
    public decimal Price { get; set; }
    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset CreatedDate { get; set; }
}