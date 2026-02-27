using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Entities.GamesScheduling;
using HRMS_Backend.Model.GameScheduling;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.GameScheduling
{
    public class WaitingQueueService : IWaitingQueueService
    {

        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public WaitingQueueService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WaitingQueue> AddUsersToQueueAsync(int bookingId ,int slotId,int cycleId, int userIds)
        {
            var waitingQueue = new WaitingQueue
            {
                SlotId = slotId,
                CycleId = cycleId,
                Status = "Waiting"
            };
            await _context.WaitingQueue.AddAsync(waitingQueue);
            await _context.SaveChangesAsync();
            return waitingQueue;
        }

        public async Task<IEnumerable<WaitingQueueDisplayDto>> GetWaitingUsersAsync(int slotId)
        {
            var waitinglist = await _context.WaitingQueue.Where(wl => wl.SlotId == slotId && wl.Status == "Waiting").ToListAsync();
            return _mapper.Map<IEnumerable<WaitingQueueDisplayDto>>(waitinglist);
        }
        public async Task<Boolean> MarkUserAsAssignedAsync(int slotId, int userId)
        {
            var record = await _context.WaitingQueue.FirstOrDefaultAsync(wq => wq.SlotId == slotId && wq.PlayerId == userId && wq.Status == "Waiting");

            if (record == null) return false;

            record.Status = "Assigned";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Boolean> RemoveUserFromQueueAsync(int slotId, int userId)
        {
            var record = await _context.WaitingQueue.FirstOrDefaultAsync(wq => wq.SlotId == slotId && wq.PlayerId == userId);

            if (record == null) return false;

            _context.WaitingQueue.Remove(record);
             await _context.SaveChangesAsync();
             return true;
        }

        public async Task<Boolean> IsUserInQueueAsync(int slotId, int userId)
        {
            return await _context.WaitingQueue.AnyAsync(wq => wq.SlotId == slotId && wq.PlayerId == userId && wq.Status == "Waiting");
        }

        //public async Task<int> GetNextEligiblePerson(int slotId)S
        //{
        //    var nextPerson = await _context.WaitingQueue.Where(wq=> wq.Status=="Waiting").OrderBy(wq.)
        //}
    }
}
