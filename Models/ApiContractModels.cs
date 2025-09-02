// Models/ApiContractModels.cs

namespace QuotaApp.Models;

// Bu dosya, API ile Blazor arayüzü arasındaki veri alışverişini
// temsil eden "sözleşme" modellerini içerir.

public class UsageInfo
{
    public int DayUsed { get; set; }
    public int DayRemaining { get; set; }
    public int MonthUsed { get; set; }
    public int MonthRemaining { get; set; }
    
    // EKSİK OLAN VE ŞİMDİ EKLEDİĞİMİZ ÖZELLİKLER
    public DateTime DayResetAtLocal { get; set; }
    public DateTime MonthResetAtLocal { get; set; }
}

public class SearchResponse
{
    public List<string> Items { get; set; } = new();
    public UsageInfo? Usage { get; set; }
}

public class ErrorResponse
{
    public string Code { get; set; } = "";
    public string Message { get; set; } = "";
}

public class SearchRequest
{
    public string? Term { get; set; }
}

public class QuotaException : Exception
{
    public string Code { get; }
    public QuotaException(string code, string message) : base(message)
    {
        Code = code;
    }
}