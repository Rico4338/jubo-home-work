using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Jubo_api.Models.Db;

public sealed class OrderModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public int Uid { get; set; }
    public string Message { get; set; }
}