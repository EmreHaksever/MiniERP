using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.DTOs.Orders;
using MiniERP.Application.Interfaces;
using MiniERP.Domain.Entities;

namespace MiniERP.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrdersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreateDto)
        {
            var order = _mapper.Map<Order>(orderCreateDto);
            order.OrderNumber = "ORD-" + new Random().Next(1000, 9999);

            // 🌟 1. STOK KONTROLÜ VE DÜŞME İŞLEMİ
            foreach (var item in order.OrderItems)
            {
                // Önce sepetteki ürünü veritabanından bul
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);

                if (product == null)
                    return BadRequest($"Hata: {item.ProductId} ID'li ürün bulunamadı!");

                // Eğer müşterinin istediği adet, depodaki stoktan fazlaysa hata dön!
                if (product.StockQuantity < item.Quantity)
                    return BadRequest($"Hata: {product.Name} ürünü için yeterli stok yok! Mevcut Stok: {product.StockQuantity}");

                // Stok yeterliyse, depodaki miktarı düş ve güncelle
                product.StockQuantity -= item.Quantity;
                _unitOfWork.Products.Update(product);
            }

            // Siparişin toplam tutarını hesapla
            order.TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);

            await _unitOfWork.Orders.AddAsync(order);

            // 🌟 2. MÜŞTERİ CARİ BORÇ GÜNCELLEMESİ (Zaten yazmıştık)
            var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId);
            if (customer != null)
            {
                customer.CurrentBalance += order.TotalAmount;
                _unitOfWork.Customers.Update(customer);
            }

            // Tüm bu işlemleri (Stok düşme, Sipariş ekleme, Borç artırma) TEK BİR PAKET olarak kaydet
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<OrderDto>(order);
            return Ok(resultDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Infrastructure katmanında yazdığımız "Eager Loading" yapan o özel metodu çağırıyoruz.
            // Bu sayede siparişin içindeki kalemler (OrderItems) boş gelmeyecek!
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id);

            if (order == null) return NotFound("Sipariş bulunamadı!");

            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }
    }
}