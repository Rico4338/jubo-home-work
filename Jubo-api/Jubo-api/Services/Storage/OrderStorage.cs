using jubo_api.Interfaces.Storage;
using Jubo_api.Models.Db;
using Jubo_api.Models.Dto;
using Mapster;
using MongoDB.Driver;

namespace Jubo_api.Services.Storage;

public sealed class OrderStorage(IMongoDatabase database) : IOrderStorage
{
    private const string CollectionName = "orders";
    private readonly IMongoCollection<OrderModel> _orderCollection = database.GetCollection<OrderModel>(CollectionName);

    async Task<OrderDto> IOrderStorage.GetAsync(string id)
    {
        return (await _orderCollection.Find(x => x.Id == id).FirstOrDefaultAsync()).Adapt<OrderDto>();
    }

    async ValueTask IOrderStorage.UpdateAsync(OrderDto dto)
    {
        var filter = Builders<OrderModel>.Filter.Eq(doc => doc.Id, dto.Id);
        var update = Builders<OrderModel>.Update.Set(doc => doc.Message, dto.Message);
        var result = await _orderCollection.UpdateOneAsync(filter, update);
        if (result.IsAcknowledged && result.ModifiedCount <= 0)
        {
            throw new Exception("Update order failed.");
        }
    }

    async ValueTask<string> IOrderStorage.CreateAsync(OrderDto dto)
    {
        var insertData = dto.Adapt<OrderModel>();
        await _orderCollection.InsertOneAsync(insertData);
        return insertData.Id;
    }

    async ValueTask IOrderStorage.DeleteAsync(string id)
    {
        await _orderCollection.DeleteOneAsync(x => x.Id == id);
    }
}