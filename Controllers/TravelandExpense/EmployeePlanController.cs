using HRMS_Backend.Model.TravelandExpense;
using HRMS_Backend.Services.TravelandExpenses;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.TravelandExpense
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeePlanController : ControllerBase
    {
        private readonly IEmployeeTravelService _service;

        public EmployeePlanController(IEmployeeTravelService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<bool> CreateBulkPlan([FromBody] BulkTravelAssignmentDto dto)
        {
            if (dto?.EmpId == null || !dto.EmpId.Any())
            { return false; }

            bool result = await _service.createBulkUploadTravelPlan(dto);
            return result;
        }

        [HttpGet("plan/", Name = "getAllAssignDetails")]
        public async Task<IActionResult> getAllAssignDetails()
        {
            var travelPlans = _service.getAllAssignDetails();
            if (travelPlans == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(travelPlans);
        }

        [HttpGet("plan/{id}", Name = "getAssignedTravelPlayById")]
        public async Task<IActionResult> getAssignedTravelPlayById(int id)
        {
            var travelPlans = _service.getAssignedTravelPlayById(id);
            if (travelPlans == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(travelPlans);
        }

        [HttpGet("employee/{id}", Name = "getAllAssignedPlansForEmpId")]
        public async Task<IActionResult> getAllAssignedPlansForEmpId(int id)
        {
            var plans = _service.getAllAssignedPlansForEmpId(id);
            if (plans == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(plans);
        }


    }
}
