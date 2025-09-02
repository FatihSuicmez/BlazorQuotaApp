// Services/IQuotaService.cs

using QuotaApp.Models; // <-- YENİ EKLENEN SATIR

namespace QuotaApp.Services;

// Buradaki UsageInfo ve QuotaException sınıf tanımlarını sildik.
// Onlar artık Models klasöründen gelecek.

public interface IQuotaService
{
    Task<UsageInfo> GetUsageAsync(string userId);
    Task<UsageInfo> TryConsumeAsync(string userId, string term);
}