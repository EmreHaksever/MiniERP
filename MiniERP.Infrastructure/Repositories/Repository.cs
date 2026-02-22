using Microsoft.EntityFrameworkCore;
using MiniERP.Application.Interfaces.Repositories;
using MiniERP.Domain.Common;
using MiniERP.Infrastructure.Context;
using System.Linq.Expressions;

namespace MiniERP.Infrastructure.Repositories
{
    // 1. Kural: Bu sınıf IRepository<T> sözleşmesini imzalar.
    // 2. Kural: T mutlaka BaseEntity'den türeyen bir varlık (Product, Customer vb.) olmalıdır.
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        // Veritabanı ile konuşacak olan asıl nesnemiz (DbContext)
        protected readonly AppDbContext _context;

        // İşlem yapacağımız tabloyu temsil eder (Örn: Products tablosu)
        protected readonly DbSet<T> _dbSet;

        // Constructor (Yapıcı Metot): Bu repository çalıştığında bana bir DbContext ver diyoruz. (Buna Dependency Injection denir)
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); // Gelen T tipine göre ilgili tabloyu seç (T Product ise Products tablosunu seç)
        }

        public async Task AddAsync(T entity)
        {
            // Veritabanına "bunu ekle" diyoruz (Henüz kaydetmiyoruz, sadece hafızaya alıyor)
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            // Gelen şarta (predicate) uyanları ve IsDeleted == false olanları getir (Soft delete mantığı)
            return await _dbSet.Where(predicate).Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Silinmemiş tüm kayıtları getir
            return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            // İd'si eşleşen ve silinmemiş kaydı getir
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public void Remove(T entity)
        {
            // GERÇEK SİLME YAPMIYORUZ! (Soft Delete)
            // Sadece nesnenin IsDeleted özelliğini true yapıyoruz ve güncelliyoruz.
            entity.IsDeleted = true;
            _dbSet.Update(entity);

            // Eğer veritabanından tamamen uçurmak isteseydik şunu yazardık:
            // _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}