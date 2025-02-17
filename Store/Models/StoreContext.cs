using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Store.Models;

public partial class StoreContext : DbContext
{
    public StoreContext(DbContextOptions<StoreContext> options)
        : base(options)
    {
        try
        {
            var dbCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (dbCreator != null)
            {
                if (!dbCreator.CanConnect())
                    dbCreator.Create();
                if (!dbCreator.HasTables())
                    dbCreator.CreateTables();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("PK__Category__CBD7470613D06136");

            entity.Property(e => e.NameCategory).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Products__2E8946D441AAADE3");

            entity.Property(e => e.NameProduct).HasMaxLength(150);
            entity.Property(e => e.PhotoUrl).HasColumnName("PhotoURL");
            entity.Property(e => e.Price).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categories_Products");
        });

        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Action__3214EC07BA4C2A33");

            entity.Property(e => e.Controller).HasMaxLength(50);
            entity.Property(e => e.ActionDate).HasColumnType("datetime");
            entity.Property(e => e.Ip).HasMaxLength(50);
            entity.Property(e => e.ActionName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07CF9C1A4E");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
