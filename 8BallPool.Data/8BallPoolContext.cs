using _8BallPool.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace _8BallPool.Data
{
    // use connection string from configuration
    public partial class _8BallPoolContext : DbContext
    {
        public DbSet<Player> Players { get; set; } = null!;

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
                // Additional property configurations can go here
            });
        }
        #endregion
    }
}