using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.DTOs.Customers;
using MiniERP.Application.Interfaces;
using MiniERP.Domain.Entities;

namespace MiniERP.API.Controllers
{
    [Authorize] // Güvenlik kilitimizi baştan takıyoruz!
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var customersDto = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return Ok(customersDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerCreateDto customerCreateDto)
        {
            var customer = _mapper.Map<Customer>(customerCreateDto);

            // Yeni müşterinin başlangıç borcunu sıfır olarak ayarlıyoruz
            customer.CurrentBalance = 0;

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<CustomerDto>(customer);
            return Ok(resultDto);
        }

        // Müşteri repository'sine yazdığımız o özel metodu dışarı açıyoruz
        [HttpGet("over-risk-limit")]
        public async Task<IActionResult> GetCustomersOverRiskLimit()
        {
            var customers = await _unitOfWork.Customers.GetCustomersOverRiskLimitAsync();
            var customersDto = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return Ok(customersDto);
        }
    }
}