using Jubo_api.Interfaces.BusinessLogic;
using Jubo_api.Models.Dto;
using jubo_api.Models.Request.Patients;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Jubo_api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class PatientsController(IPatientsBusinessLogic patientsBusinessLogic) : Controller
{
    [HttpGet("")]
    [HttpGet("{id}")]
    public async Task<IActionResult> List(string? id = null)
    {
        return id == null ? Json(await patientsBusinessLogic.GetAllAsync()) : Json(await patientsBusinessLogic.GetAsync(id));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PutPatientsRequest request)
    {
        var updateModel = request.Adapt<PatientsDto>();
        if (await patientsBusinessLogic.UpdateAsync(updateModel))
        {
            return NoContent();
        }

        return Problem();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostPatientsRequest request)
    {
        var createModel = request.Adapt<PatientsDto>();
        await patientsBusinessLogic.CreateAsync(createModel);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (await patientsBusinessLogic.DeleteAsync(id))
        {
            return NoContent();
        }

        return Problem();
    }
}