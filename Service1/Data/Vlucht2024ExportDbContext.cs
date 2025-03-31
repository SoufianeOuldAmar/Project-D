using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Project_D.Data;

public partial class Vlucht2024ExportDbContext : DbContext
{
    public Vlucht2024ExportDbContext()
    {
    }

    public Vlucht2024ExportDbContext(DbContextOptions<Vlucht2024ExportDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ExportInfo> ExportInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-JLBRQTRK\\SQLEXPRESS;Database=Vlucht2024ExportDb;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<ExportInfo>(entity =>
        {
            entity.HasKey(e => e.UniqueId);

            entity.ToTable("ExportInfo");

            entity.Property(e => e.UniqueId)
                .ValueGeneratedNever()
                .HasColumnName("UniqueID");
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
            entity.Property(e => e.Bagageband).HasMaxLength(50);
            entity.Property(e => e.Bags).HasMaxLength(50);
            entity.Property(e => e.BagsTransit).HasMaxLength(50);
            entity.Property(e => e.BagsTransitWeight).HasMaxLength(50);
            entity.Property(e => e.BagsWeight).HasMaxLength(50);
            entity.Property(e => e.Bewegingen).HasMaxLength(50);
            entity.Property(e => e.Bus).HasMaxLength(50);
            entity.Property(e => e.Column67)
                .HasMaxLength(1)
                .HasColumnName("column67");
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CrewCabin).HasMaxLength(50);
            entity.Property(e => e.CrewCockpit).HasMaxLength(50);
            entity.Property(e => e.Datasource).HasMaxLength(50);
            entity.Property(e => e.Debiteur).HasMaxLength(50);
            entity.Property(e => e.Direction).HasMaxLength(50);
            entity.Property(e => e.Diverted).HasMaxLength(50);
            entity.Property(e => e.Eu)
                .HasMaxLength(50)
                .HasColumnName("EU");
            entity.Property(e => e.Feestdag).HasMaxLength(50);
            entity.Property(e => e.FlightClass).HasMaxLength(50);
            entity.Property(e => e.FlightCode).HasMaxLength(50);
            entity.Property(e => e.FlightCodeDescription).HasMaxLength(50);
            entity.Property(e => e.FlightCodeIata)
                .HasMaxLength(50)
                .HasColumnName("FlightCodeIATA");
            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.FlightNumber).HasMaxLength(50);
            entity.Property(e => e.Forecast).HasMaxLength(50);
            entity.Property(e => e.Forecast1).HasMaxLength(1);
            entity.Property(e => e.ForecastBabys)
                .HasMaxLength(50)
                .HasColumnName("Forecast_babys");
            entity.Property(e => e.ForecastPax)
                .HasMaxLength(50)
                .HasColumnName("Forecast_Pax");
            entity.Property(e => e.Gate).HasMaxLength(50);
            entity.Property(e => e.Mtow).HasColumnName("MTOW");
            entity.Property(e => e.Nachtvlucht).HasMaxLength(50);
            entity.Property(e => e.Parked)
                .HasMaxLength(50)
                .HasColumnName("parked");
            entity.Property(e => e.Parkeercontract).HasMaxLength(1);
            entity.Property(e => e.Parkeerpositie).HasMaxLength(50);
            entity.Property(e => e.PaxChild).HasMaxLength(50);
            entity.Property(e => e.PaxFemale).HasMaxLength(50);
            entity.Property(e => e.PaxInfant).HasMaxLength(50);
            entity.Property(e => e.PaxMale).HasMaxLength(50);
            entity.Property(e => e.PaxTransitChild).HasMaxLength(50);
            entity.Property(e => e.PaxTransitFemale).HasMaxLength(50);
            entity.Property(e => e.PaxTransitInfant).HasMaxLength(50);
            entity.Property(e => e.PaxTransitMale).HasMaxLength(50);
            entity.Property(e => e.PublicAnnouncement).HasMaxLength(50);
            entity.Property(e => e.ScheduledLocal).HasPrecision(0);
            entity.Property(e => e.ScheduledUtc)
                .HasPrecision(0)
                .HasColumnName("ScheduledUTC");
            entity.Property(e => e.Schengen).HasMaxLength(50);
            entity.Property(e => e.Seizoen).HasMaxLength(50);
            entity.Property(e => e.TerminalBags).HasMaxLength(50);
            entity.Property(e => e.TerminalBagsWeight).HasMaxLength(50);
            entity.Property(e => e.TerminalCrew).HasMaxLength(50);
            entity.Property(e => e.TerminalPax).HasMaxLength(50);
            entity.Property(e => e.TerminalPaxBetalend).HasMaxLength(50);
            entity.Property(e => e.TimetableId).HasColumnName("TimetableID");
            entity.Property(e => e.TotaalBags).HasMaxLength(50);
            entity.Property(e => e.TotaalBagsWeight).HasMaxLength(50);
            entity.Property(e => e.TotaalCrew).HasMaxLength(50);
            entity.Property(e => e.TotaalPax).HasMaxLength(50);
            entity.Property(e => e.TotaalPaxBetalend).HasMaxLength(50);
            entity.Property(e => e.TrafficType).HasMaxLength(50);
            entity.Property(e => e.TransitBags).HasMaxLength(50);
            entity.Property(e => e.TransitBagsWeight).HasMaxLength(50);
            entity.Property(e => e.TransitPax).HasMaxLength(50);
            entity.Property(e => e.TransitPaxBetalend).HasMaxLength(50);
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
