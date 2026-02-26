namespace MiniERP.Application.DTOs.Products
{
    // Dikkat et: Id, CreatedDate, IsDeleted gibi alanlar YOK! 
    // Sadece kullanıcının doldurmasına izin verdiğimiz alanlar var.
    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CriticalStockLevel { get; set; }
    }
}