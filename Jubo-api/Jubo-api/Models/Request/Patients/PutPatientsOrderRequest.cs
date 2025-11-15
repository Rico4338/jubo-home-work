using Jubo_api.Models.Dto;

namespace jubo_api.Models.Request.Patients;

public sealed class PutPatientsOrderRequest
{
    public string Id { get; set; }
    public IEnumerable<OrderDto> Orders { get; set; }
}