using Microsoft.EntityFrameworkCore;
using MiniERP.Application.Interfaces.Repositories;
using MiniERP.Domain.Entities;
using MiniERP.Infrastructure.Context;

namespace MiniERP.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        // Siparişi tüm detaylarıyla (Kalemleri ve Kalemlerin içindeki Ürün bilgisiyle) getiren özel metot
        public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
        {
            return await _dbSet
                .Include(o => o.OrderItems)          // Önce sipariş kalemlerini (sepettekileri) dahil et
                .ThenInclude(oi => oi.Product)       // Sonra o kalemlerin içindeki Ürün (Product) bilgilerini dahil et
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
        }
    }
}