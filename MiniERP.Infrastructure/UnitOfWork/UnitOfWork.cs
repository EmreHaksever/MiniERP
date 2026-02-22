using MiniERP.Application.Interfaces;
using MiniERP.Application.Interfaces.Repositories;
using MiniERP.Infrastructure.Context;
using MiniERP.Infrastructure.Repositories;

namespace MiniERP.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // Repository'leri sadece okunabilir (readonly) property olarak tanımlıyoruz
        public IProductRepository Products { get; private set; }
        public ICustomerRepository Customers { get; private set; }
        public IOrderRepository Orders { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            // İstendiği anda Repository'lerin nesnelerini (Instance) oluşturup içine DbContext'i veriyoruz
            Products = new ProductRepository(_context);
            Customers = new CustomerRepository(_context);
            Orders = new OrderRepository(_context);
        }

        // Tüm değişiklikleri tek bir "Transaction" (İşlem bütünlüğü) halinde veritabanına yazar.
        // Hata olursa hiçbirini yazmaz.
        public async Task<int> SaveChangesAsync()
        {
            // Burada context.SaveChangesAsync() çalıştığında, AppDbContext içinde 
            // ezdiğimiz (override) o tarih atama işlemleri otomatik devreye girecek!
            return await _context.SaveChangesAsync();
        }

        // İşimiz bitince RAM'den bu sınıfı temizler (IDisposable implementasyonu)
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}