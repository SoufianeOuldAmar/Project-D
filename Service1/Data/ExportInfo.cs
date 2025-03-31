using System;
using System.Collections.Generic;

namespace Project_D.Data;

public partial class ExportInfo
{
    public string? Type { get; set; }

    public int? FlightId { get; set; }

    public int? TimetableId { get; set; }

    public string? TrafficType { get; set; }

    public string? FlightNumber { get; set; }

    public string? Diverted { get; set; }

    public string? Nachtvlucht { get; set; }

    public string? FlightCode { get; set; }

    public string? FlightCodeDescription { get; set; }

    public string? FlightCodeIata { get; set; }

    public string? PublicAnnouncement { get; set; }

    public DateTime? ScheduledUtc { get; set; }

    public DateTime? ActualUtc { get; set; }

    public DateTime? ScheduledLocal { get; set; }

    public DateTime? ActualLocal { get; set; }

    public string? Bewegingen { get; set; }

    public string? Parkeerpositie { get; set; }

    public string? Parkeercontract { get; set; }

    public string? Bus { get; set; }

    public string? Gate { get; set; }

    public string? Bagageband { get; set; }

    public string? AirportIcao { get; set; }

    public string? Airport { get; set; }

    public string? Country { get; set; }

    public string? ViaAirportIcao { get; set; }

    public string? ViaAirport { get; set; }

    public string? AircraftRegistration { get; set; }

    public int? Seats { get; set; }

    public int? Mtow { get; set; }

    public string? AircraftType { get; set; }

    public string? AircraftDescription { get; set; }

    public string? Eu { get; set; }

    public string? Schengen { get; set; }

    public string? AirlineFullname { get; set; }

    public string? AirlineShortname { get; set; }

    public string? AirlineIcao { get; set; }

    public string? AirlineIata { get; set; }

    public string? Debiteur { get; set; }

    public int? DebiteurNr { get; set; }

    public string? PaxMale { get; set; }

    public string? PaxFemale { get; set; }

    public string? PaxChild { get; set; }

    public string? PaxInfant { get; set; }

    public string? PaxTransitMale { get; set; }

    public string? PaxTransitFemale { get; set; }

    public string? PaxTransitChild { get; set; }

    public string? PaxTransitInfant { get; set; }

    public string? CrewCabin { get; set; }

    public string? CrewCockpit { get; set; }

    public string? BagsWeight { get; set; }

    public string? BagsTransitWeight { get; set; }

    public string? Bags { get; set; }

    public string? BagsTransit { get; set; }

    public string? Afhandelaar { get; set; }

    public string? Forecast { get; set; }

    public string? ForecastPax { get; set; }

    public string? ForecastBabys { get; set; }

    public string? FlightClass { get; set; }

    public string? Datasource { get; set; }

    public string? TotaalPax { get; set; }

    public string? TerminalPax { get; set; }

    public string? TotaalPaxBetalend { get; set; }

    public string? TerminalPaxBetalend { get; set; }

    public string? TransitPax { get; set; }

    public string? TransitPaxBetalend { get; set; }

    public string? TotaalCrew { get; set; }

    public string? Column67 { get; set; }

    public string? TerminalCrew { get; set; }

    public int? TotaalSeats { get; set; }

    public int? TerminalSeats { get; set; }

    public string? TotaalBags { get; set; }

    public string? TerminalBags { get; set; }

    public string? TransitBags { get; set; }

    public string? TotaalBagsWeight { get; set; }

    public string? TerminalBagsWeight { get; set; }

    public string? TransitBagsWeight { get; set; }

    public int? Runway { get; set; }

    public double? Longitude { get; set; }

    public double? Elevation { get; set; }

    public double? Latitude { get; set; }

    public double? DistanceKilometers { get; set; }

    public string? Direction { get; set; }

    public string? AirportIata { get; set; }

    public string? Forecast1 { get; set; }

    public string? Parked { get; set; }

    public string? Seizoen { get; set; }

    public string? Feestdag { get; set; }

    public int UniqueId { get; set; }
}
