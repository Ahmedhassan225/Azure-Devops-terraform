using Domain.Models;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Context.Persistence
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
         : base(options)
        {
        }

        #region Entities

        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<UsersActivation> UsersActivations { get; set; }
        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Soft Delete Filter

            //modelBuilder.Entity<BaseEntity>()
            //    .HasQueryFilter(x => x.Deleted == false);

            #endregion

            #region Seeding Data

            //modelBuilder.Entity<Category>().HasData(
            //   new Category { Id = 1, Name = "C01", Description = "Category 01", Created = DateTime.Now, CreatedBy = "efcore seed" },
            //   new Category { Id = 2, Name = "C02", Description = "Category 02", Created = DateTime.Now, CreatedBy = "efcore seed" },
            //   new Category { Id = 3, Name = "C03", Description = "Category 03", Created = DateTime.Now, CreatedBy = "efcore seed" }
            //   );

            #endregion
        }

    }
}
