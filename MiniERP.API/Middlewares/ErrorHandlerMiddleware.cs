using System.Net;
using System.Text.Json;

namespace MiniERP.API.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        // Dependency Injection ile akışı (_next) ve Loglama aracını alıyoruz
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // API'ye gelen HER İSTEK bu metottan geçer
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // İstegi Controller'a gönder, her şey yolundaysa devam et
                await _next(context);
            }
            catch (Exception error)
            {
                // EĞER SİSTEMİN HERHANGİ BİR YERİNDE HATA PATLARSA BURAYA DÜŞER!
                var response = context.Response;
                response.ContentType = "application/json";

                // Varsayılan olarak 500 Internal Server Error döneceğiz
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // 1. Hatayı konsola/dosyaya logla (İleride Serilog bağlayacağız buraya)
                _logger.LogError(error, "Sistemde beklenmeyen bir hata oluştu!");

                // 2. Kullanıcıya döneceğimiz standart, güvenli JSON formatını hazırla
                var result = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Sunucu tarafında bir hata oluştu. Lütfen daha sonra tekrar deneyin.",
                    detail = error.Message // (Gerçek projede detail kısmını sadece Development ortamında gösteririz, şimdilik test için açık bırakıyoruz)
                });

                // 3. Kullanıcıya cevabı gönder
                await response.WriteAsync(result);
            }
        }
    }
}