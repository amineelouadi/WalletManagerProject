using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WalletManager.Domain.Entities;
using WalletManager.Domain.Enums;

namespace WalletManager.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; } = default!;
        public DbSet<Transaction> Transactions { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User entity
            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            });

            // Configure Wallet entity
            builder.Entity<Wallet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.Currency).HasMaxLength(3).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.UserId).IsRequired();

                // Relationship with User
                entity.HasOne(e => e.User)
                      .WithMany(e => e.Wallets)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Create index on UserId
                entity.HasIndex(e => e.UserId);
            });

            // Configure Transaction entity
            builder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.Description).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.TransactionDate).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.Reference).HasMaxLength(50);

                // Relationship with Wallet
                entity.HasOne(e => e.Wallet)
                      .WithMany(e => e.Transactions)
                      .HasForeignKey(e => e.WalletId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Create indexes
                entity.HasIndex(e => e.WalletId);
                entity.HasIndex(e => e.TransactionDate);
                entity.HasIndex(e => e.Type);
            });

            // Seed roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = UserRole.Admin, NormalizedName = UserRole.Admin.ToUpper() },
                new IdentityRole { Id = "2", Name = UserRole.User, NormalizedName = UserRole.User.ToUpper() }
            );
        }
    }
}
