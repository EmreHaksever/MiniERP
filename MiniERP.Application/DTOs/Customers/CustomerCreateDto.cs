namespace MiniERP.Application.DTOs.Customers
{
    public class CustomerCreateDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal RiskLimit { get; set; }
        // CurrentBalance (Güncel Borç) alanını burada istemiyoruz, çünkü yeni müşterinin borcu sıfırdır!
    }
}