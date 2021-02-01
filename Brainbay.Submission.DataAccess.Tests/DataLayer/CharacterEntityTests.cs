using Brainbay.Submission.DataAccess.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using System;
using Microsoft.Data.Sqlite;

namespace Brainbay.Submission.DataAccess.Tests.DataLayer
{
    public class CharacterEntityTests : EntityTestsBase
    {
        public CharacterEntityTests()
        {
        }


        [Fact]
        public void Should_Create_New_Character()
        {
            // Arrange
            using (var db = new RickAndMortyContext(dbOptions))
            {
                var existing = db.Characters.Find(1);
                existing.Should().BeNull();

                // Act
                var character = new Character(id: 1, name: "Mozart");
                db.Characters.Add(character);
                db.SaveChanges();
            }

            // Assert
            using (var db = new RickAndMortyContext(dbOptions))
            {
                var found = db.Characters.Find(1);
                found.Should().NotBeNull();
            }
        }


        [Fact]
        public void Should_Fail_New_Character_WithEmptyName()
        {
            // Arrange
            using (var db = new RickAndMortyContext(dbOptions))
            {
                // Act & Assert
                var character = new Character(id: 2, name: null);
                db.Characters.Add(character);
                var dbException = Assert.Throws<DbUpdateException>(() => db.SaveChanges());
                var sqlException = dbException.InnerException as SqliteException;
                sqlException.SqliteErrorCode.Should().Be(19, "Inserting a null Name violates Not Null constraint.");
                //SQLite Error 19: 'NOT NULL constraint failed: Characters.Name'.
            }
        }


        [Fact]
        public void Should_Create_New_Character_WithUrl()
        {
            // Arrange
            using (var db = new RickAndMortyContext(dbOptions))
            {
                var existing = db.Characters.Find(3);
                existing.Should().BeNull();

                // Act
                var character = new Character(id: 3, name: "BlackPink",url: new Uri("http://www.google.com"));
                db.Characters.Add(character);
                db.SaveChanges();
            }

            // Assert
            using (var db = new RickAndMortyContext(dbOptions))
            {
                var found = db.Characters.Find(3);
                found.Should().NotBeNull();
            }
        }
         
    }
}

