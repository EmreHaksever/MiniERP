using MiniERP.Domain.Entities;

namespace MiniERP.Application.Interfaces.Repositories
{
    // IRepository<Product> diyerek temel CRUD işlemlerini otomatik miras aldık.
    public interface IProductRepository : IRepository<Product>
    {
        // Temel Ekle/Sil dışında, sadece Product'a özel bir metot sözleşmesi:
        Task<IEnumerable<Product>> GetProductsWithCriticalStockAsync();
    }
}