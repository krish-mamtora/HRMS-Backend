using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.TravelandExpense;
using HRMS_Backend.Services.TravelandExpenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.TravelandExpense
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly ITravelExpenseService _service;

        public ExpenseController(ITravelExpenseService service)
        {
            _service = service;
        }
       
        [HttpPost]
        public async Task<ActionResult> CreateTravelExpense([FromBody] ExpenseCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var CreateExpense = await _service.CreateTravelExpenseAsync(dto);
                return CreatedAtAction(nameof(getExpenseById), new { id = CreateExpense.Id }, CreateExpense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("{id}", Name = "getExpenseById")]
        public async Task<IActionResult> getExpenseById(int id)
        {
            var Expense = await _service.GetExpenseByIdAsync(id);
            if (Expense == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(Expense);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Expenses = await _service.GetAllExpenseAsync();
            if (Expenses == null || !Expenses.Any())
            {
                return NotFound("No Explense found");
            }
            return Ok(Expenses);
        }
        
        [HttpGet("travelassign/{id}", Name = "GetExpenseByTravelAssignmentId")]
        public async Task<IActionResult> GetExpenseByTravelAssignmentId(int id)
        {
            var Expense = await _service.getExpensesByTravelAssignedId(id);
            if (Expense == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(Expense);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdatePlanExpenseById(int id, [FromBody] ExpenseCreateUpdateDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var result = await _service.UpdatePlanExpenseById(id, dto);
        //    if (!result)
        //    {
        //        return NotFound("Expense not found");
        //    }
        //    return Ok("Expense updated successfully");
        //}

        [HttpGet("getId")]
        public async Task<IActionResult> GetAssignPlanIdfromEmpIdandPId([FromQuery(Name = "EmpId")] int EmpId, [FromQuery(Name = "PId")] int PId)

        {
            var rowId = await _service.GetIdfromEmpIDandPID(EmpId,PId);
            if(rowId == null)
            {
                return NotFound(new { message = "No record found for the given EmpId and PId." });
            }
            return Ok(rowId);
        }

        [HttpGet("getExpensesByTravelAssignedId/{id}", Name = "getExpensesByTravelAssignedId")]

        public async Task<IActionResult> getExpensesByTravelAssignedId(int id)
        {
            var response = await _service.getExpensesByTravelAssignedId(id);
            if (response == null)
            {
                return NotFound(new { message = "No record found for the given Travel AssignId." });
            }
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpenseByIdAsync([FromBody] ExpenseCreateUpdateDto dto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _service.UpdateExpenseByIdAsync(dto , id);
            if (!result)
            {
                return NotFound("Expense not found");
            }
            return Ok("Expense updated successfully");
        }
    }
}
