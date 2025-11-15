using Jubo_api.Models.Dto;

namespace Jubo_api.Interfaces.BusinessLogic;

public interface IOrderBusinessLogic
{
    ValueTask<OrderDto> GetAsync(string id);
    ValueTask<bool> UpdateAsync(OrderDto dto);
    ValueTask CreateAsync(OrderDto dto, string userId);
    ValueTask<bool> DeleteAsync(string id);
}