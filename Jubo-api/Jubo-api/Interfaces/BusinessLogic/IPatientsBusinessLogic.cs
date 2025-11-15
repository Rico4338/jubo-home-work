using Jubo_api.Models.Dto;

namespace Jubo_api.Interfaces.BusinessLogic;

public interface IPatientsBusinessLogic
{
    ValueTask<PatientsDto> GetAsync(string id);
    ValueTask<IEnumerable<PatientsDto>> GetAllAsync();
    ValueTask<bool> UpdateAsync(PatientsDto model);
    ValueTask<bool> DeleteAsync(string id);
    ValueTask<bool> CreateAsync(PatientsDto model);
}