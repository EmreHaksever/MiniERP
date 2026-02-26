namespace MiniERP.Application.DTOs.Products
{
    // Dışarıya veri verirken Id ve CreatedDate'i gösterebiliriz, 
    // ama IsDeleted gibi arka plan detaylarını yine gizliyoruz.
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CriticalStockLevel { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}