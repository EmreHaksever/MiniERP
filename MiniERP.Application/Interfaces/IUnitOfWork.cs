using MiniERP.Application.Interfaces.Repositories;

namespace MiniERP.Application.Interfaces
{
    // IDisposable: İşimiz bittiğinde RAM'i temizlemek (Garbage Collector'a yardım etmek) için
    public interface IUnitOfWork : IDisposable
    {
        // Tüm repository'lere tek bir merkezden erişim sağlıyoruz.
        IProductRepository Products { get; }
        ICustomerRepository Customers { get; }
        IOrderRepository Orders { get; }

        // Yapılan tüm değişiklikleri tek seferde veritabanına "COMMIT" eder (Uygular).
        Task<int> SaveChangesAsync();
    }
}