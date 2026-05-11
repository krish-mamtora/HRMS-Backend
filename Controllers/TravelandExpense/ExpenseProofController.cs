using HRMS_Backend.Common.Constants;
using HRMS_Backend.Common.Enums;
using HRMS_Backend.Common.Exceptions;
using HRMS_Backend.Common.Responses;
using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.TravelandExpense;
using HRMS_Backend.Services.TravelandExpenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.TravelandExpense
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseProofController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IExpenseProofService _service;

        public ExpenseProofController(
            IWebHostEnvironment hostingEnvironment,
            IExpenseProofService service)
        {
            _hostingEnvironment = hostingEnvironment;
            _service = service;
        }

        [HttpGet("download-expense-proof/{fileName}")]
        public async Task<IActionResult>
            GetExpenseProof(string fileName)
        {
            var filePath = Path.Combine(
                _hostingEnvironment.ContentRootPath,
                "UploadedExpenseProof",
                fileName);

            if (!System.IO.File.Exists(filePath))
            {
                throw new NotFoundException(
                    "Expense proof file not found");
            }

            var fileBytes =
                await System.IO.File.ReadAllBytesAsync(
                    filePath);

            return File(
                fileBytes,
                "application/pdf",
                fileName);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Employee)]
        public async Task<
            ActionResult<ApiResponse<ExpenseProofDisplayDto>>>
            CreateExpenseProofAsync(
                [FromForm]
                ExpenseProofCreateUpdateDto dto)
        {
            if (dto.ProofDocument == null)
            {
                throw new BadRequestException(
                    "Proof document is required");
            }

            var allowedExtensions =
                new[] { ".pdf", ".doc", ".docx" };

            var extension = Path
                .GetExtension(dto.ProofDocument.FileName)
                .ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension) ||
                !allowedExtensions.Contains(extension))
            {
                throw new BadRequestException(
                    "Invalid file type. Only PDF and Word documents are allowed");
            }

            var expenseProof = await _service
                .CreateExpenseProofAsync(dto);

            var response =
                ApiResponse<ExpenseProof>
                .SuccessResponse(
                    expenseProof,
                    "Expense proof uploaded successfully",
                    (int)ResponseCode.Created);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }

        [HttpGet("getExpenseProofById/{id}")]
        public async Task<
            ActionResult<
                ApiResponse<ExpenseProofDisplayDto>>>
            GetExpenseProofById(int id)
        {
            var expenseProof = await _service
                .GetExpenseProofByIdAsync(id);

            var response =
                ApiResponse<ExpenseProofDisplayDto>
                .SuccessResponse(
                    expenseProof,
                    "Expense proof fetched successfully",
                    (int)ResponseCode.Success);

            return Ok(response);
        }

        [HttpGet("getExpenseProofForExpenseid/{id}")]
        public async Task<
            ActionResult<
                ApiResponse<
                    IEnumerable<ExpenseProofDisplayDto>>>>
            GetExpenseProofByExpenseId(int id)
        {
            var expenseProofs = await _service
                .GetExpenseProofByExpenseIdAsync(id);

            var response =
                ApiResponse<
                    IEnumerable<ExpenseProofDisplayDto>>
                .SuccessResponse(
                    expenseProofs,
                    "Expense proofs fetched successfully",
                    (int)ResponseCode.Success);

            return Ok(response);
        }
    }
}