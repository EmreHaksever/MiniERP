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
            // 1. DTO'yu gerçek Order nesnesine çevir
            var order = _mapper.Map<Order>(orderCreateDto);

            // 2. Rastgele bir sipariş numarası üret (Örn: ORD-4821)
            order.OrderNumber = "ORD-" + new Random().Next(1000, 9999);

            // 3. Siparişin toplam tutarını hesapla (Sepetteki her ürünün Adet * Birim Fiyatı)
            order.TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);

            // Siparişi veritabanına ekle
            await _unitOfWork.Orders.AddAsync(order);

            // 🌟 SENIOR ERP DOKUNUŞU: Müşterinin cari borcunu (CurrentBalance) güncelle!
            var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId);
            if (customer != null)
            {
                customer.CurrentBalance += order.TotalAmount;
                _unitOfWork.Customers.Update(customer);
            }

            // Tüm işlemleri tek bir Transaction (işlem bütünlüğü) olarak kaydet
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