using Jubo_api.Interfaces.BusinessLogic;
using jubo_api.Interfaces.Storage;
using Jubo_api.Models.Dto;

namespace Jubo_api.Services.BusinessLogic;

public sealed class OrderBusinessLogic(
    IOrderStorage orderStorage,
    IPatientsStorage patientsStorage,
    ICounterStorage counterStorage) : IOrderBusinessLogic
{
    async ValueTask<OrderDto> IOrderBusinessLogic.GetAsync(string id)
    {
        return await orderStorage.GetAsync(id);
    }

    async ValueTask<bool> IOrderBusinessLogic.UpdateAsync(OrderDto dto)
    {
        await orderStorage.UpdateAsync(dto);
        return true;
    }

    async ValueTask IOrderBusinessLogic.CreateAsync(OrderDto dto, string userId)
    {
        var userInfo = await patientsStorage.GetPatientAsync(userId);
        if (userInfo == null)
        {
            throw new Exception("patients not found");
        }

        if (userInfo.OrderId != null)
        {
            throw new Exception("patients order exists");
        }

        var maxId = await counterStorage.GetNextSequenceValueAsync($"order");
        dto.Uid = maxId;
        var orderId = await orderStorage.CreateAsync(dto);
        await patientsStorage.UpdateOrderAsync(userId, orderId);
    }

    async ValueTask<bool> IOrderBusinessLogic.DeleteAsync(string id)
    {
        await orderStorage.DeleteAsync(id);
        return true;
    }
}