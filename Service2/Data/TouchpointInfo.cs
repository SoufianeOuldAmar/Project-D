using System;
using System.Collections.Generic;

namespace Service2.Data;

public partial class TouchpointInfo
{
    public int? FlightId { get; set; }

    public int? TimetableId { get; set; }

    public string? FlightNumber { get; set; }

    public string? TrafficType { get; set; }

    public DateTime? ScheduledLocal { get; set; }

    public string? AirlineShortname { get; set; }

    public string? AircraftType { get; set; }

    public string? Airport { get; set; }

    public string? Country { get; set; }

    public int? PaxForecast { get; set; }

    public string? Touchpoint { get; set; }

    public DateTime? TouchpointTime { get; set; }

    public double? TouchpointPax { get; set; }

    public DateTime? ActualLocal { get; set; }

    public string? PaxActual { get; set; }

    public int UniqueId { get; set; }
}
