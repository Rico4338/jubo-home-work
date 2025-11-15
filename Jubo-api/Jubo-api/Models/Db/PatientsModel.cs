using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Jubo_api.Models.Db;

public sealed class PatientsModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public int Uid { get; set; }
    public string Name { get; set; }
    public string? OrderId { get; set; }
}
