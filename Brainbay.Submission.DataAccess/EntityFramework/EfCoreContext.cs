using Brainbay.Submission.DataAccess.Models.Domain;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class EfCoreContext : DbContext
    {
        public EfCoreContext(                             
            DbContextOptions<EfCoreContext> options)      
            : base(options) {}

        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterLocation> CharacterLocations { get; set; }
        public DbSet<CharacterOrigin> CharacterOrigins { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CharacterConfig());  
        }
    }
}

