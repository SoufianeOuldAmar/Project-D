using Microsoft.EntityFrameworkCore;
using Service2.Data;

namespace TouchpointServicexUnitTest
{
    public class TouchpointControllerTest
    {
        private FlightTouchpointDbContextTest CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<FlightTouchpointDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new FlightTouchpointDbContextTest(options);
            context.Database.EnsureCreated();
            return context;
        }

        private void SeedFlights(FlightTouchpointDbContext context)
        {
            if (context.FlightTouchpointInfos.Any())
            {
                context.FlightTouchpointInfos.RemoveRange(context.FlightTouchpointInfos);
                context.SaveChanges();
            }

            var flights = new List<FlightTouchpointInfo>
            {
                new FlightTouchpointInfo
                {
                    FlightId = 585146,
                    TimetableId = 609902,
                    FlightNumber = "PGT1261",
                    TrafficType = "A",
                    ScheduledLocal = new DateTime(2024, 01, 01, 13, 25, 00),
                    AirlineShortname = "PEGASUS",
                    AircraftType = "A320N",
                    Airport = "Istanbul",
                    Country = "Turkey",
                    PaxForecast = 175,
                    Touchpoint = "Aankomsthal",
                    TouchpointTime = new DateTime(2024, 01, 01, 13, 35, 00),
                    TouchpointPax = 175,
                    ActualLocal =  new DateTime(2024, 01, 01, 13, 25, 00),
                    PaxActual = null
                },
                new FlightTouchpointInfo
                {
                    FlightId = 585146,
                    TimetableId = 609902,
                    FlightNumber = "PGT1261",
                    TrafficType = "A",
                    ScheduledLocal = new DateTime(2024, 01, 01, 13, 25, 00),
                    AirlineShortname = "PEGASUS",
                    AircraftType = "A320N",
                    Airport = "Istanbul",
                    Country = "Turkey",
                    PaxForecast = 175,
                    Touchpoint = "Aankomsthal",
                    TouchpointTime = new DateTime(2024, 01, 01, 14, 05, 00),
                    TouchpointPax = 35,
                    ActualLocal = new DateTime(2024, 01, 01, 13, 35, 00),
                    PaxActual = null
                },
                new FlightTouchpointInfo
                {
                    FlightId = 585137,
                    TimetableId = 607143,
                    FlightNumber = "TRA5592",
                    TrafficType = "A",
                    ScheduledLocal = new DateTime(2024, 01, 01, 14, 40, 00),
                    AirlineShortname = "TRANSAVIA",
                    AircraftType = "B737W8",
                    Airport = "Al-Hoceima",
                    Country = "Marokko",
                    PaxForecast = 186,
                    Touchpoint = "Aankomsthal",
                    TouchpointTime = new DateTime(2024, 01, 01, 14, 50, 00),
                    TouchpointPax = 35,
                    ActualLocal = new DateTime(2024, 01, 01, 14, 40, 00),
                    PaxActual = null
                },
                new FlightTouchpointInfo
                {
                    FlightId = 585147,
                    TimetableId = 610340,
                    FlightNumber = "JAF2647",
                    TrafficType = "A",
                    ScheduledLocal = new DateTime(2024, 01, 01, 15, 30, 00),
                    AirlineShortname = "TUI FLY",
                    AircraftType = "B737S8",
                    Airport = "Oujda",
                    Country = "Marokko",
                    PaxForecast = 138,
                    Touchpoint = "Aankomsthal",
                    TouchpointTime = new DateTime(2024, 01, 01, 15, 50, 00),
                    TouchpointPax = 110.400001525879,
                    ActualLocal = new DateTime(2024, 01, 01, 15, 30, 00),
                    PaxActual = null
                }
            };

            context.FlightTouchpointInfos.AddRange(flights);
            context.SaveChanges();
        }

    }
}
