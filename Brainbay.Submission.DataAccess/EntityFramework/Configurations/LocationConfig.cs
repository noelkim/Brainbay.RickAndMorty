using Brainbay.Submission.DataAccess.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DataLayer.EfCode.Configurations
{
    public class LocationConfig : IEntityTypeConfiguration<Location>
    {
        public void Configure
            (EntityTypeBuilder<Location> entity)
        {
            entity.Property(p => p.Created).HasColumnType("date"); 

            entity.Property(x => x.Url).IsUnicode(false);

            entity.Property(c => c.Residents).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}