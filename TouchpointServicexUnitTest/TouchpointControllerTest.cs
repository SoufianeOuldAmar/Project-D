using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Service2.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using TouchpointsService.Controllers.V1;

namespace TouchpointServicexUnitTest
{

    public class FakeUrlHelper : IUrlHelper
    {
        public ActionContext ActionContext { get; set; }

        public string Action(UrlActionContext context)
        {
            // Pull out the "id" route value
            var values = new RouteValueDictionary(context.Values);
            if (values.TryGetValue("id", out var id))
            {
                return $"/api/v1/flight-touchpoints/by-touchpoint-id/{id}";
            }
            return "/dummy";
        }

        // Unused members
        public string Content(string contentPath) => throw new NotImplementedException();
        public bool IsLocalUrl(string url) => throw new NotImplementedException();
        public string Link(string routeName, object values) => throw new NotImplementedException();
        public string RouteUrl(UrlRouteContext routeContext) => throw new NotImplementedException();
    }
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

        private FlightTouchpointsController CreateController(FlightTouchpointDbContextTest context)
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new FlightTouchpointsController(context, cache);
            controller.Url = new FakeUrlHelper();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost");
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            return controller;
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

        [Fact]
        public async Task GetYearlyStats_WithYearBefore2024_ReturnsBadRequest()
        {
            var db = CreateInMemoryContext(nameof(GetYearlyStats_WithYearBefore2024_ReturnsBadRequest));
            SeedFlights(db);
            var controller = CreateController(db);

            var result = await controller.GetYearlyStats(2023);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var message = badRequest.Value.ToString();
            Assert.Contains("Data is only available from 2024 onward", message);
        }

        [Fact]
        public async Task GetYearlyStats_WithNoTouchpoints_ReturnsOkWithNoDataMessage()
        {
            var db = CreateInMemoryContext(nameof(GetYearlyStats_WithNoTouchpoints_ReturnsOkWithNoDataMessage));
            var controller = CreateController(db);

            var result = await controller.GetYearlyStats(2025);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = okResult.Value!;
            var dataType = data.GetType();

            Assert.Equal(2025, (int)dataType.GetProperty("Year")!.GetValue(data)!);
            var noDataMsg = dataType.GetProperty("Message")!.GetValue(data)!.ToString();
            Assert.Contains("No data found for this year.", noDataMsg);
        }

        [Fact]
        public async Task GetYearlyStats_WithTouchpointsIn2024_ReturnsExpectedStatistics()
        {
            var db = CreateInMemoryContext(nameof(GetYearlyStats_WithTouchpointsIn2024_ReturnsExpectedStatistics));
            SeedFlights(db);
            var controller = CreateController(db);

            var result = await controller.GetYearlyStats(2024);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = okResult.Value!;
            var type = stats.GetType();

            Assert.Equal(2024, (int)type.GetProperty("Year")!.GetValue(stats)!);
            Assert.Equal(3, (int)type.GetProperty("FlightCount")!.GetValue(stats)!);

            var totalPassengers = (double)type.GetProperty("TotalPassengers")!.GetValue(stats)!;
            Assert.InRange(totalPassengers, 355.4, 355.400002);

            Assert.Equal(10.0, (double)type.GetProperty("AverageDelay")!.GetValue(stats)!);

            var airports = (IEnumerable<object>)type.GetProperty("TopAirports")!.GetValue(stats)!;
            var list = airports.ToList();

            Assert.Equal(3, list.Count);

            var first = list[0];
            var ft = first.GetType();
            Assert.Equal("Istanbul", ft.GetProperty("Airport")!.GetValue(first)!.ToString());
            Assert.Equal(2, (int)ft.GetProperty("Count")!.GetValue(first)!);

            var second = list[1];
            var st = second.GetType();
            Assert.Equal("Al-Hoceima", st.GetProperty("Airport")!.GetValue(second)!.ToString());
            Assert.Equal(1, (int)st.GetProperty("Count")!.GetValue(second)!);

            var third = list[2];
            var tt = third.GetType();
            Assert.Equal("Oujda", tt.GetProperty("Airport")!.GetValue(third)!.ToString());
            Assert.Equal(1, (int)tt.GetProperty("Count")!.GetValue(third)!);
        }

