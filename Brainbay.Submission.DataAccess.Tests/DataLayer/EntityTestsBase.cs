using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace Brainbay.Submission.DataAccess.Tests.DataLayer
{
    public abstract class EntityTestsBase : IDisposable
    {
        // Using Sqlite in-memory database needs to keep the DbConnection for the lifetime of one set of tests.
        // https://docs.microsoft.com/en-us/ef/core/testing/sqlite
        protected readonly DbConnection dbConnection;
        protected readonly DbContextOptions<RickAndMortyContext> dbOptions;


        protected EntityTestsBase()
        {
            // Use Sqlite in-memory database 
            dbOptions = new DbContextOptionsBuilder<RickAndMortyContext>()
                .UseSqlite(CreateInMemoryDatabase()).Options;
            
            using var db = new RickAndMortyContext(dbOptions);
            db.Database.EnsureCreated();

        }



        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }
        public void Dispose() => dbConnection?.Dispose();
    }
}