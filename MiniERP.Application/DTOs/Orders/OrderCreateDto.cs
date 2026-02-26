namespace MiniERP.Application.DTOs.Orders
{
    public class OrderCreateDto
    {
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }

        // Siparişin içindeki kalemler (Sepet)
        public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
    }
}