using Jubo_api.Interfaces.BusinessLogic;
using jubo_api.Interfaces.Storage;
using Jubo_api.Models.Dto;

namespace Jubo_api.Services.BusinessLogic;

public sealed class PatientsBusinessLogic(
    IPatientsStorage patientsStorage,
    ICounterStorage counterStorage) : IPatientsBusinessLogic
{
    async ValueTask<PatientsDto> IPatientsBusinessLogic.GetAsync(string id)
    {
        return await patientsStorage.GetPatientAsync(id);
    }

    async ValueTask<IEnumerable<PatientsDto>> IPatientsBusinessLogic.GetAllAsync()
    {
        return await patientsStorage.GetPatientsAsync();
    }

    async ValueTask<bool> IPatientsBusinessLogic.UpdateAsync(PatientsDto model)
    {
        if (await patientsStorage.IsExistByNameAsync(model.Name))
        {
            return false;
        }
        await patientsStorage.UpdatePatientAsync(model);
        return true;
    }

    async ValueTask<bool> IPatientsBusinessLogic.DeleteAsync(string id)
    {
        await patientsStorage.DeletePatientAsync(id);
        return true;
    }

    async ValueTask<bool> IPatientsBusinessLogic.CreateAsync(PatientsDto model)
    {
        if (await patientsStorage.IsExistByNameAsync(model.Name))
        {
            return false;
        }

        var maxId = await counterStorage.GetNextSequenceValueAsync("Patients");
        model.Uid = maxId;
        await patientsStorage.CreatePatientAsync(model);
        return true;
    }
}