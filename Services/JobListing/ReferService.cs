using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Migrations;
using HRMS_Backend.Model.JobListing;
using HRMS_Backend.Model.TravelandExpense;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.JobListing
{
    public class ReferService : IReferService
    {
        private readonly MyDbContext _context;
        private readonly IReferService _service;
        private readonly IMapper _mapper;
        public ReferService(MyDbContext context , IReferService service) { 
            _context = context;
            _service = service;
        }
        public async Task<Referals> createReferalAsync(JobRefferalCreateUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var referal = new Referals
            {
                Id = dto.Id,
                JobId = dto.JobId,
                ReffName = dto.ReffName,
                ReffMail = dto.ReffMail,
                ReffResumeUrl = dto.ReffResumeUrl,
                EmpId = dto.EmpId,
                Description = dto.Description,
                CreatedAt = dto.CreatedAt,
            };
            _context.Referals.AddAsync(referal);
            await _context.SaveChangesAsync();
            return referal;
        }

        public async Task<JobRefferalResponseDto> getReferalById(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var Referals = await _context.Referals.FindAsync(id);
            var ReferalsDto = _mapper.Map<JobRefferalResponseDto>(Referals);
            return ReferalsDto;

        }

        public async Task<JobRefferalResponseDto> getReferalByJobId(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var Referals = await _context.Referals.FirstOrDefaultAsync(x=>x.JobId == id);
            var ReferalsDto = _mapper.Map<JobRefferalResponseDto>(Referals);
            return ReferalsDto;

        }
        public async Task<JobRefferalResponseDto> getReferalByUserId(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var Referals = await _context.Referals.FirstOrDefaultAsync(x => x.EmpId == id);
            var ReferalsDto = _mapper.Map<JobRefferalResponseDto>(Referals);
            return ReferalsDto;

        }

        public async Task<bool> UpdateReferalWithId(int id , JobRefferalCreateUpdateDto dto)
        {
            var referal = await _context.Referals.FindAsync(id);
            if(referal == null)
            {
                return false;
            }
            _mapper.Map(dto, referal);
            try
            {
                _context.Referals.Update(referal);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }
    }
}
