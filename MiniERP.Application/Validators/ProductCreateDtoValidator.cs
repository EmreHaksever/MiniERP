using FluentValidation;
using MiniERP.Application.DTOs.Products;

namespace MiniERP.Application.Validators
{
    // Hangi DTO'yu doğrulayacağımızı AbstractValidator<T> ile belirtiyoruz
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            // Ürün Adı Kuralları
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı boş bırakılamaz!")
                .MinimumLength(3).WithMessage("Ürün adı en az 3 karakter olmalıdır!")
                .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir!");

            // Fiyat Kuralları
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Ürün fiyatı 0'dan büyük olmalıdır!");

            // Stok Kuralları
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stok adedi eksiye düşemez!");

            // Kritik Stok Kuralları
            RuleFor(x => x.CriticalStockLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Kritik stok seviyesi eksi olamaz!");
        }
    }
}