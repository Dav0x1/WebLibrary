using Microsoft.EntityFrameworkCore;

namespace WebLibrary.Models
{
    public class WLDbContext : DbContext
    {
        public WLDbContext(DbContextOptions<WLDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasOne(e => e.UserType)
                .WithMany(e => e.Users);

            modelBuilder.Entity<TagModel>()
                .HasMany(e => e.Books)
                .WithMany(e => e.Tags);

            modelBuilder.Entity<OrderHistoryModel>()
                .HasOne(e => e.User)
                .WithMany(e => e.OrderHistory);

            modelBuilder.Entity<OrderHistoryModel>()
                .HasMany(o => o.Books)
                .WithMany(e=>e.OrderHistory);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserTypeModel> UserTypes { get; set; }
        public DbSet<NewsModel> News { get; set; }
        public DbSet<BookModel> Books { get; set; }
        public DbSet<OrderHistoryModel> OrderHistory { get; set; }
        public DbSet<TagModel> Tags { get; set; }
    }
}