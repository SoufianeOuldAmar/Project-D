using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_D.Data;

namespace ExportServicexUnitTest
{
    public class TestFlightExportDbContext : FlightExportDbContext
    {
        public TestFlightExportDbContext(DbContextOptions<FlightExportDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<FlightExportInfo>()
                .HasKey(f => f.FlightId);
        }
    }
}
