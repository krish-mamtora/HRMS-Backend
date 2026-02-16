using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.TravelandExpense;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRMS_Backend.Services.TravelandExpenses
{
    public class TravelExpenseService :ITravelExpenseService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public TravelExpenseService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TravelExpense> CreateTravelExpenseAsync(ExpenseCreateUpdateDto dto)
        {
            var travelPlanExpense = new TravelExpense
            {         /// TravelAssignId , Id ,ExpenseType ,ExpensePolicy ,Amount ,Status ,HrRemarks, CreatedAt , ApprovedBy
                TravelAssignId = dto.TravelAssignId,
                ExpenseType = dto.ExpenseType,
                Amount = dto.Amount,
                Status = dto.Status,
                ApprovedBy = dto.ApprovedBy,
                CreatedAt = dto.CreatedAt
            };
            
            _context.TravelExpense.Add(travelPlanExpense);
            await _context.SaveChangesAsync();
            return travelPlanExpense;

        }

        public async Task<IEnumerable<ExpenseDisplayDto>> GetAllExpenseAsync()
        {
            var expenses = await _context.TravelExpense.ToListAsync();
            return _mapper.Map<IEnumerable<ExpenseDisplayDto>>(expenses); // need ot add 
        }

        public async Task<ExpenseDisplayDto> GetExpenseByIdAsync(int id)
        {
            var expense = await _context.TravelExpense.FindAsync(id);
            if (expense == null)
            {
                return null;
            }
            return _mapper.Map<ExpenseDisplayDto>(expense);
        }

        public async Task<ExpenseDisplayDto> GetExpenseByTravelAssignmentId(int id)
        {
            var expense = await _context.TravelExpense.FirstOrDefaultAsync(e => e.TravelAssignId == id);
            if (expense == null)
            {
                return null;
            }
            return _mapper.Map<ExpenseDisplayDto>(expense);
        }

    }
}
