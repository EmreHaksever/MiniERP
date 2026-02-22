using MiniERP.Domain.Common;

namespace MiniERP.Domain.Entities
{
    public class Order : BaseEntity
    {
        // Satışın hangi müşteriye ait olduğunu tutan Yabancı Anahtar (Foreign Key)
        public Guid CustomerId { get; set; }

        // İlişki (Navigation Property): Kod yazarken o siparişin müşterisine kolayca erişmek için
        public Customer Customer { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Fatura kesilip kesilmediği durumu
        public string OrderStatus { get; set; } = "Pending";

        // Bire-Çok İlişki: Bir siparişin BİRDEN FAZLA kalemi (ürünü) olur.
        public ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderItems = new List<OrderItem>();
            OrderDate = DateTime.UtcNow; // Sipariş oluşturulduğunda tarihi otomatik şu anki zaman yap.
        }
    }
}