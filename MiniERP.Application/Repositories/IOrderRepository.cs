using MiniERP.Domain.Entities;

namespace MiniERP.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        // Siparişleri, içindeki ürün kalemleriyle (OrderItems) beraber getirmek için özel metot
        Task<Order?> GetOrderWithDetailsAsync(Guid orderId);
    }
}