using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Interfaces;
using MiniERP.Domain.Entities;

namespace MiniERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        // 1. EKSİK OLAN KISIM: Constructor (Yapıcı Metot)
        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 2. EKSİK OLAN KISIM: GET İşlemi (Ürünleri Listeleme)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return Ok(products);
        }

        // 3. EKSİK OLAN KISIM: POST İşlemi (Ürün Ekleme)
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return Ok(product);
        }

        // 4. EKSİK OLAN KISIM: GET İşlemi (Kritik Stokları Listeleme)
        [HttpGet("critical-stock")]
        public async Task<IActionResult> GetCriticalStock()
        {
            var products = await _unitOfWork.Products.GetProductsWithCriticalStockAsync();
            return Ok(products);
        }
    }
}