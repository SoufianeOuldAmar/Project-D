using FlightExportsService.Controllers.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Project_D.Data;

namespace ExportServicexUnitTest
{
    public class FakeUrlHelper : IUrlHelper
    {
        public ActionContext ActionContext { get; set; }

        public string Action(UrlActionContext context)
        {
            if (context.Values != null)
            {
                var val = context.Values;
                var type = val.GetType();
                var prop = type.GetProperty("flightId") ?? type.GetProperty("FlightId");
                if (prop != null)
                {
                    var id = prop.GetValue(val);
                    return $"/api/v1/flight-exports/by-flight-id?flightId={id}";
                }
            }
            return "/dummy";
        }

        // Unused members
        public string Content(string contentPath) => throw new NotImplementedException();
        public bool IsLocalUrl(string url) => throw new NotImplementedException();
        public string Link(string routeName, object values) => throw new NotImplementedException();
        public string RouteUrl(UrlRouteContext routeContext) => throw new NotImplementedException();
    }

    public class FlightExportControllerTest
    {
        private FlightExportDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<FlightExportDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new TestFlightExportDbContext(options);

            context.Database.EnsureCreated();
            return context;
        }

        private FlightExportsController CreateController(FlightExportDbContext context)
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var controller = new FlightExportsController(context, memoryCache);

            controller.Url = new FakeUrlHelper();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        private void SeedFlights(FlightExportDbContext context)
        {
            if (context.FlightExportInfos.Any())
            {
                context.FlightExportInfos.RemoveRange(context.FlightExportInfos);
                context.SaveChanges();
            }

            var flights = new List<FlightExportInfo>
    {
        new FlightExportInfo
        {
            // --- REQUIRED NON‐NULLABLE FIELDS (strings) ---
            Type                   = "Rolling",
            TrafficType            = "D",
            FlightCodeDescription  = "Geregeld/lijnvlucht",
            FlightCodeIata         = "J",
            Parkeerpositie         = "D1",
            AirportIcao            = "LEMG",
            Country                = "Spain",
            AircraftRegistration   = "PH-HSF",
            AircraftType           = "B737W8",
            AircraftDescription    = "Boeing B737-800 Winglets",
            Datasource             = "CA_1",
            Debiteur               = "Transavia",
            FlightClass            = "A",
            Afhandelaar            = "Aviapartner",
            Seizoen                = "W23",           

            // --- NULLABLE or VALUE‐TYPE FIELDS (from your original seed) ---
            FlightId        = 585150,
            AirlineFullname = "Transavia",
            Airport         = "Malaga",
            AirportIata     = "AGP",
            ScheduledLocal  = new DateTime(2024, 01, 01, 07, 30, 00),
            ActualLocal     = new DateTime(2024, 01, 01, 07, 51, 00),

            Diverted        = false,
            Nachtvlucht     = false,
            Seats           = 189,
            Eu              = true,

            PaxMale         = 62,
            PaxFemale       = 68,
            PaxInfant       = 2,
            PaxChild        = 8,
            TotaalPax       = 140,
            TerminalPax     = 140,
            TransitPax      = 0,

            TotaalBagsWeight   = 66,
            TerminalBagsWeight = 66,
            TransitBagsWeight  = 0,
            TotaalBags         = 0,
            TerminalBags       = 0,
            TransitBags        = 0
        },
        new FlightExportInfo
        {
            // --- REQUIRED NON‐NULLABLE FIELDS (strings) ---
            Type                   = "Rolling",
            TrafficType            = "A",
            FlightCodeDescription  = "Geregeld/lijnvlucht",
            FlightCodeIata         = "P",
            Parkeerpositie         = "B1",
            AirportIcao            = "LTFJ",
            Country                = "Istanbul",
            AircraftRegistration   = "TC-NBI",
            AircraftType           = "A320N",
            AircraftDescription    = "Airbus A320-200 NEO",
            Datasource             = "CA_1",
            Debiteur               = "Pegasus Airlines",
            FlightClass            = "A",
            Afhandelaar            = "Aviapartner",
            Seizoen                = "W23",

            // --- NULLABLE or VALUE‐TYPE FIELDS ---
            FlightId        = 585155,
            AirlineFullname = "Pegasus",
            Airport         = "Istanbul",
            AirportIata     = "SAW",
            ScheduledLocal  = new DateTime(2024, 02, 02, 13, 25, 00),
            ActualLocal     = new DateTime(2024, 02, 02, 13, 14, 00),

            Diverted        = false,
            Nachtvlucht     = false,
            Seats           = 186,
            Eu              = false,

            PaxMale         = 96,
            PaxFemale       = 67,
            PaxInfant       = 3,
            PaxChild        = 9,
            TotaalPax       = 175,
            TerminalPax     = 175,
            TransitPax      = 0,

            TotaalBagsWeight   = 0,
            TerminalBagsWeight = 0,
            TransitBagsWeight  = 0,
            TotaalBags         = 0,
            TerminalBags       = 0,
            TransitBags        = 0
        }
    };

            context.FlightExportInfos.AddRange(flights);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllFlightsWithIds_ReturnsTwoFlightsInOrder()
        {
            var context = CreateInMemoryContext(nameof(GetAllFlightsWithIds_ReturnsTwoFlightsInOrder));
            SeedFlights(context);
            var controller = CreateController(context);

            var result = await controller.GetAllFlightsWithIds();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value).ToList();
            Assert.Equal(2, items.Count);

            var first = items[0];
            var second = items[1];

            var idProp = first.GetType().GetProperty("FlightId");
            var urlProp = first.GetType().GetProperty("DetailUrl");
            Assert.NotNull(idProp);
            Assert.NotNull(urlProp);

            Assert.Equal(585150, (int)idProp.GetValue(first));
            Assert.Contains("flightId=585150", urlProp.GetValue(first).ToString());

            Assert.Equal(585155, (int)idProp.GetValue(second));
            Assert.Contains("flightId=585155", urlProp.GetValue(second).ToString());
        }

