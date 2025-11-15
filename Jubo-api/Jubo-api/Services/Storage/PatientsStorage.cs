using jubo_api.Interfaces.Storage;
using Jubo_api.Models.Db;
using Jubo_api.Models.Dto;
using Mapster;
using MongoDB.Driver;

namespace Jubo_api.Services.Storage;

public sealed class PatientsStorage(IMongoDatabase database) : IPatientsStorage
{
    private const string CollectionName = "patients";
    private readonly IMongoCollection<PatientsModel> _patientsCollection = database.GetCollection<PatientsModel>(CollectionName);

    async Task IPatientsStorage.CreatePatientAsync(PatientsDto patient)
    {
        await _patientsCollection.InsertOneAsync(patient.Adapt<PatientsModel>());
    }

    async Task IPatientsStorage.UpdatePatientAsync(PatientsDto patient)
    {
        var filter = Builders<PatientsModel>.Filter.Eq(doc => doc.Id, patient.Id);
        var update = Builders<PatientsModel>.Update.Set(doc => doc.Name, patient.Name);

        var result = await _patientsCollection.UpdateOneAsync(filter, update);
        if (result.IsAcknowledged && result.ModifiedCount <= 0)
        {
            throw new Exception("Update patient failed.");
        }
    }

    async Task IPatientsStorage.UpdateOrderAsync(string userId, string orderId)
    {
        var filter = Builders<PatientsModel>.Filter.Eq(doc => doc.Id, userId);
        var update = Builders<PatientsModel>.Update.Set(doc => doc.OrderId, orderId);
        var result = await _patientsCollection.UpdateOneAsync(filter, update);
        if (result.IsAcknowledged && result.ModifiedCount <= 0)
        {
            throw new Exception("Update order failed.");
        }
    }

    async Task IPatientsStorage.DeletePatientAsync(string patientId)
    {
        await _patientsCollection.DeleteOneAsync(x => x.Id == patientId);
    }

    async Task<IEnumerable<PatientsDto>> IPatientsStorage.GetPatientsAsync()
    {
        return (await _patientsCollection.Find(x => true).ToListAsync()).Adapt<IEnumerable<PatientsDto>>();
    }

    async Task<PatientsDto> IPatientsStorage.GetPatientAsync(string patientId)
    {
        return (await _patientsCollection.Find(x => x.Id == patientId).FirstOrDefaultAsync()).Adapt<PatientsDto>();
    }

    async Task<bool> IPatientsStorage.IsExistByNameAsync(string name)
    {
        var filter = Builders<PatientsModel>.Filter.Eq(doc => doc.Name, name);
        return await _patientsCollection.Find(filter).AnyAsync();
    }
}