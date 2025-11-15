using Jubo_api.Models.Dto;

namespace jubo_api.Interfaces.Storage;

public interface IOrderStorage
{
    Task<OrderDto> GetAsync(string id);
    ValueTask UpdateAsync(OrderDto dto);
    ValueTask<string> CreateAsync(OrderDto dto);
    ValueTask DeleteAsync(string id);
}