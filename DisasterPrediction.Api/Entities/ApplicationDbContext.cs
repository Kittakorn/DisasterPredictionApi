using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DisasterPrediction.Api.Entities;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

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
            entity.HasKey(e => e.AlertId).HasName("PK__Alert__EBB16A8DE504A6A3");

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
            entity.HasKey(e => e.DisasterTypeId).HasName("PK__Disaster__73935107C5747480");

            entity.ToTable("DisasterType");

            entity.Property(e => e.DisasterTypeName)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RegionId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Region).WithMany(p => p.DisasterTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DisasterType_Region");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__tmp_ms_x__ACD84443FD9605C2");

            entity.ToTable("Region");

            entity.Property(e => e.RegionId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RegionID");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
