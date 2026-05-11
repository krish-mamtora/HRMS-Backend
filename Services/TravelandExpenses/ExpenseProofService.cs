using AutoMapper;
using HRMS_Backend.Common.Exceptions;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.TravelandExpense;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.TravelandExpenses
{
    public class ExpenseProofService : IExpenseProofService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<ExpenseProofService> _logger;

        public ExpenseProofService(
            MyDbContext context,
            IMapper mapper,
            IWebHostEnvironment hostingEnvironment,
            ILogger<ExpenseProofService> logger)
        {
            _context = context;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public async Task<ExpenseProof>
            CreateExpenseProofAsync(
                ExpenseProofCreateUpdateDto dto)
        {
            if (dto.ProofDocument == null)
            {
                throw new BadRequestException(
                    "Proof document is required");
            }

            var uploadsFolder = Path.Combine(
                _hostingEnvironment.ContentRootPath,
                "UploadedExpenseProof");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName =
                $"{Guid.NewGuid()}_{Path.GetFileName(dto.ProofDocument.FileName)}";

            var filePath = Path.Combine(
                uploadsFolder,
                uniqueFileName);

            await using (var fileStream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await dto.ProofDocument
                    .CopyToAsync(fileStream);
            }

            var expenseProof = new ExpenseProof
            {
                TravelExpenseId = dto.TravelExpenseId,

                ProofDocumentUrl = uniqueFileName,

                CreatedAt = DateTime.UtcNow
            };

            await _context.ExpenseProof
                .AddAsync(expenseProof);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Expense proof uploaded successfully");

            return expenseProof;
        }

        public async Task<ExpenseProofDisplayDto>
            GetExpenseProofByIdAsync(int id)
        {
            var expenseProof = await _context.ExpenseProof
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (expenseProof is null)
            {
                throw new NotFoundException(
                    "Expense proof not found");
            }

            return _mapper.Map<ExpenseProofDisplayDto>(
                expenseProof);
        }

        public async Task<IEnumerable<ExpenseProofDisplayDto>>
            GetExpenseProofByExpenseIdAsync(int id)
        {
            var expenseProofs = await _context.ExpenseProof
                .AsNoTracking()
                .Where(x => x.TravelExpenseId == id)
                .ToListAsync();

            return _mapper.Map<
                IEnumerable<ExpenseProofDisplayDto>>(
                expenseProofs);
        }
    }
}