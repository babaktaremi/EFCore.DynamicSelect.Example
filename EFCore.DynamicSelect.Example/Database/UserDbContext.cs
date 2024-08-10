using EFCore.DynamicSelect.Example.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.DynamicSelect.Example.Database;

public class UserDbContext:DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=localhost;Initial Catalog=EFCoreDynamicSelectDb;Integrated Security=true;Encrypt=False");
        }
    }

    public DbSet<UserEntity> Users => base.Set<UserEntity>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FirstName).HasMaxLength(100);
            builder.Property(c => c.LastName).HasMaxLength(100);
            builder.Property(c => c.UserName).HasMaxLength(100);
        });
    }
}