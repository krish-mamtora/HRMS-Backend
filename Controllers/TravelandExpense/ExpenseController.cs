using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.TravelandExpense;
using HRMS_Backend.Services.TravelandExpenses;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.TravelandExpense
{
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
                return Ok(CreateExpense);
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
            var Expense = await _service.GetExpenseByTravelAssignmentId(id);
            if (Expense == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(Expense);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlanExpenseById(int id, [FromBody] ExpenseCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _service.UpdatePlanExpenseById(id, dto);
            if (!result)
            {
                return NotFound("Expense not found");
            }
            return Ok("Expense updated successfully");
        }
    }
}
