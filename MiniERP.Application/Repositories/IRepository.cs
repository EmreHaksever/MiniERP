using MiniERP.Domain.Common;
using System.Linq.Expressions;

namespace MiniERP.Application.Interfaces.Repositories
{
    // <T> ifadesi buraya Product, Customer gibi herhangi bir sınıfın gelebileceğini söyler.
    // where T : BaseEntity -> "Ama buraya rastgele bir sınıf gelemez, sadece BaseEntity'den türeyenler gelebilir" diyerek güvenlik önlemi alıyoruz.
    public interface IRepository<T> where T : BaseEntity
    {
        // Asenkron (Task) metotlar kullanıyoruz çünkü veritabanı işlemleri zaman alır 
        // ve ana iş parçacığını (thread) kilitlememelidir. Kurumsal projelerde her şey Async'tir.
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();

        // Expression kullanarak "Bana stoğu 10'dan küçük olanları getir" gibi özel filtreler yazabilmemizi sağlar.
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        void Update(T entity); // Update ve Remove işlemlerinin Async versiyonu EF Core'da yoktur, senkron çalışırlar.
        void Remove(T entity);
    }
}