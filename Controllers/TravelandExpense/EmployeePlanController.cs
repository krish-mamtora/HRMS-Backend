using HRMS_Backend.Model.TravelandExpense;
using HRMS_Backend.Services.TravelandExpenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Controllers.TravelandExpense
{
    [Authorize]
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
        public async Task<IActionResult> CreateBulkPlan([FromBody] BulkTravelAssignmentDto dto)
        {
            if (dto?.EmpId == null || !dto.EmpId.Any())
            {
                return BadRequest("Employee list cannot be empty");
            }
            try
            {
                bool result = await _service.createBulkUploadTravelPlan(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("plan/", Name = "getAllAssignDetails")]
        public async Task<IActionResult> getAllAssignDetails()
        {
            var travelPlans = await  _service.getAllAssignDetails();
            if (travelPlans == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(travelPlans);
        }

        [HttpGet("plan/{id}", Name = "getAssignedTravelPlayById")]
        public async Task<IActionResult> getAssignedTravelPlayById(int id)
        {
            var travelPlans =await  _service.getAssignedTravelPlayById(id);
            if (travelPlans == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(travelPlans);
        }

        [HttpGet("employee/{id}", Name = "getAllAssignedPlansForEmpId")]
        public async Task<IActionResult> getAllAssignedPlansForEmpId(int id)
        {
            var plans = await _service.getAllAssignedPlansForEmpId(id);
            if (plans == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(plans);
        }


        [HttpGet("employeeForTravelPlan/{id}", Name = "getEmployeesForTravelPlan")]
        public async Task<ActionResult<List<int>>> getAllEmployeesAssignedToPlan(int id)
        {
            var employeeIds = await _service.getAllEmployeesAssignedToPlan(id);

            if (employeeIds == null || employeeIds.Count == 0)
            {
                return NotFound($"No employees found for plan ID {id}.");
            }

            return Ok(employeeIds);
        }
    }
}
