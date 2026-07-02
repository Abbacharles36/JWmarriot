using BuysimTechnology.Models;
using Microsoft.EntityFrameworkCore;

namespace BuysimTechnology.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Explicit primary keys
            modelBuilder.Entity<Merchant>().HasKey(m => m.MerchantId);
            modelBuilder.Entity<Invitation>().HasKey(i => i.InvitationId);
            modelBuilder.Entity<AuditLog>().HasKey(a => a.LogId);

            // PortalToken must be unique for each merchant
            modelBuilder.Entity<Merchant>()
                .HasIndex(m => m.PortalToken)
                .IsUnique();

            // Store enum as readable string
            modelBuilder.Entity<Invitation>()
                .Property(i => i.Status)
                .HasConversion<string>();

            // QR code is large — store as text
            modelBuilder.Entity<Invitation>()
                .Property(i => i.QrCodeBase64)
                .HasColumnType("nvarchar(max)");

            // Relationships
            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Merchant)
                .WithMany(m => m.Invitations)
                .HasForeignKey(i => i.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.Merchant)
                .WithMany(m => m.AuditLogs)
                .HasForeignKey(a => a.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
