using HRMS_Backend.Services.TravelandExpenses;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.TravelandExpense
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeePlanController : ControllerBase
    {
        private readonly IEmployeeTravelService _service;

        public EmployeePlanController(IEmployeeTravelService service)
        {
            _service = service;
        }

    }
}
