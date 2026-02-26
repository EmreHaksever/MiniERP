using AutoMapper;
using MiniERP.Application.DTOs.Customers;
using MiniERP.Application.DTOs.Orders;
using MiniERP.Application.DTOs.Products;
using MiniERP.Domain.Entities;

namespace MiniERP.Application.Mappings
{
    // Profile sınıfından miras alıyoruz ki AutoMapper buranın bir kural seti olduğunu anlasın
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product sınıfı ile ProductDto birbirine dönüşebilir (ReverseMap sayesinde iki yönlü)
            CreateMap<Product, ProductDto>().ReverseMap();

            // Kullanıcıdan gelen ProductCreateDto, veritabanına yazılacak Product'a dönüşebilir
            CreateMap<ProductCreateDto, Product>();

            // Müşteri (Customer) çeviri kuralları
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CustomerCreateDto, Customer>();

            // Sipariş (Order) ve Sipariş Kalemi (OrderItem) çeviri kuralları
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderCreateDto, Order>();

            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<OrderItemCreateDto, OrderItem>();
        }
    }
}