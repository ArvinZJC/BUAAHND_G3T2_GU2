#region Using Directives
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        } // end constructor ApplicationDbContext

        public DbSet<AddressBook> AddressBooks { get; set; }

        public DbSet<Bouquet> Bouquets { get; set; }

        public DbSet<CartDetail> CartDetails { get; set; }

        public DbSet<Colour> Colours { get; set; }

        public DbSet<Flower> Flowers { get; set; }

        public DbSet<Occasion> Occasions { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<SalesRecord> SalesRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>(typeBuilder =>
            {
                typeBuilder.ToTable("Users");
                typeBuilder.Property(user => user.UserName).IsRequired();
                typeBuilder.Property(user => user.UserName).HasMaxLength(25);
                typeBuilder.Property(user => user.NormalizedUserName).IsRequired();
                typeBuilder.Property(user => user.NormalizedUserName).HasMaxLength(25);
                typeBuilder.Property(user => user.Email).IsRequired();
                typeBuilder.Property(user => user.Email).HasMaxLength(50);
                typeBuilder.Property(user => user.NormalizedEmail).IsRequired();
                typeBuilder.Property(user => user.NormalizedEmail).HasMaxLength(50);
                typeBuilder.Property(user => user.PasswordHash).IsRequired();
                typeBuilder.Property(user => user.SecurityStamp).IsRequired();
                typeBuilder.Property(user => user.ConcurrencyStamp).IsRequired();
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(typeBuilder =>
            {
                typeBuilder.ToTable("UserLogins");
                typeBuilder.Property(userLogin => userLogin.ProviderDisplayName).HasMaxLength(25);
            });
            modelBuilder.Entity<IdentityRole>(typeBuilder =>
            {
                typeBuilder.ToTable("Roles");
                typeBuilder.Property(role => role.Name).HasMaxLength(25);
                typeBuilder.Property(role => role.NormalizedName).HasMaxLength(25);
            });
            modelBuilder.Entity<IdentityUserToken<string>>(typeBuilder => typeBuilder.ToTable("UserTokens"));
            modelBuilder.Entity<IdentityUserClaim<string>>(typeBuilder => typeBuilder.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityRoleClaim<string>>(typeBuilder => typeBuilder.ToTable("RoleClaims"));
            modelBuilder.Entity<IdentityUserRole<string>>(typeBuilder => typeBuilder.ToTable("UserRoles"));
        } // end method OnModelCreating
    } // end class ApplicationDbContext
} // end namespace NewEraFlowerStore.Data