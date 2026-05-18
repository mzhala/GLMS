using GLMS.Data;
using GLMS.Models;
using GLMS.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Services
{
    public class ContractService
    {
        private readonly ApplicationDbContext _context;

        public ContractService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ServiceResult> CreateAsync(Contract contract)
        {
            if (contract.EndDate < contract.StartDate)
            {
                return ServiceResult.Fail(
                    "End date cannot be before start date.");
            }

            if (contract.Status == ContractStatus.Active &&
                contract.EndDate < DateTime.Now)
            {
                return ServiceResult.Fail(
                    "Active contracts cannot already be expired.");
            }

            _context.Contracts.Add(contract);

            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public bool Exists(int id)
        {
            return _context.Contracts.Any(c => c.Id == id);
        }
    }
}