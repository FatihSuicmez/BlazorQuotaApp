// Data/ApplicationDbContext.cs

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace QuotaApp.Data;

// IdentityDbContext sınıfını kendi ApplicationUser sınıfımızla kullanacağımızı belirtiyoruz.
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // YENİ: QueryLog sınıfımızı bir veritabanı tablosu ("QueryLogs") olarak tanıtıyoruz.
    public DbSet<QueryLog> QueryLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // YENİ: Proje tanımında istenen index'i ekliyoruz.
        // Bu, bir kullanıcıya ait sorguları tarihe göre ararken
        // veritabanının çok daha hızlı çalışmasını sağlar.
        builder.Entity<QueryLog>()
            .HasIndex(q => new { q.UserId, q.CreatedAtUtc });
    }
}