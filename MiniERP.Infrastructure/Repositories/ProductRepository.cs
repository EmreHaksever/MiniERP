using Microsoft.EntityFrameworkCore;
using MiniERP.Application.Interfaces.Repositories;
using MiniERP.Domain.Entities;
using MiniERP.Infrastructure.Context;

namespace MiniERP.Infrastructure.Repositories
{
    // Hem genel işlemleri yapabilmek için Repository<Product>'tan miras alıyoruz,
    // Hem de Product'a özel işlemleri yapacağımıza dair IProductRepository sözleşmesini imzalıyoruz.
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        // Base(context) diyerek, DbContext'i üst sınıfa (Repository<T>) gönderiyoruz.
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        // Product'a özel metodumuzu implemente ediyoruz:
        public async Task<IEnumerable<Product>> GetProductsWithCriticalStockAsync()
        {
            // Stoğu, kritik stok seviyesinden küçük veya eşit olan silinmemiş ürünleri getir.
            return await _dbSet
                .Where(p => !p.IsDeleted && p.StockQuantity <= p.CriticalStockLevel)
                .ToListAsync();
        }
    }
}