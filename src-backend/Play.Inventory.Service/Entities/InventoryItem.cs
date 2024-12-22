using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Play.Common;

namespace Play.Inventory.Service.Entities;

public class InventoryItem: IEntity
{
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid UsedId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid CatalogItemId { get; set; }
    [BsonRepresentation(BsonType.Double)] 
    public int Quantity { get; set; }
    [BsonRepresentation(BsonType.String)] 
    public DateTimeOffset AdquireDate { get; set; }
    
}