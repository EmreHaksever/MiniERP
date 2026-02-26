namespace MiniERP.Application.DTOs.Orders
{
    public class OrderItemCreateDto
    {
        public Guid ProductId { get; set; }

        // Kullanıcı sadece adedi gönderebilir, fiyatı biz arkada bulacağız!
        public int Quantity { get; set; }
    }
}