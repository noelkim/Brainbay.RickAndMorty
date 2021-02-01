using Brainbay.Submission.DataAccess.Configurations;
using Brainbay.Submission.DataAccess.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Brainbay.Submission.DataAccess
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

