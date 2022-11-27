using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataManager.Common.POCOs;

namespace DataManager.Common
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid>
    {
        public const string AUTH_SCHEMA_NAME = "Auth";

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            RegisterUserTables(builder);
        }

        private static void RegisterUserTables(ModelBuilder builder)
        {
            builder.Entity<User>(b =>
            {
                b.ToTable("Users", AUTH_SCHEMA_NAME);

                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                b.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<Role>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
                b.ToTable("Roles", AUTH_SCHEMA_NAME);

                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany(e => e.RoleClaims)
                    .WithOne()
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
                
                b.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<IdentityUserRole<Guid>>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.ToTable("UserRoles", AUTH_SCHEMA_NAME);
            });

            builder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("UserClaims", AUTH_SCHEMA_NAME);
            });

            builder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("RoleClaims", AUTH_SCHEMA_NAME);
            });
        }
    }
}
