// Endpoints/SearchApiEndpoints.cs

using Microsoft.AspNetCore.Authorization;
using QuotaApp.Services;
using System.Security.Claims;
using QuotaApp.Models;

namespace QuotaApp.Endpoints;

// Bu API endpoint'lerinin istek gövdesinde gelecek olan JSON verisini temsil eden bir kayıt.
public record SearchRequest(string Term);

public static class SearchApiEndpoints
{
    public static void MapSearchApiEndpoints(this IEndpointRouteBuilder app)
    {
        // Tüm endpoint'leri bir grup altında toplayıp ortak kurallar uyguluyoruz.
        var group = app.MapGroup("/api").RequireAuthorization(); // "/api" ile başlayan tüm yollar ve hepsi yetki (login) gerektirir.

        // GET /api/usage endpoint'i
        group.MapGet("/usage", async (HttpContext context, IQuotaService quotaService) =>
        {
            // O anki giriş yapmış kullanıcının kimliğini (ID) alıyoruz.
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var usage = await quotaService.GetUsageAsync(userId);
            return Results.Ok(usage);
        });

        // POST /api/search endpoint'i
        group.MapPost("/search", async (SearchRequest request, HttpContext context, IQuotaService quotaService) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                // Servisimizdeki tüketme metodunu çağırıyoruz.
                var usageInfo = await quotaService.TryConsumeAsync(userId, request.Term);

                // Proje tanımındaki gibi rate-limit başlıklarını yanıta ekliyoruz. (Artı puan!)
                context.Response.Headers.Append("X-RateLimit-Limit-Day", "5");
                context.Response.Headers.Append("X-RateLimit-Remaining-Day", usageInfo.DayRemaining.ToString());
                context.Response.Headers.Append("X-RateLimit-Limit-Month", "20");
                context.Response.Headers.Append("X-RateLimit-Remaining-Month", usageInfo.MonthRemaining.ToString());
                
                // Başarılı yanıtı oluşturuyoruz.
                var response = new
                {
                    items = new[] { $"'{request.Term}' için sonuç 1", $"'{request.Term}' için sonuç 2" }, // Dummy data
                    usage = usageInfo
                };

                return Results.Ok(response);
            }
            // Servisimiz limit aşımında QuotaException fırlatıyordu. Onu burada yakalıyoruz.
            catch (QuotaException ex)
            {
                // Hata durumunda 429 Too Many Requests kodu ile birlikte hata mesajını dönüyoruz.
                var errorResponse = new { code = ex.Code, message = ex.Message };
                return Results.Json(errorResponse, statusCode: StatusCodes.Status429TooManyRequests);
            }
        });
    }
}