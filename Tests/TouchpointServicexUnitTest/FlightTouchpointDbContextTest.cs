using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service2.Data;

namespace TouchpointServicexUnitTest
{
    public class FlightTouchpointDbContextTest : FlightTouchpointDbContext
    {
        public FlightTouchpointDbContextTest(DbContextOptions<FlightTouchpointDbContext> options)
           : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<FlightTouchpointInfo>()
                .HasKey(f => new { f.FlightId, f.TouchpointTime});
        }
    }
}
