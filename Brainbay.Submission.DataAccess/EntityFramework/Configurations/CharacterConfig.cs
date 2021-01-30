using Brainbay.Submission.DataAccess.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace DataLayer.EfCode.Configurations
{
    public class CharacterConfig : IEntityTypeConfiguration<Character>
    {
        public void Configure
            (EntityTypeBuilder<Character> entity)
        {
            entity.Property(p => p.Created).HasColumnType("date");

            // Save URLs in string format 
            entity.Property(x => x.Image)
                .HasConversion(v => v.AbsoluteUri, v => new Uri(v));
            entity.Property(x => x.Url)
                .HasConversion(v => v.AbsoluteUri, v => new Uri(v));
            entity.Property(x => x.LocationUrl)
                .HasConversion(v => v.AbsoluteUri, v => new Uri(v));
            entity.Property(x => x.OriginUrl)
                .HasConversion(v => v.AbsoluteUri, v => new Uri(v));

            //
            entity.Property(c => c.Episode).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.Name);

        }
    }
}