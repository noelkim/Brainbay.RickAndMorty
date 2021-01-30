using Brainbay.Submission.DataAccess.Models.Domain;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Brainbay.Submission.DataAccess.Models.Dto;
using AutoMapper;
using RickAndMorty.Net.Api.Mapper;

namespace Brainbay.Submission.DataAccess.Tests.DataLayer
{
    public class CharacterDomainTests : EntityTestsBase
    {
        private readonly IMapper mapper;

        public CharacterDomainTests()
        {
            this.mapper = new MapperConfiguration(c => c.AddProfile<RickAndMortyMapperProfile>())
                    .CreateMapper();
        }


        [Fact]
        public void Should_Create_New_Character_UsingDto()
        {
            // Arrange
            using (var db = new RickAndMortyContext(dbOptions))
            {
                var existing = db.Characters.Find(1);
                existing.Should().BeNull();

                // Act
                var character = new CharacterDto
                {
                    Id = 1,
                    Name = "Steve",
                    Location = new CharacterLocationDto { Name = "Earth" },
                    Origin = new CharacterOriginDto { Name = "Moon" },
                    Episode = Array.Empty<string>()
                };
                var entity = mapper.Map<Character>(character);
                db.Characters.Add(entity);
                db.SaveChanges();
            }

            // Assert
            using (var db = new RickAndMortyContext(dbOptions))
            {
                var found = db.Characters.Find(1);
                found.Should().NotBeNull();
                found.Name.Should().Be("Steve");
            }
        }



    }
}

