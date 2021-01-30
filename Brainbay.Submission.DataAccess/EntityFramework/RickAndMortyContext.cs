using Brainbay.Submission.DataAccess.Models.Domain;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class RickAndMortyContext : DbContext
    {
        public RickAndMortyContext(
            DbContextOptions<RickAndMortyContext> options)
            : base(options) { }

        public DbSet<Character> Characters { get; set; }

        public DbSet<Episode> Episodes { get; set; }

        public DbSet<Location> Locations { get; set; }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CharacterConfig())
                .ApplyConfiguration(new EpisodeConfig())
                .ApplyConfiguration(new LocationConfig());
        }
    }
}

