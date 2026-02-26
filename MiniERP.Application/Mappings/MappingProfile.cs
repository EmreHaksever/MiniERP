using AutoMapper;
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
        }
    }
}