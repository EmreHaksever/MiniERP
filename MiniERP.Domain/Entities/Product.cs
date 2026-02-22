using MiniERP.Domain.Common;

namespace MiniERP.Domain.Entities
{
    // BaseEntity'den miras alıyoruz, böylece Id, CreatedDate vb. otomatik geliyor.
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }

        // Stok takibi için
        public int StockQuantity { get; set; }

        // Kritik stok kontrolü hedefimiz vardı, onun için bu alanı ekliyoruz
        public int CriticalStockLevel { get; set; }
    }
}