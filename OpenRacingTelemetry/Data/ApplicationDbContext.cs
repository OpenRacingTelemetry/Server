using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using OpenRacingTelemetry.Models;

namespace OpenRacingTelemetry.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<OrganizersTeam> OrganizersTeams { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Race> Race { get; set; }
        public DbSet<Record> Records { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Car>().ToTable("Cars");         
            builder.Entity<OrganizersTeam>().ToTable("OrganizersTeams");
            builder.Entity<Place>().ToTable("Places");
            builder.Entity<Race>().ToTable("Races");
            builder.Entity<Record>().ToTable("Records");

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
