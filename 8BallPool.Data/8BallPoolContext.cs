using _8BallPool.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace _8BallPool.Data
{
    // use connection string from configuration
    public partial class _8BallPoolContext : DbContext
    {
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;

        public _8BallPoolContext(DbContextOptions<_8BallPoolContext> options)
        : base(options)
        {
        }
        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Auth0_id).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Profile_picture_url).IsRequired().HasMaxLength(300);
            });
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Configure StartTime as required
                entity.Property(e => e.StartTime).IsRequired();
                
                // Configure EndTime as optional
                entity.Property(e => e.EndTime).IsRequired(false);
                
                // Configure TableNumber as optional
                entity.Property(e => e.TableNumber).IsRequired(false);
                
                // Configure Player1 relationship (required one-to-many)
                entity.HasOne(m => m.Player1)
                    .WithMany(p => p.MatchesAsPlayer1)
                    .HasForeignKey(m => m.Player1Id)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
                
                // Configure Player2 relationship (required one-to-many)
                entity.HasOne(m => m.Player2)
                    .WithMany(p => p.MatchesAsPlayer2)
                    .HasForeignKey(m => m.Player2Id)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
                
                // Configure Winner relationship (optional one-to-many)
                entity.HasOne(m => m.Winner)
                    .WithMany(p => p.MatchesWon)
                    .HasForeignKey(m => m.WinnerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });
        }
        #endregion
    }
}