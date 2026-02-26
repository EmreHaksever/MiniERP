using MiniERP.Domain.Common;

namespace MiniERP.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string CompanyName { get; set; } = string.Empty;

        // 1. DTO ile eşleşmesi için yeni eklediğimiz alan:
        public string ContactName { get; set; } = string.Empty;

        // 2. DTO ile eşleşmesi için "ContactEmail" adını "Email" olarak değiştirdik:
        public string? Email { get; set; }

        public string? TaxNumber { get; set; }
        public string? PhoneNumber { get; set; }

        // --- ERP Mantığı (Business Logic) İçin Gerekli Alanlar ---

        // Müşterinin şu anki toplam borcu/alacağı
        public decimal CurrentBalance { get; set; }

        // Müşteriye tanımlanan risk limiti (Örn: 50.000 TL'yi geçerse yeni satışa izin verme)
        public decimal RiskLimit { get; set; }

        // Bire-Çok İlişki: Bir müşterinin BİRDEN FAZLA siparişi olabilir.
        public ICollection<Order> Orders { get; set; }

        // Constructor (Yapıcı Metot)
        public Customer()
        {
            Orders = new List<Order>();
        }
    }
}