        [Fact]
        public async Task GetAllFlightsWithIds_EmptyDatabase_ReturnsNoFlightsMessage()
        {
            var context = CreateInMemoryContext(nameof(GetAllFlightsWithIds_EmptyDatabase_ReturnsNoFlightsMessage));
            var controller = CreateController(context);

            var result = await controller.GetAllFlightsWithIds();
            var ok = Assert.IsType<OkObjectResult>(result);
            var msg = ok.Value.GetType().GetProperty("Message")!.GetValue(ok.Value)!.ToString();
            Assert.Equal("No flights found in the system.", msg);
        }

        [Fact]
        public async Task GetByFlightId_ExistingFlightId_ReturnsOkObject()
        {
            var context = CreateInMemoryContext(nameof(GetByFlightId_ExistingFlightId_ReturnsOkObject));
            SeedFlights(context);
            var controller = CreateController(context);

            var result = await controller.GetByFlightId(585150);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var flight = Assert.IsType<FlightExportInfo>(okResult.Value);

            Assert.Equal(585150, flight.FlightId);
            Assert.Equal("Transavia", flight.AirlineFullname);
        }

        [Fact]
        public async Task GetByFlightId_NegativeId_ReturnsNotFound()
        {
            var context = CreateInMemoryContext(nameof(GetByFlightId_NegativeId_ReturnsNotFound));
            SeedFlights(context);
            var controller = CreateController(context);

            var result = await controller.GetByFlightId(-1);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("ID -1", notFound.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetByFlightId_NonExistentFlightId_ReturnsNotFound()
        {
            var context = CreateInMemoryContext(nameof(GetByFlightId_NonExistentFlightId_ReturnsNotFound));
            SeedFlights(context);
            var controller = CreateController(context);

            var result = await controller.GetByFlightId(999999);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Flight not found with ID 999999", notFound.Value.ToString());
        }

        [Fact]
        public async Task GetByFlightId_ZeroId_ReturnsNotFound()
        {
            var context = CreateInMemoryContext(nameof(GetByFlightId_ZeroId_ReturnsNotFound));
            SeedFlights(context);
            var controller = CreateController(context);

            var result = await controller.GetByFlightId(0);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("ID 0", notFound.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetByAirlineFullName_Whitespace_ReturnsBadRequest()
        {
            var context = CreateInMemoryContext(nameof(GetByAirlineFullName_Whitespace_ReturnsBadRequest));
            var controller = CreateController(context);

            Assert.IsType<BadRequestObjectResult>(await controller.GetByAirlineFullName(string.Empty));
            Assert.IsType<BadRequestObjectResult>(await controller.GetByAirlineFullName("   "));
        }

        [Fact]
        public async Task GetByAirlineFullName_NoMatchingFlights_ReturnsNotFound()
        {
            var context = CreateInMemoryContext(nameof(GetByAirlineFullName_NoMatchingFlights_ReturnsNotFound));
            SeedFlights(context);
            var controller = CreateController(context);

            var result = await controller.GetByAirlineFullName("NonExistentAirline");
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetByAirlineFullName_TrimsAndCaseInsensitive_ReturnsExpectedFlights()
        {
            var context = CreateInMemoryContext(nameof(GetByAirlineFullName_TrimsAndCaseInsensitive_ReturnsExpectedFlights));
            SeedFlights(context);
            var controller = CreateController(context);

            var result1 = await controller.GetByAirlineFullName("  Transavia  ");
            var ok1 = Assert.IsType<OkObjectResult>(result1);
            var list1 = Assert.IsAssignableFrom<IEnumerable<object>>(ok1.Value);
            Assert.Single(list1);

            var result2 = await controller.GetByAirlineFullName("TRANSAVIA");
            var ok2 = Assert.IsType<OkObjectResult>(result2);
            var list2 = Assert.IsAssignableFrom<IEnumerable<object>>(ok2.Value);
            Assert.Single(list2);
        }

        [Fact]
        public async Task GetByAirport_NullOrWhitespace_ReturnsBadRequest()
        {
            var context = CreateInMemoryContext(nameof(GetByAirport_NullOrWhitespace_ReturnsBadRequest));
            var controller = CreateController(context);

            Assert.IsType<BadRequestObjectResult>(await controller.GetByAirport(null));
            Assert.IsType<BadRequestObjectResult>(await controller.GetByAirport(string.Empty));
            Assert.IsType<BadRequestObjectResult>(await controller.GetByAirport("   "));
        }

        [Fact]
        public async Task GetByAirport_NoMatchingFlights_ReturnsNotFound()
        {
            var context = CreateInMemoryContext(nameof(GetByAirport_NoMatchingFlights_ReturnsNotFound));
            SeedFlights(context);
            var controller = CreateController(context);

            var result = await controller.GetByAirport("XYZ");
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetByAirport_TrimsAndCaseInsensitive_ReturnsExpectedFlights()
        {
            var context = CreateInMemoryContext(nameof(GetByAirport_TrimsAndCaseInsensitive_ReturnsExpectedFlights));
            SeedFlights(context);
            var controller = CreateController(context);

            var result1 = await controller.GetByAirport("  malaga ");
            var ok1 = Assert.IsType<OkObjectResult>(result1);
            var list1 = Assert.IsAssignableFrom<IEnumerable<object>>(ok1.Value);
            Assert.Single(list1);

            var result2 = await controller.GetByAirport("MALAGA");
            var ok2 = Assert.IsType<OkObjectResult>(result2);
            var list2 = Assert.IsAssignableFrom<IEnumerable<object>>(ok2.Value);
            Assert.Single(list2);
        }

        [Fact]
        public async Task GetFlightStatistics_ReturnsExpectedStatistics()
        {
            var context = CreateInMemoryContext(nameof(GetFlightStatistics_ReturnsExpectedStatistics));
            SeedFlights(context);
            var controller = CreateController(context);

            var result = await controller.GetFlightStatistics(null, null);
            var okResult = Assert.IsType<OkObjectResult>(result);

            var statsObj = okResult.Value!;
            var statsType = statsObj.GetType();

            DateTime? start = (DateTime?)statsType
                .GetProperty("StartDate")!
                .GetValue(statsObj);
            DateTime? end = (DateTime?)statsType
                .GetProperty("EndDate")!
                .GetValue(statsObj);
            Assert.Null(start);
            Assert.Null(end);

            int delayed = (int)statsType.GetProperty("DelayedFlights")!.GetValue(statsObj);
            int arriving = (int)statsType.GetProperty("ArrivingFlights")!.GetValue(statsObj);
            int diverted = (int)statsType.GetProperty("DivertedFlights")!.GetValue(statsObj);
            int nacht = (int)statsType.GetProperty("NachtvluchtFlights")!.GetValue(statsObj);
            Assert.Equal(1, delayed);
            Assert.Equal(1, arriving);
            Assert.Equal(0, diverted);
            Assert.Equal(0, nacht);

            var popularList = (IEnumerable<object>)statsType
                .GetProperty("MostPopularAirports")!
                .GetValue(statsObj)!;
            var popular = popularList.ToList();
            Assert.Equal(2, popular.Count);

            int totalSeats = (int)statsType.GetProperty("TotalSeats")!.GetValue(statsObj);
            int euFlights = (int)statsType.GetProperty("EuFlights")!.GetValue(statsObj);
            int nonEuFlights = (int)statsType.GetProperty("NonEuFlights")!.GetValue(statsObj);
            Assert.Equal(189 + 186, totalSeats);
            Assert.Equal(1, euFlights);
            Assert.Equal(1, nonEuFlights);

            var paxStats = statsType.GetProperty("PaxStatistics")!.GetValue(statsObj)!;
            var paxType = paxStats.GetType();
            int male = (int)paxType.GetProperty("Male")!.GetValue(paxStats);
            int female = (int)paxType.GetProperty("Female")!.GetValue(paxStats);
            int infant = (int)paxType.GetProperty("Infant")!.GetValue(paxStats);
            int child = (int)paxType.GetProperty("Child")!.GetValue(paxStats);
            int totalPax = (int)paxType.GetProperty("Total")!.GetValue(paxStats);
            Assert.Equal(62 + 96, male);
            Assert.Equal(68 + 67, female);
            Assert.Equal(2 + 3, infant);
            Assert.Equal(8 + 9, child);
            Assert.Equal(140 + 175, totalPax);

            var bagStats = statsType.GetProperty("BaggageStatistics")!.GetValue(statsObj)!;
            var bagType = bagStats.GetType();
            int totalWeight = (int)bagType.GetProperty("TotalWeight")!.GetValue(bagStats);
            int terminalWeight = (int)bagType.GetProperty("TerminalWeight")!.GetValue(bagStats);
            int transitWeight = (int)bagType.GetProperty("TransitWeight")!.GetValue(bagStats);
            Assert.Equal(66, totalWeight);
            Assert.Equal(66, terminalWeight);
            Assert.Equal(0, transitWeight);

            int totalFlights = (int)statsType.GetProperty("TotalFlights")!.GetValue(statsObj);
            Assert.Equal(2, totalFlights);
        }

        [Fact]
        public async Task GetFlightStatistics_InvalidStartDateFormat_ReturnsBadRequest()
        {
            var context = CreateInMemoryContext(nameof(GetFlightStatistics_InvalidStartDateFormat_ReturnsBadRequest));
            SeedFlights(context);
            var controller = CreateController(context);

            var result = await controller.GetFlightStatistics("invalid-date", null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Invalid Start Date format", badRequest.Value.ToString());
        }

        [Fact]
        public async Task GetFlightStatistics_InvalidEndDateFormat_ReturnsBadRequest()
        {
            var context = CreateInMemoryContext(nameof(GetFlightStatistics_InvalidEndDateFormat_ReturnsBadRequest));
            SeedFlights(context);
            var controller = CreateController(context);

            var validStart = new DateTime(2024, 01, 01, 00, 00, 00).ToString("o");
            var result = await controller.GetFlightStatistics(validStart, "not-a-date");
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Invalid End Date format", badRequest.Value.ToString());
        }

        [Fact]
        public async Task GetFlightStatistics_StartAfterEnd_ReturnsBadRequest()
        {
            var context = CreateInMemoryContext(nameof(GetFlightStatistics_StartAfterEnd_ReturnsBadRequest));
            SeedFlights(context);
            var controller = CreateController(context);

            var start = new DateTime(2024, 02, 02, 00, 00, 00).ToString("o");
            var end = new DateTime(2024, 01, 01, 00, 00, 00).ToString("o");
            var result = await controller.GetFlightStatistics(start, end);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Start Date must be before End Date", badRequest.Value.ToString());
        }

    }
}
