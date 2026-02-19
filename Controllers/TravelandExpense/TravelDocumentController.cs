using HRMS_Backend.Services.TravelandExpenses;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.TravelandExpense
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelDocumentController : ControllerBase
    {
        private readonly ITravelDocumentsService _service;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public TravelDocumentController(ITravelDocumentsService service, IWebHostEnvironment hostingEnvironment)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet("download-travel-document/{filename}")]
        public async Task<IActionResult> getExpenseProof(string fileName)
        {
            string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "UploadedExpenseProof", fileName);
            if (!System.IO.File.Exists(filePath)) return NotFound();
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
