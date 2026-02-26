namespace MiniERP.Application.DTOs.Customers
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal RiskLimit { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}