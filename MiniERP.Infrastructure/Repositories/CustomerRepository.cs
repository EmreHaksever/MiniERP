using Microsoft.EntityFrameworkCore;
using MiniERP.Application.Interfaces.Repositories;
using MiniERP.Domain.Entities;
using MiniERP.Infrastructure.Context;

namespace MiniERP.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        // Müşteriye özel metodumuz: Risk limitini aşmış olanları getir
        public async Task<IEnumerable<Customer>> GetCustomersOverRiskLimitAsync()
        {
            // Güncel borcu, tanımlanan risk limitinden büyük olanları listele
            return await _dbSet
                .Where(c => !c.IsDeleted && c.CurrentBalance > c.RiskLimit)
                .ToListAsync();
        }
    }
}