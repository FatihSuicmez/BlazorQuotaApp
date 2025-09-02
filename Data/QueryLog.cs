// Data/QueryLog.cs

using System.ComponentModel.DataAnnotations.Schema;

namespace QuotaApp.Data; // Proje adın QuotaApp olduğu için namespace bu şekilde olmalı

public class QueryLog
{
    public int Id { get; set; } // Her kayıt için benzersiz numara (Primary Key)

    public string UserId { get; set; } = null!; // Sorguyu yapan kullanıcının kimliği

    public string Term { get; set; } = null!; // Aranan kelime

    public DateTime CreatedAtUtc { get; set; } // Sorgunun yapıldığı zaman (UTC formatında)

    // Bu özellik, UserId'nin ApplicationUser tablosundaki bir kullanıcıya ait
    // olduğunu Entity Framework'e bildirir. Buna "Navigation Property" denir.
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;
}