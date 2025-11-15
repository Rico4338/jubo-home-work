using Jubo_api.Models.Dto;

namespace jubo_api.Interfaces.Storage;

public interface IPatientsStorage
{
    Task CreatePatientAsync(PatientsDto patient);
    Task UpdatePatientAsync(PatientsDto patient);
    Task UpdateOrderAsync(string userId, string orderId);
    Task DeletePatientAsync(string patientId);
    Task<IEnumerable<PatientsDto>> GetPatientsAsync();
    Task<PatientsDto> GetPatientAsync(string patientId);
    Task<bool> IsExistByNameAsync(string name);
}