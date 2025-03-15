using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DisasterPrediction.Api.Entities;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alert> Alerts { get; set; }

    public virtual DbSet<DisasterType> DisasterTypes { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.AlertId).HasName("PK__Alert__EBB16A8DDC731CC2");

            entity.ToTable("Alert");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RiskLevel)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.DisasterType).WithMany(p => p.Alerts)
                .HasForeignKey(d => d.DisasterTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Alert_DisasterType");
        });

        modelBuilder.Entity<DisasterType>(entity =>
        {
            entity.HasKey(e => e.DisasterTypeId).HasName("PK__Disaster__739351078420573B");

            entity.ToTable("DisasterType");

            entity.Property(e => e.DisasterTypeName)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RegionId)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Region).WithMany(p => p.DisasterTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DisasterType_Region");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__Region__ACD84443C37E7C1C");

            entity.ToTable("Region");

            entity.Property(e => e.RegionId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RegionID");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
