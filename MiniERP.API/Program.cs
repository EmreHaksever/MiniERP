using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiniERP.Application.Interfaces;
using MiniERP.Application.Interfaces.Repositories;
using MiniERP.Infrastructure.Context;
using MiniERP.Infrastructure.Repositories;
using MiniERP.Infrastructure.UnitOfWork;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsýný (DbContext) Sisteme Tanýtýyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. DEPENDENCY INJECTION (Baðýmlýlýk Enjeksiyonu) Ayarlarý
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// UnitOfWork'ü sisteme tanýtýyoruz
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper'ý manuel ve en garantili yöntemle sisteme tanýtýyoruz:
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<MiniERP.Application.Mappings.MappingProfile>();
});

// FluentValidation'ý sisteme MODERN yöntemle tanýtýyoruz (AutoValidation YOK):
builder.Services.AddValidatorsFromAssemblyContaining<MiniERP.Application.Validators.ProductCreateDtoValidator>();

// Sadece Controllers'ý ekliyoruz
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 3. JWT KÝMLÝK DOÐRULAMA AYARLARI
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// 4. SWAGGER ÜZERÝNE "AUTHORIZE" (KÝLÝT) BUTONUNU EKLEME
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniERP API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Lütfen 'Bearer' kelimesini yazýp boþluk býraktýktan sonra Token'ýnýzý yapýþtýrýn. \r\n\r\n Örnek: \"Bearer eyJhbGciOiJIUzI1Ni...\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 5. MÝDDLEWARE (Ara Yazýlým) BORU HATTI
// Global Hata Yakalayýcý Middleware'imizi devreye alýyoruz.
app.UseMiddleware<MiniERP.API.Middlewares.ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Güvenlik Kapýlarý (Sýrasý Çok Önemlidir!)
app.UseAuthentication(); // Önce Kimliðini Doðrula (Kimsin?)
app.UseAuthorization();  // Sonra Yetkini Kontrol Et (Buna Ýznin Var Mý?)

app.MapControllers();

app.Run();