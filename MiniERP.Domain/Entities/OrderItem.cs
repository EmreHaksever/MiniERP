using MiniERP.Domain.Common;

namespace MiniERP.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        // Bu kalem hangi siparişe ait?
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        // Bu kalem hangi ürünü temsil ediyor?
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        // Kaç adet alındı?
        public int Quantity { get; set; }

        // Ürünün o anki satış fiyatı (Neden Product tablosundan çekmiyoruz? Açıklaması aşağıda)
        public decimal UnitPrice { get; set; }

        // Miktar * Birim Fiyat
        public decimal TotalPrice { get; set; }
    }
}