using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiniERP.Application.Interfaces;
using MiniERP.Application.Interfaces.Repositories;
using MiniERP.Infrastructure.Context;
using MiniERP.Infrastructure.Repositories;
using MiniERP.Infrastructure.UnitOfWork;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsýný (DbContext) Sisteme Tanýtýyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. DEPENDENCY INJECTION (Baðýmlýlýk Enjeksiyonu) Ayarlarý
// "AddScoped" ne demek? (Mülakat Sorusu!): Her yeni HTTP isteði (Request) geldiðinde bu sýnýftan 1 tane yeni nesne üret, 
// istek bittiðinde (Response dönünce) nesneyi çöpe at demektir. Web projeleri için en güvenli yöntemdir.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// UnitOfWork'ü sisteme tanýtýyoruz
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper'ý manuel ve en garantili yöntemle sisteme tanýtýyoruz:
builder.Services.AddAutoMapper(config =>
{
    // Hangi profil sýnýfýný kullanacaðýný açýkça belirtiyoruz
    config.AddProfile<MiniERP.Application.Mappings.MappingProfile>();
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

// 2. SWAGGER ÜZERÝNE "AUTHORIZE" (KÝLÝT) BUTONUNU EKLEME
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
// Global Hata Yakalayýcý Middleware'imizi devreye alýyoruz.
// Artýk projenin hiçbir yerinde try-catch yazmamýza gerek yok!
app.UseMiddleware<MiniERP.API.Middlewares.ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // Kimlik Kontrolü
app.UseAuthorization();  // Yetki Kontrolü
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();