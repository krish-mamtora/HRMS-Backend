using HRMS_Backend.Common.Constants;
using HRMS_Backend.Common.Enums;
using HRMS_Backend.Common.Exceptions;
using HRMS_Backend.Common.Responses;
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
using System.Security.Claims;

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

        public EmployeePlanController(
            IEmployeeTravelService service,
            IEmailService emailService,
            MyDbContext context)
        {
            _service = service;
            _emailService = emailService;
            _context = context;
        }

        [Authorize(Roles = Roles.HR)]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>>
            CreateBulkPlan(
                [FromBody] BulkTravelAssignmentDto dto)
        {
            if (dto.EmpId == null || !dto.EmpId.Any())
            {
                throw new BadRequestException(
                    "Employee list cannot be empty");
            }

            await _service
                .CreateBulkUploadTravelPlanAsync(dto);

            var response = ApiResponse<string>
                .SuccessResponse(
                    "Success",
                    "Travel plan assigned successfully",
                    (int)ResponseCode.Created);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }

        [Authorize(Roles = Roles.HR)]
        [HttpPost("notifyTravel")]
        public async Task<ActionResult<ApiResponse<string>>>
            NotifyForTravelPlan(
                [FromBody]
                ShareTravelPlanMailCreateUpdateDto dto)
        {
            var travelPlan = await _context.TravelPlan
                .FindAsync(dto.PId);

            if (travelPlan is null)
            {
                throw new NotFoundException(
                    "Travel plan not found");
            }

            var travelAssignMail = new TravelAssignEmail
            {
                PId = dto.PId,
                ReceiverMail = dto.ReceiverMail,
                EmpId = dto.EmpId,
                Subject = dto.Subject,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow
            };

            _context.TravelAssignEmail
                .Add(travelAssignMail);

            var inAppNotification = new InAppNotification
            {
                EmpId = dto.EmpId,
                Message =
                    $"New Travel Plan Assigned: {travelPlan.Destination}",
                CreatedAt = DateTime.UtcNow
            };

            _context.InAppNotifications
                .Add(inAppNotification);

            await _context.SaveChangesAsync();

            var body = $@"
                <h2>Travel Plan Assigned:
                    {travelPlan.Purpose}
                </h2>

                <p>{dto.Message}</p>

                <br/>

                <p>
                    Destination:
                    {travelPlan.Destination}
                </p>

                <p>
                    Start Date:
                    {travelPlan.StartDate}
                </p>

                <p>
                    End Date:
                    {travelPlan.EndDate}
                </p>

                <p>
                    Trip Type:
                    {travelPlan.TripType}
                </p>

                <h3>
                    Kindly visit portal for more information
                    and upload verification documents.
                </h3>
            ";

            await _emailService.SendEmailAsync(
                dto.ReceiverMail,
                string.IsNullOrWhiteSpace(dto.Subject)
                    ? "Travel Plan Assigned"
                    : dto.Subject,
                body);

            var response = ApiResponse<string>
                .SuccessResponse(
                    "Success",
                    "Email sent successfully",
                    (int)ResponseCode.Success);

            return Ok(response);
        }

        [HttpGet("unread")]
        public async Task<
            ActionResult<
                ApiResponse<List<InAppNotification>>>>
            GetUnreadNotifications()
        {
            var userId = User
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedException(
                    "Invalid user");
            }

            var notifications = await _context
                .InAppNotifications
                .AsNoTracking()
                .Where(x =>
                    x.EmpId == int.Parse(userId) &&
                    !x.IsRead)
                .ToListAsync();

            var response =
                ApiResponse<List<InAppNotification>>
                .SuccessResponse(
                    notifications,
                    "Notifications fetched successfully",
                    (int)ResponseCode.Success);

            return Ok(response);
        }

        [HttpPost("mark-as-read")]
        public async Task<ActionResult<ApiResponse<string>>>
            MarkAsRead(
                [FromBody] NotificationReadDto dto)
        {
            var notifications = await _context
                .InAppNotifications
                .Where(x => dto.Ids.Contains(x.Id))
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            var response = ApiResponse<string>
                .SuccessResponse(
                    "Success",
                    "Notifications marked as read",
                    (int)ResponseCode.Success);

            return Ok(response);
        }

        [HttpGet("plan")]
        public async Task<
            ActionResult<
                ApiResponse<
                    IEnumerable<TravelAssignmentDisplayDto>>>>
            GetAllAssignDetails()
        {
            var plans = await _service
                .GetAllAssignDetailsAsync();

            var response =
                ApiResponse<
                    IEnumerable<TravelAssignmentDisplayDto>>
                .SuccessResponse(
                    plans,
                    "Travel assignments fetched successfully",
                    (int)ResponseCode.Success);

            return Ok(response);
        }

        [HttpGet("plan/{id}")]
        public async Task<
            ActionResult<
                ApiResponse<TravelAssignmentDisplayDto>>>
            GetAssignedTravelPlayById(int id)
        {
            var plan = await _service
                .GetAssignedTravelPlanByIdAsync(id);

            var response =
                ApiResponse<TravelAssignmentDisplayDto>
                .SuccessResponse(
                    plan,
                    "Travel assignment fetched successfully",
                    (int)ResponseCode.Success);

            return Ok(response);
        }

        [HttpGet("employee/{id}")]
        public async Task<
            ActionResult<
                ApiResponse<
                    IEnumerable<TravelAssignmentDisplayDto>>>>
            GetAllAssignedPlansForEmpId(int id)
        {
            var plans = await _service
                .GetAllAssignedPlansForEmpIdAsync(id);

            var response =
                ApiResponse<
                    IEnumerable<TravelAssignmentDisplayDto>>
                .SuccessResponse(
                    plans,
                    "Employee travel plans fetched successfully",
                    (int)ResponseCode.Success);

            return Ok(response);
        }

        [HttpGet("employeeForTravelPlan/{id}")]
        public async Task<
            ActionResult<ApiResponse<List<int>>>>
            GetAllEmployeesAssignedToPlan(int id)
        {
            var employeeIds = await _service
                .GetAllEmployeesAssignedToPlanAsync(id);

            var response = ApiResponse<List<int>>
                .SuccessResponse(
                    employeeIds,
                    "Employees fetched successfully",
                    (int)ResponseCode.Success);

            return Ok(response);
        }
    }
}