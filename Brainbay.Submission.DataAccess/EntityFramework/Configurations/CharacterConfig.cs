using Brainbay.Submission.DataAccess.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    public class CharacterConfig : IEntityTypeConfiguration<Character>
    {
        public void Configure
            (EntityTypeBuilder<Character> entity)
        {
            entity.Property(p => p.Created).HasColumnType("date");        

            entity.Property(x => x.Image).IsUnicode(false);

            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.Name);

        }
    }
}