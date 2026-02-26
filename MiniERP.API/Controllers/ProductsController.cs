using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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

        // URL'den "page" (sayfa) ve "pageSize" (sayfa başına kayıt) parametrelerini alıyoruz. 
        // Eğer kullanıcı bir şey göndermezse varsayılan olarak 1. sayfa ve 10 kayıt diyoruz.
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // 1. Tüm ürünleri çek
            var products = await _unitOfWork.Products.GetAllAsync();

            // 2. Toplam ürün sayısını bul (Arayüzde sayfa numaralarını çizmek için lazım olacak)
            var totalRecords = products.Count();

            // 🌟 3. SAYFALAMA MANTIĞI (LINQ)
            // Skip: Önceki sayfalardaki kayıtları atla
            // Take: Sadece bu sayfanın boyutu kadar kayıt al
            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 4. DTO'ya çevir
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts);

            // 5. Geriye sadece veriyi değil, sayfalama bilgilerini de (Meta Veri) dönüyoruz!
            return Ok(new
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize), // Toplam kaç sayfa var?
                Data = productsDto // Asıl ürün verimiz
            });
        }

        [Authorize]
        [HttpPost]
        // 🌟 GÜNCELLEME: IValidator'ı metodun içine enjekte ettik
        public async Task<IActionResult> Create(ProductCreateDto productCreateDto, [FromServices] IValidator<ProductCreateDto> validator)
        {
            // 🌟 1. Kapıdaki muhafıza "Bu veriyi kontrol et" diyoruz
            var validationResult = await validator.ValidateAsync(productCreateDto);

            // 🌟 2. Eğer kurallara uymayan bir şey varsa, içeri hiç almadan direkt hataları fırlat!
            if (!validationResult.IsValid)
            {
                // Gelen hataları şık bir liste halinde kullanıcıya dönüyoruz
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // 3. Kullanıcıdan gelen geçerli veriyi (DTO), veritabanına uyacak gerçek nesneye (Entity) çevir
            var product = _mapper.Map<Product>(productCreateDto);

            // 4. Veritabanına ekle ve kaydet (Id ve CreatedDate burada otomatik dolacak)
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // 5. Kullanıcıya "Başarıyla eklendi" derken, oluşan Id ve Tarih bilgisini de göstermek için tekrar DTO'ya çevir
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