using AutoMapper;
using Brainbay.Submission.DataAccess.Models.Domain;
using Brainbay.Submission.DataAccess.Models.Enums;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Net.Api.Mapper;
using RickAndMorty.Net.Api.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Brainbay.Submission.ApiScraper
{
    class Program
    {
        private const int MaxParallel = 16;
        private static readonly ConcurrentBag<IEnumerable<Character>> concurrentBag = new ConcurrentBag<IEnumerable<Character>>();
        private static readonly Uri uri = new Uri("https://rickandmortyapi.com/");
        private static readonly Stopwatch stopwatch = new Stopwatch();

        private static ServiceProvider serviceProvider;
        private static DbContextOptions<RickAndMortyContext> dbOptions;

        static async Task Main(string[] args)
        {
            /* Use Dependency Injection pattern */
            ConfigureServices();

            await CleanupDbAsync();

            stopwatch.Start();

            var service = serviceProvider.GetRequiredService<IRickAndMortyService>();

            /* Optimize maximum concurrent network requests */
            SetMaxConnection();

            /* Fetch & save the first page */
            var firstPage = await service.FilterCharacterSinglePage(
                                                                    page: 1,
                                                                    characterStatus: CharacterStatus.Alive);
            SaveFirstPage(firstPage);

            /* Fetch and save remaining data using TPL Dataflow */
            await FetchWithTPL(firstPage);
            SaveItemsInConcurrentBag();

            //await FetchAndSaveWithTPL(firstPage);

            Console.WriteLine("All finished.");
        }


        private static void SaveFirstPage(PagedCharacters firstPage)
        {
            using var db = new RickAndMortyContext(dbOptions);
            db.Characters.AddRange(firstPage.Characters);
            db.SaveChanges();
        }

        private static void SaveItemsInConcurrentBag()
        {
            // Bulk insert into the db
            var allCharacters = concurrentBag.SelectMany(n => n).ToArray();
            using var db = new RickAndMortyContext(dbOptions);
            db.Characters.AddRange(allCharacters);
            db.SaveChanges();
            Console.WriteLine($"Data save finished. Elapsed: {stopwatch.Elapsed}");
        }

        private static async Task FetchWithTPL(PagedCharacters firstPage)
        {
            /* Performance Considerations
               1. Because API fetch is network bound parallel execution is beneficial.
                  Therefore, use TPL dataflow for fetching and collecting data.
               2. SQLite is file-based simple db and it locks the whole db when writing.
                  Therefore, there is no gain at parallel writing.
                  So, we write all the data at once.              
             */

            var apiFetcher = new ActionBlock<int>(
                async pageNr =>
                {
                    var result = await FetchCharacter(pageNr);
                    concurrentBag.Add(result.Characters);
                }
                , new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = MaxParallel,
                    BoundedCapacity = MaxParallel,
                    SingleProducerConstrained = false
                });


            // Start by posting page numbers to the fetcher
            for (var pageNr = 2; pageNr <= firstPage.PageInfo.Pages; pageNr++)
            {
                await apiFetcher.SendAsync(pageNr);
            }

            apiFetcher.Complete();
            await apiFetcher.Completion;
            Console.WriteLine($"API fetch completed. Elapsed: {stopwatch.Elapsed}");
        }

        private static async Task FetchAndSaveWithTPL(PagedCharacters firstPage)
        {
            /* Performance Considerations
               1. Because API fetch is network bound parallel execution is beneficial.
                  Therefore, use TPL dataflow for fetching and collecting data.
               2. SQLite is file-based simple db and it locks the whole db when writing.
                  Therefore, there is no gain at parallel writing.
                  So, we write all the data at once.              
             */

            var apiFetcher = new TransformBlock<int, PagedCharacters>(
                async pageNr =>
                {
                    var result = await FetchCharacter(pageNr);
                    return result;
                }
                , new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = MaxParallel,
                    SingleProducerConstrained = false
                });

            var saver = new ActionBlock<PagedCharacters>(pc =>
            {
                using var db = new RickAndMortyContext(dbOptions);
                db.Characters.AddRange(pc.Characters);
                db.SaveChanges();
                Console.WriteLine($"Save finished. Page: {pc.CurrentPage}");
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1 //Do not allow concurrent writes
            });

            apiFetcher.LinkTo(saver, new DataflowLinkOptions { PropagateCompletion = true });

            // Start by posting page numbers to the fetcher
            for (var pageNr = 2; pageNr <= firstPage.PageInfo.Pages; pageNr++)
            {
                await apiFetcher.SendAsync(pageNr);
            }

            apiFetcher.Complete();
            await saver.Completion;
            Console.WriteLine($"API fetch & save completed. Elapsed: {stopwatch.Elapsed}");
        }

        private static async Task<PagedCharacters> FetchCharacter(int pageNr)
        {
            var service = serviceProvider.GetRequiredService<IRickAndMortyService>();
            var result = await service.FilterCharacterSinglePage(page: pageNr,
                                                               characterStatus: CharacterStatus.Alive);
            Console.WriteLine($"API fetch finished. Page: {pageNr}");
            return result;
        }

        private static async Task CleanupDbAsync()
        {
            using var db = new RickAndMortyContext(dbOptions);
            var existing = await db.Characters.CountAsync();
            Console.WriteLine($"Existing items {existing}");
            db.Database.EnsureDeleted();
            Console.WriteLine("DB deleted.");
            db.Database.EnsureCreated();
            Console.WriteLine("DB re-created.");
        }

        private static void ConfigureServices()
        {
            // Use Sqlite file database 
            dbOptions = new DbContextOptionsBuilder<RickAndMortyContext>()
                 .UseSqlite("Filename=RickMorty.db").Options;

            // Use Dependency Injection and configure the services
            var services = new ServiceCollection();
            services.AddTransient<IRickAndMortyService, RickAndMortyService>()
                    .AddSingleton(dbOptions)
                    .AddSingleton(new HttpClient() { BaseAddress = uri })
                    .AddAutoMapper(c => c.AddProfile<RickAndMortyMapperProfile>());


            // Register internal DefaultHttpClientFactory
            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#how-to-use-typed-clients-with-ihttpclientfactory
            //services.AddHttpClient<IRickAndMortyService, RickAndMortyService>(
            //                          c =>
            //                          {
            //                              c.BaseAddress = uri;
            //                          });
            serviceProvider = services.BuildServiceProvider();


            Console.WriteLine("Configuration completed.");
        }

        private static void SetMaxConnection()
        {
            ServicePointManager.DefaultConnectionLimit = MaxParallel;
        }

    }

}

