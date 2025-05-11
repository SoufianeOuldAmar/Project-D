using System;
using System.Collections.Generic;

namespace Project_D.Data;

public partial class FlightExportInfo
{
    public string Type { get; set; } = null!;

    public int FlightId { get; set; }

    public int? TimetableId { get; set; }

    public string TrafficType { get; set; } = null!;

    public string? FlightNumber { get; set; }

    public bool Diverted { get; set; }

    public bool Nachtvlucht { get; set; }

    public int FlightCode { get; set; }

    public string FlightCodeDescription { get; set; } = null!;

    public string FlightCodeIata { get; set; } = null!;

    public bool PublicAnnouncement { get; set; }

    public DateTime ScheduledUtc { get; set; }

    public DateTime ActualUtc { get; set; }

    public DateTime ScheduledLocal { get; set; }

    public DateTime ActualLocal { get; set; }

    public int Bewegingen { get; set; }

    public string Parkeerpositie { get; set; } = null!;

    public string? Parkeercontract { get; set; }

    public string? Bus { get; set; }

    public int? Gate { get; set; }

    public int? Bagageband { get; set; }

    public string AirportIcao { get; set; } = null!;

    public string Airport { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string? ViaAirportIcao { get; set; }

    public string? ViaAirport { get; set; }

    public string AircraftRegistration { get; set; } = null!;

    public int Seats { get; set; }

    public int Mtow { get; set; }

    public string AircraftType { get; set; } = null!;

    public string AircraftDescription { get; set; } = null!;

    public bool Eu { get; set; }

    public bool Schengen { get; set; }

    public string? AirlineFullname { get; set; }

    public string? AirlineShortname { get; set; }

    public string? AirlineIcao { get; set; }

    public string? AirlineIata { get; set; }

    public string Debiteur { get; set; } = null!;

    public int DebiteurNr { get; set; }

    public int PaxMale { get; set; }

    public int PaxFemale { get; set; }

    public int PaxChild { get; set; }

    public int PaxInfant { get; set; }

    public int PaxTransitMale { get; set; }

    public int PaxTransitFemale { get; set; }

    public int PaxTransitChild { get; set; }

    public int PaxTransitInfant { get; set; }

    public int CrewCabin { get; set; }

    public int CrewCockpit { get; set; }

    public int BagsWeight { get; set; }

    public int BagsTransitWeight { get; set; }

    public int Bags { get; set; }

    public int BagsTransit { get; set; }

    public string Afhandelaar { get; set; } = null!;

    public double ForecastPercent { get; set; }

    public int ForecastPax { get; set; }

    public double ForecastBabys { get; set; }

    public string FlightClass { get; set; } = null!;

    public string Datasource { get; set; } = null!;

    public int TotaalPax { get; set; }

    public int TerminalPax { get; set; }

    public int TotaalPaxBetalend { get; set; }

    public int TerminalPaxBetalend { get; set; }

    public int TransitPax { get; set; }

    public int TransitPaxBetalend { get; set; }

    public int TotaalCrew { get; set; }

    public string? EmptyCol67 { get; set; }

    public int TerminalCrew { get; set; }

    public int TotaalSeats { get; set; }

    public int TerminalSeats { get; set; }

    public int TotaalBags { get; set; }

    public int TerminalBags { get; set; }

    public int TransitBags { get; set; }

    public int TotaalBagsWeight { get; set; }

    public int TerminalBagsWeight { get; set; }

    public int TransitBagsWeight { get; set; }

    public int Runway { get; set; }

    public double? Longitude { get; set; }

    public double? Elevation { get; set; }

    public double? Latitude { get; set; }

    public double? DistanceKilometers { get; set; }

    public string? Direction { get; set; }

    public string? AirportIata { get; set; }

    public int? Forecast { get; set; }

    public bool? Parked { get; set; }

    public string Seizoen { get; set; } = null!;

    public string? Feestdag { get; set; }

    public short Idactual { get; set; }
}
