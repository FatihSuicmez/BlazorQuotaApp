// Services/QuotaService.cs

using Microsoft.EntityFrameworkCore;
using QuotaApp.Data;
using QuotaApp.Models; // <-- YENİ EKLENEN SATIR

namespace QuotaApp.Services;

public class QuotaService : IQuotaService
{
    private readonly ApplicationDbContext _db;
    private readonly TimeSpan _istanbulOffset = TimeSpan.FromHours(3);
    private const int DailyLimit = 5;
    private const int MonthlyLimit = 20;

    public QuotaService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UsageInfo> GetUsageAsync(string userId)
    {
        var utcNow = DateTime.UtcNow;
        var (dayStartUtc, _, monthStartUtc, _, dayResetLocal, monthResetLocal) = CalculateTimeWindows(utcNow);
        
        var dailyUsed = await _db.QueryLogs.CountAsync(q => 
            q.UserId == userId && q.CreatedAtUtc >= dayStartUtc);

        var monthlyUsed = await _db.QueryLogs.CountAsync(q => 
            q.UserId == userId && q.CreatedAtUtc >= monthStartUtc);

        return new UsageInfo
        {
            DayUsed = dailyUsed,
            DayRemaining = Math.Max(0, DailyLimit - dailyUsed),
            MonthUsed = monthlyUsed,
            MonthRemaining = Math.Max(0, MonthlyLimit - monthlyUsed),
            DayResetAtLocal = dayResetLocal,
            MonthResetAtLocal = monthResetLocal
        };
    }

    public async Task<UsageInfo> TryConsumeAsync(string userId, string term)
    {
        // SQLite'ta aynı anda birden fazla yazma işlemi sorun çıkarabilir.
        // BeginTransactionAsync, bu kod bloğu bitene kadar veritabanına başka
        // bir yazma işlemi yapılmayacağını garanti eder. Race condition'ı önler.
        await using var transaction = await _db.Database.BeginTransactionAsync();

        var currentUsage = await GetUsageAsync(userId);

        if (currentUsage.DayRemaining <= 0)
        {
            throw new QuotaException("DAILY_LIMIT_EXCEEDED", "Günlük limitiniz (5) doldu. Yarın tekrar deneyin.");
        }
            
        if (currentUsage.MonthRemaining <= 0)
        {
            throw new QuotaException("MONTHLY_LIMIT_EXCEEDED", "Aylık toplam hakkınız (20) doldu. Bir sonraki ay tekrar deneyin.");
        }

        // Limitler uygunsa, yeni sorgu kaydını veritabanına ekle.
        var queryLog = new QueryLog
        {
            UserId = userId,
            Term = term,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.QueryLogs.Add(queryLog);
        await _db.SaveChangesAsync();

        // Her şey yolunda gittiyse, işlemi onayla.
        await transaction.CommitAsync();

        // Kullanım sonrası güncel bilgileri tekrar hesaplayıp döndür.
        return await GetUsageAsync(userId);
    }
    
    // Bu özel metot, İstanbul saatine göre günün ve ayın başlangıç/bitiş zamanlarını
    // hesaplayıp sorgular için gerekli olan UTC formatına geri çevirir.
    private (DateTime dayStartUtc, DateTime dayEndUtc, DateTime monthStartUtc, DateTime nextMonthStartUtc, DateTime dayResetLocal, DateTime monthResetLocal) 
        CalculateTimeWindows(DateTime utcNow)
    {
        var localTime = utcNow + _istanbulOffset;

        var dayStartLocal = localTime.Date; // Örn: 2025-09-02 00:00:00
        var monthStartLocal = new DateTime(localTime.Year, localTime.Month, 1);

        var dayResetLocal = dayStartLocal.AddDays(1);
        var monthResetLocal = monthStartLocal.AddMonths(1);

        // Sorgularda kullanmak için bu lokal zamanları tekrar UTC'ye çeviriyoruz.
        return (
            dayStartLocal - _istanbulOffset,
            dayResetLocal - _istanbulOffset,
            monthStartLocal - _istanbulOffset,
            monthResetLocal - _istanbulOffset,
            dayResetLocal,
            monthResetLocal
        );
    }
}