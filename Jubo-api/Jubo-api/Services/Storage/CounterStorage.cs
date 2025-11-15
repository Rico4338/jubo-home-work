using jubo_api.Interfaces.Storage;
using Jubo_api.Models.Db;
using MongoDB.Driver;

namespace Jubo_api.Services.Storage;

public sealed class CounterStorage(IMongoDatabase database) : ICounterStorage
{
    private const string CollectionName = "counter";
    private readonly IMongoCollection<CounterModel> _counterCollection = database.GetCollection<CounterModel>(CollectionName);

    async ValueTask<int> ICounterStorage.GetNextSequenceValueAsync(string name)
    {
        var filter = Builders<CounterModel>.Filter.Eq(c => c.Name, name);
        var update = Builders<CounterModel>.Update.Inc(c => c.Max, 1);
        var options = new FindOneAndUpdateOptions<CounterModel>
        {
            IsUpsert = true, // Create if not exists
            ReturnDocument = ReturnDocument.After // Return the updated document
        };

        var counter = await _counterCollection.FindOneAndUpdateAsync(filter, update, options);
        return counter.Max;
    }
}