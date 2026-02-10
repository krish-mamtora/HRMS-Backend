using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Model.JobListing;
using HRMS_Backend.Services.Jobs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.JobListing
{
    [Route("api/[controller]")]
    [ApiController]
    public class jobListingController : ControllerBase
    {
        private readonly IJobsService service;

        public jobListingController(IJobsService jobsService) { 
            service = jobsService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await service.GetAllJobsAsync());
        }
        [HttpPost]
        public async Task<IActionResult?> CreateJob([FromBody] JobCreateUpdateDto job)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createjob = await service.CreateJobAsync(job);
            return CreatedAtAction(nameof(GetJobById), new { id = createjob.Id }, createjob);
        }


        [HttpGet("{id}", Name = "GetJobById")]
        public async Task<IActionResult> GetJobById(int id) { 
            var job = service.GetJobByIdAsync(id);
            if(job == null)
            {
                return BadRequest(ModelState);
            }
              return Ok(job);
        }
    }
}
