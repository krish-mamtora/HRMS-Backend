using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Model.JobListing;
using HRMS_Backend.Services.JobListing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.JobListing
{
    [Route("api/[controller]")]
    [ApiController]
    public class jobListingController : ControllerBase
    {
        private readonly IJobService service;

        public jobListingController(IJobService jobsService) { 
            service = jobsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await service.GetAllJobsAsync();
            return Ok(jobs);
        }


        [HttpGet("{id}", Name = "GetJobById")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await service.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] JobCreateUpdateDto job)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createjob = await service.CreateJobAsync(job);
            return CreatedAtAction(nameof(GetJobById), new { id = createjob.Id }, createjob);
        }
    }
}