        [Fact]
        public async Task GetMonthlyStats_WithYearBefore2024_ReturnsBadRequest()
        {
            var db = CreateInMemoryContext(nameof(GetMonthlyStats_WithYearBefore2024_ReturnsBadRequest));
            SeedFlights(db);

            var controller = CreateController(db);
            var result = await controller.GetMonthlyStats(2023, 1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var msg = badRequest.Value.ToString();
            Assert.Contains("Data is only available from 2024 onward", msg);
        }

        [Fact]
        public async Task GetMonthlyStats_WithInvalidMonth_ReturnsBadRequest()
        {
            var db = CreateInMemoryContext(nameof(GetMonthlyStats_WithInvalidMonth_ReturnsBadRequest));
            SeedFlights(db);
            var controller = CreateController(db);

            var badRequest0 = Assert.IsType<BadRequestObjectResult>(await controller.GetMonthlyStats(2024, 0));
            Assert.Contains("Choose between 1 and 12", badRequest0.Value.ToString());

            var badRequest13 = Assert.IsType<BadRequestObjectResult>(await controller.GetMonthlyStats(2024, 13));
            Assert.Contains("Choose between 1 and 12", badRequest13.Value.ToString());
        }

        [Fact]
        public async Task GetMonthlyStats_WithNoData_ReturnsOkWithDefaults()
        {
            var db = CreateInMemoryContext(nameof(GetMonthlyStats_WithNoData_ReturnsOkWithDefaults));

            var controller = CreateController(db);

            var result = await controller.GetMonthlyStats(2024, 2);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = okResult.Value!;
            var t = data.GetType();

            Assert.Equal(2024, (int)t.GetProperty("Year")!.GetValue(data)!);
            Assert.Equal("February", (string)t.GetProperty("Month")!.GetValue(data)!);
            Assert.Equal(0, (int?)t.GetProperty("FlightCount")!.GetValue(data)!);
            Assert.Null(t.GetProperty("TotalPassengers")!.GetValue(data));
            Assert.Equal(0, (int)t.GetProperty("BusiestDay")!.GetValue(data)!);
            var types = (IEnumerable<string>)t.GetProperty("TopAircraftTypes")!.GetValue(data)!;
            Assert.Empty(types);
            Assert.Contains("No data found.", t.GetProperty("Message")!.GetValue(data)!.ToString());
        }

        [Fact]
        public async Task GetMonthlyStats_WithTouchpointsInJanuary2024_ReturnsExpectedStatistics()
        {
            var db = CreateInMemoryContext(nameof(GetMonthlyStats_WithTouchpointsInJanuary2024_ReturnsExpectedStatistics));
            SeedFlights(db);
            var controller = CreateController(db);

            var result = await controller.GetMonthlyStats(2024, 1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = okResult.Value!;
            var t = stats.GetType();

            Assert.Equal(2024, (int)t.GetProperty("Year")!.GetValue(stats)!);
            Assert.Equal("January", (string)t.GetProperty("Month")!.GetValue(stats)!);
            Assert.Equal(3, (int)t.GetProperty("FlightCount")!.GetValue(stats)!);
            Assert.Equal(355, (int?)t.GetProperty("TotalPassengers")!.GetValue(stats)!);
            Assert.Equal(1, (int)t.GetProperty("BusiestDay")!.GetValue(stats)!);

            var types = (IEnumerable<string>)t.GetProperty("TopAircraftTypes")!.GetValue(stats)!;
            var list = types.ToList();
            Assert.Equal(3, list.Count);
            Assert.Equal("A320N", list[0]);
            Assert.Equal("B737W8", list[1]);
            Assert.Equal("B737S8", list[2]);

            Assert.Null(t.GetProperty("Message")!.GetValue(stats));
        }

        [Fact]
        public async Task GetBusyHoursPerDay_WithInvalidTimeWindow_ReturnsBadRequest()
        {
            var db = CreateInMemoryContext(nameof(GetBusyHoursPerDay_WithInvalidTimeWindow_ReturnsBadRequest));
            SeedFlights(db);
            var controller = CreateController(db);

            var result0 = await controller.GetBusyHoursPerDay(0);
            var badRequest0 = Assert.IsType<BadRequestObjectResult>(result0);
            Assert.Contains("must be positive", badRequest0.Value.ToString());

            var resultNegative = await controller.GetBusyHoursPerDay(-15);
            var badReqNeg = Assert.IsType<BadRequestObjectResult>(resultNegative);
            Assert.Contains("must be positive", badReqNeg.Value.ToString());
        }

        [Fact]
        public async Task GetBusyHoursPerDay_WithDefaultTimeWindow_ReturnsExpectedBusiestPeriod()
        {
            var db = CreateInMemoryContext(nameof(GetBusyHoursPerDay_WithDefaultTimeWindow_ReturnsExpectedBusiestPeriod));
            SeedFlights(db);
            var controller = CreateController(db);

            var result = await controller.GetBusyHoursPerDay();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            var items = list.ToList();

            Assert.Single(items);

            var entry = items[0];
            var type = entry.GetType();

            Assert.Equal("Monday", type.GetProperty("Day")!.GetValue(entry)!.ToString());
            Assert.Equal("14:00 - 15:00", type.GetProperty("BusiestPeriod")!.GetValue(entry)!.ToString());
            Assert.Equal(2, (int)type.GetProperty("TouchpointCount")!.GetValue(entry)!);
        }

        [Fact]
        public async Task GetAllTouchpointsWithIds_WithNoData_ReturnsEmptyList()
        {
            var db = CreateInMemoryContext(nameof(GetAllTouchpointsWithIds_WithNoData_ReturnsEmptyList));
            var controller = CreateController(db);

            var actionResult = await controller.GetAllTouchpointsWithIds();

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetAllTouchpointsWithIds_WithData_ReturnsLinkObjects()
        {
            var db = CreateInMemoryContext(nameof(GetAllTouchpointsWithIds_WithData_ReturnsLinkObjects));
            SeedFlights(db);
            var controller = CreateController(db);

            var actionResult = await controller.GetAllTouchpointsWithIds();

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            var items = list.ToList();

            Assert.Equal(4, items.Count);

            var first = items[0];
            var type = first.GetType();
            var idValue = type.GetProperty("id")!.GetValue(first)!.ToString();
            var tpValue = type.GetProperty("Touchpoint")!.GetValue(first)!.ToString();
            var urlValue = type.GetProperty("Url")!.GetValue(first)!.ToString();

            Assert.False(string.IsNullOrWhiteSpace(idValue));
            Assert.Equal("Aankomsthal", tpValue);
            Assert.Contains("/by-touchpoint-id/", urlValue);
        }

        [Fact]
        public async Task GetByTouchpointId_WithEmptyId_ReturnsBadRequest()
        {
            var db = CreateInMemoryContext(nameof(GetByTouchpointId_WithEmptyId_ReturnsBadRequest));
            SeedFlights(db);
            var controller = CreateController(db);

            var actionResult = await controller.GetByTouchpointId(string.Empty);

            var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var msg = badRequest.Value.ToString();
            Assert.Contains("ID is required", msg);
        }

        [Fact]
        public async Task GetByTouchpointId_WithUnknownId_ReturnsNotFound()
        {
            var db = CreateInMemoryContext(nameof(GetByTouchpointId_WithUnknownId_ReturnsNotFound));
            SeedFlights(db);
            var controller = CreateController(db);

            var actionResult = await controller.GetByTouchpointId("nonexistenthash");

            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetByTouchpointId_WithValidId_ReturnsTouchpointInfo()
        {
            var db = CreateInMemoryContext(nameof(GetByTouchpointId_WithValidId_ReturnsTouchpointInfo));
            SeedFlights(db);
            var controller = CreateController(db);

            var linksResult = await controller.GetAllTouchpointsWithIds();
            var okLinks = Assert.IsType<OkObjectResult>(linksResult.Result);
            var links = Assert.IsAssignableFrom<IEnumerable<object>>(okLinks.Value);
            var firstLink = links.First();
            var validId = firstLink.GetType()
                                  .GetProperty("id")!
                                  .GetValue(firstLink)!
                                  .ToString()!;

            var detailResult = await controller.GetByTouchpointId(validId);

            var okDetail = Assert.IsType<OkObjectResult>(detailResult.Result);
            var info = Assert.IsType<FlightTouchpointInfo>(okDetail.Value);
            Assert.Equal(585146, info.FlightId);
            Assert.Equal("Aankomsthal", info.Touchpoint);
        }

        [Fact]
        public async Task GetMostCommonTrafficType_WithEmptyDb_ReturnsNotFound()
        {
            var db = CreateInMemoryContext(nameof(GetMostCommonTrafficType_WithEmptyDb_ReturnsNotFound));
            var controller = CreateController(db);

            var result = await controller.GetMostCommonTrafficType();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetMostCommonTrafficType_WithData_ReturnsStats()
        {
            var db = CreateInMemoryContext(nameof(GetMostCommonTrafficType_WithData_ReturnsStats));
            SeedFlights(db);
            var controller = CreateController(db);

            var result = await controller.GetMostCommonTrafficType();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var payload = okResult.Value!;
            var type = payload.GetType();

            var mostCommon = type.GetProperty("MostCommonType")!.GetValue(payload)!;
            var mcType = mostCommon.GetType();
            Assert.Equal("A", mcType.GetProperty("TrafficType")!.GetValue(mostCommon)!.ToString());
            Assert.Equal(4, (int)mcType.GetProperty("Count")!.GetValue(mostCommon)!);
            Assert.Equal(100.0, (double)mcType.GetProperty("Percentage")!.GetValue(mostCommon)!);

            var allTypes = (IEnumerable<object>)type.GetProperty("AllTypes")!.GetValue(payload)!;
            Assert.Contains(allTypes, x =>
                x.GetType().GetProperty("TrafficType")!.GetValue(x)!.ToString() == "A" && (int)x.GetType().GetProperty("Count")!.GetValue(x)! == 4);
        }

        [Fact]
        public async Task GetFlightsPerAirport_WithEmptyDb_ReturnsEmptyList()
        {
            var db = CreateInMemoryContext(nameof(GetFlightsPerAirport_WithEmptyDb_ReturnsEmptyList));
            var controller = CreateController(db);

            var result = await controller.GetFlightsPerAirport();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetFlightsPerAirport_WithData_ReturnsStatsOrdered()
        {
            var db = CreateInMemoryContext(nameof(GetFlightsPerAirport_WithData_ReturnsStatsOrdered));
            SeedFlights(db);
            var controller = CreateController(db);

            var result = await controller.GetFlightsPerAirport();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            var stats = list.ToList();

            Assert.Contains(stats, item =>
            {
                var t = item.GetType();
                return t.GetProperty("Airport")!.GetValue(item)!.ToString() == "Istanbul"
                    && (int)t.GetProperty("FlightCount")!.GetValue(item)! == 1;
            });
            Assert.Contains(stats, item =>
            {
                var t = item.GetType();
                return t.GetProperty("Airport")!.GetValue(item)!.ToString() == "Al-Hoceima"
                    && (int)t.GetProperty("FlightCount")!.GetValue(item)! == 1;
            });
            Assert.Contains(stats, item =>
            {
                var t = item.GetType();
                return t.GetProperty("Airport")!.GetValue(item)!.ToString() == "Oujda"
                    && (int)t.GetProperty("FlightCount")!.GetValue(item)! == 1;
            });
        }

        [Fact]
        public async Task GetCountriesWithMostFlights_ParameterOutOfRange_ReturnsBadRequest()
        {
            var db = CreateInMemoryContext(nameof(GetCountriesWithMostFlights_ParameterOutOfRange_ReturnsBadRequest));
            SeedFlights(db);
            var controller = CreateController(db);

            var result0 = await controller.GetCountriesWithMostFlights(0);
            var result16 = await controller.GetCountriesWithMostFlights(16);

            var badRequest0 = Assert.IsType<BadRequestObjectResult>(result0);
            Assert.Contains("Top must be at least 1", badRequest0.Value.ToString());

            var badRequest16 = Assert.IsType<BadRequestObjectResult>(result16);
            Assert.Contains("not larger than 15", badRequest16.Value.ToString());
        }

        [Fact]
        public async Task GetCountriesWithMostFlights_WithData_DefaultTop_ReturnsCorrectOrder()
        {
            var db = CreateInMemoryContext(nameof(GetCountriesWithMostFlights_WithData_DefaultTop_ReturnsCorrectOrder));
            SeedFlights(db);
            var controller = CreateController(db);

            var result = await controller.GetCountriesWithMostFlights();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            var stats = list.ToList();

            var first = stats[0];
            var ft = first.GetType();
            Assert.Equal("Marokko", ft.GetProperty("Country")!.GetValue(first)!.ToString());
            Assert.Equal(2, (int)ft.GetProperty("FlightCount")!.GetValue(first)!);

            var second = stats[1];
            var sd = second.GetType();
            Assert.Equal("Turkey", sd.GetProperty("Country")!.GetValue(second)!.ToString());
            Assert.Equal(1, (int)sd.GetProperty("FlightCount")!.GetValue(second)!);
        }
    }
}
