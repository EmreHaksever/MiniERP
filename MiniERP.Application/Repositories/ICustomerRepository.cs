using MiniERP.Domain.Entities;

namespace MiniERP.Application.Interfaces.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        // Sadece Müşteriye özel metot: Risk limitini aşan müşterileri getir
        Task<IEnumerable<Customer>> GetCustomersOverRiskLimitAsync();
    }
}