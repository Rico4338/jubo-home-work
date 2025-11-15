using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Jubo_api.Models.Db;

public sealed class CounterModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; }
    public int Max { get; set; }
}