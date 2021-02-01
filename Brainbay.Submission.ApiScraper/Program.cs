using AutoMapper;
using Brainbay.Submission.DataAccess;
using Brainbay.Submission.DataAccess.Models.Domain;
using Brainbay.Submission.DataAccess.Models.Enums;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Net.Api.Mapper;
using RickAndMorty.Net.Api.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Brainbay.Submission.ApiScraper
{
    class Program
    {
        private const int MaxParallel = 4;
        private static readonly ConcurrentBag<IEnumerable<Character>> concurrentBag = new ConcurrentBag<IEnumerable<Character>>();
        private static readonly Uri uri = new Uri("https://rickandmortyapi.com/");
        private static readonly Stopwatch stopwatch = new Stopwatch();

        private static ServiceProvider serviceProvider;
        private static DbContextOptions<RickAndMortyContext> dbOptions;

        static async Task Main(string[] args)
        {
            Console.Write(@"Rick and Morty API Scraper is started. 
By pressing any key, below actions will be taken:
    1. All the data in the database will be deleted.
    2. Rick and Morty Characters with Alive status will be fetched from the API endpoint.
    3. Fetched data will be saved to the database.
Press any key to continue. Or press 'q' or 'esc' to quit.
");
            var pressed = Console.ReadKey();
            if(pressed.Key == ConsoleKey.Q ||
                pressed.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Cancelled.");
                return;
            }

            /* Use Dependency Injection & Configure services. */
            ConfigureServices();

            /* Ensure db is created and made empty. */
            await CleanupDbAsync();

            stopwatch.Start();

            /* Fetch & save the first page. */
            var service = serviceProvider.GetRequiredService<IRickAndMortyService>();
            var firstPage = await service.GetCharacterSinglePage(
                                          page: 1,
                                          characterStatus: CharacterStatus.Alive);
            SavePage(firstPage);

            /* Fetch and save remaining data using TPL Dataflow */
            await FetchWithTPL(firstPage);
            SaveItemsInConcurrentBag();

            //await FetchAndSaveWithTPL(firstPage);

            Console.WriteLine("All finished.");
        }


        private static void SavePage(PagedCharacters firstPage)
        {
            using var db = new RickAndMortyContext(dbOptions);
            db.BulkInsert(firstPage.Characters.ToArray());
            db.SaveChanges();
        }

        private static void SaveItemsInConcurrentBag()
        {
            /* 
                  SQLite is file-based simple db and it locks the whole db when writing.
                  Therefore, there is no gain at parallel writing.
                  So, we write all the data at once.    
            */

            var allCharacters = concurrentBag.SelectMany(n => n).ToArray();
            using var db = new RickAndMortyContext(dbOptions);
            // Use BulkInsert extension 
            /* With SQLite, BulkInsert has no preformance gain since it does not support BulkCopy. */
            db.BulkInsert(allCharacters);
            db.SaveChanges();
            Console.WriteLine($"Data save finished. Elapsed: {stopwatch.Elapsed}");
        }

        private static async Task FetchWithTPL(PagedCharacters firstPage)
        {
            /* Because API fetch is network bound parallel execution is beneficial.
                  Therefore, use TPL dataflow for fetching and collecting data.          
             */

            var apiFetcher = new ActionBlock<int>(
                async pageNr =>
                {
                    var result = await FetchCharacter(pageNr);
                    concurrentBag.Add(result.Characters);
                }
                , new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = MaxParallel, // Degree of parallelism is controlled by this property.
                    SingleProducerConstrained = false // Instructing about the SingleProducer enhances the performance.
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
            /* This is an experimental method and NOT in use.
               This method links two DataFlow blocks.
                1. TransformBlock for fetching API data and pass to the next block
                2. AcitonBlock to save the fetched data.
            
                The idea is that because API Fetch is much slower than the db insert, 
                parallel execution of API fetch and db save can reduce some of db saving load.
                
                But the result seems to show the gain from this approach is less than the overhead of 
                having two TPL blocks being maintained.
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
                db.BulkInsert(pc.Characters.ToArray());
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
            var result = await service.GetCharacterSinglePage(page: pageNr,
                                                               characterStatus: CharacterStatus.Alive);
            Console.WriteLine($"API fetch finished. Page: {pageNr}");
            return result;
        }

        private static async Task CleanupDbAsync()
        {
            using var db = new RickAndMortyContext(dbOptions);
            db.Database.EnsureCreated();
            var existing = await db.Characters.CountAsync();
            Console.WriteLine($"Existing items {existing}");
            db.Database.EnsureDeleted();
            Console.WriteLine("DB deleted.");
            db.Database.EnsureCreated();
            Console.WriteLine("DB re-created.");
        }

        private static void ConfigureServices()
        {
            var dbFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                , "RickMorty.db");
            // Use Sqlite file database 
            dbOptions = new DbContextOptionsBuilder<RickAndMortyContext>()
                 .UseSqlite($"DataSource={dbFilePath}").Options;

            // Use Dependency Injection and configure the services
            var services = new ServiceCollection();
            services.AddTransient<IRickAndMortyService, RickAndMortyService>()
                    .AddSingleton(dbOptions)
                    .AddAutoMapper(c => c.AddProfile<RickAndMortyMapperProfile>());


            // Register internal DefaultHttpClientFactory
            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#how-to-use-typed-clients-with-ihttpclientfactory
            services.AddHttpClient<IRickAndMortyService, RickAndMortyService>(
                                      c =>
                                      {
                                          c.BaseAddress = uri;
                                      });
            serviceProvider = services.BuildServiceProvider();

            /* Increase maximum concurrent network requests */
            ServicePointManager.DefaultConnectionLimit = MaxParallel;

            Console.WriteLine("Configuration completed.");
        }
         
    }

}

