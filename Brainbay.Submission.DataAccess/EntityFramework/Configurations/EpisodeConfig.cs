using Brainbay.Submission.DataAccess.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DataLayer.EfCode.Configurations
{
    public class EpisodeConfig : IEntityTypeConfiguration<Episode>
    {
        public void Configure
            (EntityTypeBuilder<Episode> entity)
        {
            entity.Property(p => p.Created).HasColumnType("date");
            entity.Property(p => p.AirDate).HasColumnType("date");

            entity.Property(x => x.Url).IsUnicode(false);
            entity.Property(c => c.Characters).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}