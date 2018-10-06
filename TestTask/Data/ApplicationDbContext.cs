using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TestTask.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TestTask.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {

        public DbSet<Models.Task> Tasks { get; set; }

        public DbQuery<TaskWithSubtreePath> taskWithSubtreePaths { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(200);

                entity.Property(m => m.ProviderKey).HasMaxLength(200);
            });

            builder.Entity<IdentityUserRole<string>>().Property(r => r.RoleId).HasMaxLength(36);

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(e => e.LoginProvider).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(200);
            });

            builder.Entity<User>().HasMany<Models.Task>().WithOne(t => t.User).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Models.Task>().HasOne<Models.Task>().WithMany().HasForeignKey(t => t.ParentTaskId)
                .IsRequired(false).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Models.Task>().Property(t => t.Status).HasConversion(new EnumToStringConverter<Models.TaskStatus>());
        }

    }
}
