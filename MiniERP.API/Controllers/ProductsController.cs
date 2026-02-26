using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.DTOs.Products;
using MiniERP.Application.Interfaces;
using MiniERP.Domain.Entities;

namespace MiniERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; // Çevirmenimizi tanımlıyoruz

        // IMapper'ı içeri enjekte ediyoruz
        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // 1. Veritabanından ham verileri (Entity) çek
            var products = await _unitOfWork.Products.GetAllAsync();

            // 2. Ham verileri, dışarıya açacağımız güvenli formata (ProductDto) çevir
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(productsDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto productCreateDto) // Artık Product değil, ProductCreateDto bekliyoruz!
        {
            // 1. Kullanıcıdan gelen "eksik" veriyi (DTO), veritabanına uyacak gerçek nesneye (Entity) çevir
            var product = _mapper.Map<Product>(productCreateDto);

            // 2. Veritabanına ekle ve kaydet (Id ve CreatedDate burada otomatik dolacak)
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // 3. Kullanıcıya "Başarıyla eklendi" derken, oluşan Id ve Tarih bilgisini de göstermek için tekrar DTO'ya çevir
            var resultDto = _mapper.Map<ProductDto>(product);

            return Ok(resultDto);
        }

        [HttpGet("critical-stock")]
        public async Task<IActionResult> GetCriticalStock()
        {
            var products = await _unitOfWork.Products.GetProductsWithCriticalStockAsync();
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productsDto);
        }
    }
}