using HRMS_Backend.Model.JobListing;
using HRMS_Backend.Model.TravelandExpense;
using HRMS_Backend.Services.JobListing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HRMS_Backend.Controllers.JobListing
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class ReferalController : ControllerBase
    {
        private readonly IReferService _service;
        public ReferalController(IReferService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult?> CreateReferalAsync([FromForm] JobRefferalCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string uniqueFileName = string.Empty;
            if (dto.ReffResume != null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var extension = Path.GetExtension(dto.ReffResume.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid file type. Only PDF and Word documents are allowed.");
                }
            }
            var createjob = await _service.createReferalAsync(dto);
            return CreatedAtAction(nameof(getReferalById), new { id = createjob.Id }, createjob);
        }


        [HttpGet("{id}", Name = "getReferalById")]
        public async Task<IActionResult> getReferalById(int id)
        {
            var referal = await _service.getReferalById(id);
            if (referal == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(referal);
        }

        [HttpGet("job/{id}", Name = "getReferalByJobId")]
        public async Task<IActionResult> getReferalByJobId(int id)
        {
            var referal = await  _service.getReferalByJobId(id);
            if (referal == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(referal);
        }

        [HttpGet("user/{id}", Name = "getReferalByUserId")]
        public async Task<IActionResult> getReferalByUserId(int id)
        {
            var referal =await  _service.getReferalByUserId(id);
            if (referal == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(referal);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReferalById(int id, [FromBody] JobRefferalCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _service.UpdateReferalWithId(id, dto);
            if (!result)
            {
                return NotFound("Plan not found");
            }
            return Ok("Plan updated successfully");
        }
    }
}
