using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Models;

namespace WorkshopFlow.Data;

public class WorkshopFlowContext : DbContext
{

    public WorkshopFlowContext(DbContextOptions<WorkshopFlowContext> options)
        : base(options)
    {
    }

    public DbSet<Capability> Capabilities { get; set; }


    public DbSet<Role> Roles { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Capability>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name, "UQ_Capabilities_Name").IsUnique();
        });


        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.Capabilities).WithMany(p => p.Roles)
                .UsingEntity("RolesCapabilities", j =>
                {
                    j.HasIndex("CapabilitiesId")
                    .HasDatabaseName("IX_RolesCapabilities_CapabilityId");
                });
            //entity.HasIndex(e => e.Name, "IX_Roles_Name");
            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();
        });


        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Firstname).HasMaxLength(50);
            entity.Property(e => e.Lastname).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(60);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_RoleId");

            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();
            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");
            entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();
        });

        modelBuilder.Entity<UnitOfMeasure>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Symbol).HasMaxLength(10);

            entity.HasIndex(e => e.Name, "UQ_UnitOfMeasures_Name").IsUnique();
            entity.HasIndex(e => e.Symbol, "UQ_UnitOfMeasures_Symbol").IsUnique();
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.Property(e => e.ItemCode).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.StockQuantity).HasPrecision(18, 4);
            entity.Property(e => e.WeightPerUoM).HasPrecision(18, 4);
            entity.Property(e => e.Weight).HasPrecision(18, 4);

            // Το enum αποθηκεύεται ως string στη βάση — πιο readable
            entity.Property(e => e.ItemType)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Computed property — δεν αποθηκεύεται στη βάση
            entity.Ignore(e => e.IsManufactured);

            // FK προς UnitOfMeasure (stock UoM)
            entity.HasOne(d => d.UnitOfMeasure)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Items_UnitOfMeasureId");

            // FK προς UnitOfMeasure (weight UoM) — nullable
            entity.HasOne(d => d.WeightUoM)
                .WithMany(p => p.WeightItems)
                .HasForeignKey(d => d.WeightUoMId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Items_WeightUoMId");

            entity.HasIndex(e => e.ItemCode, "UQ_Items_ItemCode").IsUnique();
            entity.HasIndex(e => e.UnitOfMeasureId, "IX_Items_UnitOfMeasureId");
            entity.HasIndex(e => e.ItemType, "IX_Items_ItemType");
        });
    }
}
