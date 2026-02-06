using BackendAuthAssignment.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAuthAssignment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<OtpRequest> OtpRequests { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // USER
    modelBuilder.Entity<User>()
        .HasIndex(u => u.PhoneNumber)
        .IsUnique();

    modelBuilder.Entity<User>()
        .HasOne(u => u.Profile)
        .WithOne(p => p.User)
        .HasForeignKey<UserProfile>(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // USER PROFILE
    modelBuilder.Entity<UserProfile>()
        .HasIndex(p => p.UserId)
        .IsUnique();

    modelBuilder.Entity<UserProfile>()
        .Property(p => p.LocationJson)
        .HasColumnType("jsonb");

    // OTP
    modelBuilder.Entity<OtpRequest>()
        .HasIndex(o => new { o.PhoneNumber, o.CreatedAt });

    // SESSION
    modelBuilder.Entity<Session>()
        .HasIndex(s => s.UserId);

    base.OnModelCreating(modelBuilder);
}

    }
}
