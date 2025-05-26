using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Project_D.Data;

public partial class FlightExportDbContext : DbContext
{
    public FlightExportDbContext()
    {
    }

    public FlightExportDbContext(DbContextOptions<FlightExportDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FlightExportInfo> FlightExportInfos { get; set; }

    //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //         => optionsBuilder.UseSqlServer("Server=Shanaya\\SQLEXPRESS;Database=FlightExportDb;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlightExportInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FlightExportInfo");

            entity.Property(e => e.ActualLocal).HasPrecision(0);
            entity.Property(e => e.ActualUtc)
                .HasPrecision(0)
                .HasColumnName("ActualUTC");
            entity.Property(e => e.Afhandelaar).HasMaxLength(50);
            entity.Property(e => e.AircraftDescription).HasMaxLength(50);
            entity.Property(e => e.AircraftRegistration).HasMaxLength(50);
            entity.Property(e => e.AircraftType).HasMaxLength(50);
            entity.Property(e => e.AirlineFullname).HasMaxLength(50);
            entity.Property(e => e.AirlineIata)
                .HasMaxLength(50)
                .HasColumnName("AirlineIATA");
            entity.Property(e => e.AirlineIcao)
                .HasMaxLength(50)
                .HasColumnName("AirlineICAO");
            entity.Property(e => e.AirlineShortname).HasMaxLength(50);
            entity.Property(e => e.Airport).HasMaxLength(50);
            entity.Property(e => e.AirportIata)
                .HasMaxLength(50)
                .HasColumnName("AirportIATA");
            entity.Property(e => e.AirportIcao)
                .HasMaxLength(50)
                .HasColumnName("AirportICAO");
            entity.Property(e => e.Bus).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Datasource).HasMaxLength(50);
            entity.Property(e => e.Debiteur).HasMaxLength(50);
            entity.Property(e => e.Direction).HasMaxLength(50);
            entity.Property(e => e.EmptyCol67).HasMaxLength(1);
            entity.Property(e => e.Eu).HasColumnName("EU");
            entity.Property(e => e.Feestdag).HasMaxLength(50);
            entity.Property(e => e.FlightClass).HasMaxLength(50);
            entity.Property(e => e.FlightCodeDescription).HasMaxLength(50);
            entity.Property(e => e.FlightCodeIata)
                .HasMaxLength(50)
                .HasColumnName("FlightCodeIATA");
            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.FlightNumber).HasMaxLength(50);
            entity.Property(e => e.ForecastBabys).HasColumnName("Forecast_babys");
            entity.Property(e => e.ForecastPax).HasColumnName("Forecast_Pax");
            entity.Property(e => e.Mtow).HasColumnName("MTOW");
            entity.Property(e => e.Parked).HasColumnName("parked");
            entity.Property(e => e.Parkeercontract).HasMaxLength(1);
            entity.Property(e => e.Parkeerpositie).HasMaxLength(50);
            entity.Property(e => e.ScheduledLocal).HasPrecision(0);
            entity.Property(e => e.ScheduledUtc)
                .HasPrecision(0)
                .HasColumnName("ScheduledUTC");
            entity.Property(e => e.Seizoen).HasMaxLength(50);
            entity.Property(e => e.TimetableId).HasColumnName("TimetableID");
            entity.Property(e => e.TrafficType).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.ViaAirport).HasMaxLength(50);
            entity.Property(e => e.ViaAirportIcao)
                .HasMaxLength(50)
                .HasColumnName("ViaAirportICAO");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
