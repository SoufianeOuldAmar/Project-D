using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Service2.Data;

public partial class Vlucht2024TouchpointDbContext : DbContext
{
    public Vlucht2024TouchpointDbContext()
    {
    }

    public Vlucht2024TouchpointDbContext(DbContextOptions<Vlucht2024TouchpointDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TouchpointInfo> TouchpointInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-JLBRQTRK\\SQLEXPRESS;Database=Vlucht2024TouchpointDb;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<TouchpointInfo>(entity =>
        {
            entity.HasKey(e => e.UniqueId);

            entity.ToTable("TouchpointInfo");

            entity.Property(e => e.UniqueId)
                .ValueGeneratedNever()
                .HasColumnName("UniqueID");
            entity.Property(e => e.ActualLocal).HasPrecision(6);
            entity.Property(e => e.AircraftType).HasMaxLength(50);
            entity.Property(e => e.AirlineShortname).HasMaxLength(50);
            entity.Property(e => e.Airport).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.FlightNumber).HasMaxLength(50);
            entity.Property(e => e.PaxActual).HasMaxLength(50);
            entity.Property(e => e.ScheduledLocal).HasPrecision(6);
            entity.Property(e => e.TimetableId).HasColumnName("TimetableID");
            entity.Property(e => e.Touchpoint).HasMaxLength(50);
            entity.Property(e => e.TrafficType).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
