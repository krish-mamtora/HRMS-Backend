using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Model.JobListing;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.JobListing
{
    public class ShareEmailService : IShareEmailService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public ShareEmailService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<ShareMailResponseDto>> getJobShareByUserId(int id)
        {
            var jobShare = await _context.ShareEmail.Where(x => x.EmpId == id).ToListAsync();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return _mapper.Map<List<ShareMailResponseDto>>(jobShare);

        }
    }
}
