using HRMS_Backend.Data;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.JobListing;
using HRMS_Backend.Model.TravelandExpense;
using HRMS_Backend.Services.Email;
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
        private readonly IEmailService _emailService;
        private readonly MyDbContext _context;
        public EmployeePlanController(IEmployeeTravelService service , IEmailService emailservice , MyDbContext context)
        {
            _service = service;
            _context = context;
            _emailService = emailservice;
        }
        [Authorize(Roles = "HR")]
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
        [Authorize(Roles = "HR")]
        [HttpPost("notifyTravel/", Name = "NotifyForTravelPlan")]

        public async Task<IActionResult> NotifyForTravelPlan([FromBody] ShareTravelPlanMailCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var travelplan = await _context.TravelPlan.FindAsync(dto.PId);
            if (travelplan == null)
            {
                return NotFound("Travel Plan Not Found");
            }

            var travelassignmail = new TravelAssignEmail
            {
                PId = dto.PId,
                ReceiverMail = dto.ReceiverMail,
                EmpId = dto.EmpId,
                Subject = dto.Subject,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow,
            };

            _context.TravelAssignEmail.Add(travelassignmail);
            await _context.SaveChangesAsync();

            var body = $@"
                  <h2> Travel Plan Assigned: {travelplan.Purpose}</h2>
                   <p>{dto.Message}</p>   
                        <br/>
                        <p>Experiance Required {travelplan.Destination}</p>
                          <p>Start Date  :  {travelplan.StartDate}</p>
                          <p>End Date :  {travelplan.EndDate}</p>
                          <p>Trip Type :  {travelplan.TripType}</p>
                              <h3>Kindly Visit portal for more information and upload varification documents ...<h3/>
            ";
            await _emailService.SendEmailAsync(
                dto.ReceiverMail,
                string.IsNullOrEmpty(dto.Subject) ? "Travel Plan Assigned" : dto.Subject,
                body
            );

            return Ok(new { message = "Email send successfully!" });
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
