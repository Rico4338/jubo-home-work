using Jubo_api.Interfaces.BusinessLogic;
using Jubo_api.Models.Dto;
using Jubo_api.Models.Request.Order;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Jubo_api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class OrderController(IOrderBusinessLogic orderBusinessLogic) : Controller
{
    [HttpGet("{id}")]
    public async Task<IActionResult> List(string id)
    {
        return Json(await orderBusinessLogic.GetAsync(id));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PutOrderRequest request)
    {
        var updateModel = request.Adapt<OrderDto>();
        if (await orderBusinessLogic.UpdateAsync(updateModel))
        {
            return NoContent();
        }

        return Problem();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostOrderRequest request)
    {
        await orderBusinessLogic.CreateAsync(request.Adapt<OrderDto>(), request.UserId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (await orderBusinessLogic.DeleteAsync(id))
        {
            return NoContent();
        }

        return Problem();
    }
}