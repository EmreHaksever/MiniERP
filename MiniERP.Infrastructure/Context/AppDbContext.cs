using Microsoft.EntityFrameworkCore;
using MiniERP.Domain.Common;
using MiniERP.Domain.Entities;

namespace MiniERP.Infrastructure.Context
{
    // DbContext'ten miras alarak bu sınıfın bir veritabanı yöneticisi olduğunu belirtiyoruz.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Veritabanında oluşacak tablolarımız:
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // 🌟 SENIOR DOKUNUŞU: SaveChangesAsync metodunu eziyoruz (override).
        // Neden? Her kayıt eklediğimizde CreatedDate'i, her güncellediğimizde UpdatedDate'i 
        // elle yazmak ameleliktir ve unutulmaya müsaittir. Burada EF Core'a diyoruz ki: 
        // "Veritabanına gitmeden hemen önce araya gir ve tarihleri otomatik bas!"
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // ChangeTracker: O an bellekte değişen, eklenen veya silinen tüm nesneleri takip eden mekanizma.
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false; // İlk eklendiğinde silinmemiş kabul et
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                }
            }

            // Bizim müdahalemiz bitti, şimdi normal kaydetme işlemine devam et.
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}