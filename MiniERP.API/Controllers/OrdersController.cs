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

            foreach (var item in order.OrderItems)
            {
                // 1. Ürünü veritabanından bul
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null) return BadRequest($"Hata: {item.ProductId} ID'li ürün bulunamadı!");

                // 2. Stok Kontrolü
                if (product.StockQuantity < item.Quantity)
                    return BadRequest($"Hata: {product.Name} ürünü için yeterli stok yok!");

                // 🌟 3. İŞTE O MÜKEMMEL SENIOR DOKUNUŞU: Fiyatı güvenlik için veritabanından alıyoruz!
                item.UnitPrice = product.Price;

                // 4. Stoku düş
                product.StockQuantity -= item.Quantity;
                _unitOfWork.Products.Update(product);
            }

            // Siparişin toplam tutarını hesapla (Artık güvenli fiyatlarla hesaplanıyor)
            order.TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);

            await _unitOfWork.Orders.AddAsync(order);

            var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId);
            if (customer != null)
            {
                customer.CurrentBalance += order.TotalAmount;
                _unitOfWork.Customers.Update(customer);
            }

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