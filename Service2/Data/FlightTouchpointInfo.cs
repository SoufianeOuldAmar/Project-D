using System;
using System.Collections.Generic;

namespace Service2.Data;

public partial class FlightTouchpointInfo
{
    public int FlightId { get; set; }

    public int? TimetableId { get; set; }

    public string? FlightNumber { get; set; }

    public string TrafficType { get; set; } = null!;

    public DateTime ScheduledLocal { get; set; }

    public string? AirlineShortname { get; set; }

    public string AircraftType { get; set; } = null!;

    public string Airport { get; set; } = null!;

    public string Country { get; set; } = null!;

    public int PaxForecast { get; set; }

    public string Touchpoint { get; set; } = null!;

    public DateTime TouchpointTime { get; set; }

    public double TouchpointPax { get; set; }

    public DateTime ActualLocal { get; set; }

    public string? PaxActual { get; set; }

    public int Idactual { get; set; }
}
