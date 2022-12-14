using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataManager.Common.POCOs;
using DataManager.Common.Abstractions;

namespace DataManager.Common
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid>, IEntity
    {
        public const string AUTH_SCHEMA_NAME = "Auth";

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            RegisterUserTables(builder);
        }

        private static void RegisterUserTables(ModelBuilder builder)
        {
            builder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.ToTable("Users", AUTH_SCHEMA_NAME);

                b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex").IsUnique();

                b.Property(u => u.UserName).HasMaxLength(256).IsRequired();
                b.Property(u => u.NormalizedUserName).HasMaxLength(256).IsRequired();
                b.Property(u => u.Email).HasMaxLength(256).IsRequired();
                b.Property(u => u.NormalizedEmail).HasMaxLength(256).IsRequired();
                b.Property(u => u.Sex).HasMaxLength(1);
                b.Property(e => e.BirthDate).IsRequired();
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.HasMany(r => r.Claims).WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
                b.HasMany(u => u.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
                b.HasMany<IdentityUserLogin<Guid>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
                b.HasMany<IdentityUserToken<Guid>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

                b.HasMany(u => u.RefreshTokens).WithOne().HasForeignKey(rt => rt.UserId).IsRequired();
            });

            builder.Entity<Role>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).ValueGeneratedOnAdd();
                b.ToTable("Roles", AUTH_SCHEMA_NAME);

                b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

                b.Property(r => r.Name).HasMaxLength(256).IsRequired();
                b.Property(r => r.NormalizedName).HasMaxLength(256).IsRequired();
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                b.HasMany(r => r.RoleClaims).WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
                b.HasMany(r => r.UserRoles).WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
            });

            builder.Entity<IdentityUserRole<Guid>>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.ToTable("UserRoles", AUTH_SCHEMA_NAME);
            });

            builder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.Property(uc => uc.Id).ValueGeneratedOnAdd();
                b.ToTable("UserClaims", AUTH_SCHEMA_NAME);
            });

            builder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.Property(rc => rc.Id).ValueGeneratedOnAdd();
                b.ToTable("RoleClaims", AUTH_SCHEMA_NAME);
            });

            builder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });
                b.ToTable("UserLogins", AUTH_SCHEMA_NAME);

                b.Property(l => l.LoginProvider).HasMaxLength(128);
                b.Property(l => l.ProviderKey).HasMaxLength(128);
            });

            builder.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
                b.ToTable("UserTokens", AUTH_SCHEMA_NAME);

                b.Property(t => t.LoginProvider).HasMaxLength(512);
                b.Property(t => t.Name).HasMaxLength(512);
            });

            builder.Entity<RefreshToken>(b =>
            {
                b.HasKey(rt => rt.Id);
                b.Property(rt => rt.Id).ValueGeneratedOnAdd();
                b.ToTable("RefreshTokens", AUTH_SCHEMA_NAME);

                b.HasOne(rt => rt.User).WithMany(u => u.RefreshTokens).HasForeignKey(rt => rt.UserId);
            });
        }
    }
}
