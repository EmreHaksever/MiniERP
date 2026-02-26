using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniERP.Application.Interfaces;
using MiniERP.Application.Interfaces.Repositories;
using MiniERP.Infrastructure.Context;
using MiniERP.Infrastructure.Repositories;
using MiniERP.Infrastructure.UnitOfWork;
using System;

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
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Global Hata Yakalayýcý Middleware'imizi devreye alýyoruz.
// Artýk projenin hiçbir yerinde try-catch yazmamýza gerek yok!
app.UseMiddleware<MiniERP.API.Middlewares.ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();