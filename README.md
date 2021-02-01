# Brainbay.RickAndMorty 
This solution is an assignment submission for the Brainbay technical assessment.

## How to run
Build and start the console app Brainbay.Submission.ApiScraper for the assignment 1.
Build and start the ASP.NET MVC app Brainbay.Submission.CharacterWeb for the assignment 2.

## Assignment 1: Console Application
### Objectives and solutions
1. Access the REST API using the HttpClient class from the Microsoft.Extensions.Http NuGet package.
   solution: 
   HttpClient has a few known issues like socket exhaustion and failure of DNS change with shared instance. 
   To avoid this and ensure the optimal performance, `IHttpClientFactory` interface is used as suggested by this [.Net Architecture Guide](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-request).
   
2. The program should get all the characters from this endpoint: https://rickandmortyapi.com/api/character/
   solution:
   A [.Net Api library](https://github.com/Carlj28/RickAndMorty.Net.Api) already contributed by Carlj28(https://github.com/Carlj28) is forked.
   Below modifications are made.
      * Removed the factory pattern used for the `BaseService` and `AutoMapper` instance and replaced with Dependency Injection pattern. 
     This allows `HttpClient` and `AutoMapperProfiler` to be injected.
      * Adjusted and moved model and DTO classes.
      
3. Use any ADO.NET technology (for example Entity Framework) to save the data.
   solution:
   EF Core is used with SQLite file database.
   EF Core's `DbContextOptions` with in-memory type makes it possible to run unit tests against EF CRUD operations.
   Using SQLite is a convenient solution for simple db. But it comes with limitations on performance optimization. With this submistion the limitation is pronounced as lacking the bulk insertion.
   Performance of inserting a few hundred entities can be greatly improved by using BulkCopy or BulkInsert. However, since SQLite uses [UPSERT](https://www.sqlite.org/lang_UPSERT.html) this is not possible.

4. Before saving the data, the database has to be emptied.
   solution: 
   At the beginning of the program execution, EF core's `Database.EnsureDeleted()` is used for recreating the whole database.

 ## Assignment 2: MVC Web Application
 ### Objectives and solutions
1. Shows a list of all the characters and offers the functionality to add a new character to the database. 
    solution: 
    MVC 5 project is created and Scaffolding template is used for creating the `CharacterController` and views for list/detail/edit/delete/create actions.

2. Retrieving the list of characters should only happen once every 5 minutes as long as no new character has been added. 
   solution:
   [ASP.NET Memory Cache] is used to store the Characters. Absolute expiration of 5 minutes is set.
   ```csharp
    var characters = await memoryCache.GetOrCreateAsync(CacheKeyCharacters,
                async entry =>
             {
                 entry.AbsoluteExpiration = DateTime.Now.AddMinutes(5); // Absolute expiration after 5 minutes
                 Response.Headers["X-cache-info"] = "from-database";
                 return await _context.Characters.ToListAsync();
             });

   ```
   When there is a edit/delete/create action of a character then the cache is invalidated.
   ```csharp
    memoryCache.Remove(CacheKeyCharacters);
   ```
   
   
3. A response header should be used to indicate whether the application got its data from the database.
   The name of the response header should be ‘from-database’.
   solution:
   A custom response header `x-cache-info` is added and 'from-cache' or 'from-database' is returned based on the source.

   
 
 ### Performance Optimization
 The process of querying from Web API and saving to the database is a form of data [ETL](https://en.wikipedia.org/wiki/Extract,_transform,_load).
 Given task has two distinctive tech-stacks namely Web and DB. Separating the extration part from saving part is important to keep two different concerns apart.
 Without overdoing the architecture, [TPL Dataflow library from the Microsoft](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library) provides an easy means to setup the pipleline of distinctive tasks.
 Using the TPL Dataflow comes with below benefits:
    * Fine control over the parallelism
    * Thread safety over the mutable objects
    * Modular building blocks for flexible ETL pipeline
 Although TPL Dataflow is not anymore performant than a raw multi-threaded programming, it is used for this assignment to give more emphasis on the separation of concern principle.
 
   ```csharp
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

   ```
 
 Default setting for the concurrent connection is only 2 for Console applications. (https://docs.microsoft.com/en-us/dotnet/api/system.net.servicepointmanager.defaultconnectionlimit?view=net-5.0)
 Therefore, [ServicePointManager](https://docs.microsoft.com/en-us/dotnet/api/system.net.servicepointmanager.defaultconnectionlimit?view=net-5.0) is used to set the connection limit same as the maximum parallel operations.
 ```csharp
 ServicePointManager.DefaultConnectionLimit = MaxParallel;
 ```
 
 ## For Extra Points
Create a view that shows all the characters from a given planet. The application should accept the planet in the URL.
solution:
Because of a time constraint, this is implemented in a simple way.
Adding a query string `locationId` and passing in the Id property of a character's origin location will filter the Index view as such.
e.g) https://localhost:44351/Characters?locationId=1  

## Unit test projects
Brainbay.Submission.DataAccess has its matching test project Brainbay.Submission.DataAcess.Tests with below test classes.
* ChracterEntityTests.cs: this contains the EF entity tests for Chracter class
* ChracterDtoTests.cs: this contains the EF data saving from the DTO objects so that the DTO<>Entity mapping done with AutoMapper can be tested.
* EntityTestsBase.cs: includes the test setup codes. Although EF in-memory option can be used, instead SQLite with memory datasource is used.
```csharp
            // Use Sqlite in-memory database 
            dbOptions = new DbContextOptionsBuilder<RickAndMortyContext>()
                .UseSqlite(CreateInMemoryDatabase()).Options;
```
  This overcomes the limitation of EF in-memory option being non-relational database. 
  
Original .Net API from Carlj28 comes with test project. This is modified with below points.
* [FluentAssertions](https://fluentassertions.com/) is used to improve the assert statement readability as below.
```csharp

            // ASSERT
            characterList.Count.Should().BeGreaterThan(0, "The result cannot be empty.");
            characterList.Count.Should().Be(totalCount, "It is expected to fetch all characters.");
            characterList.Select(n => n.Id).Should().OnlyHaveUniqueItems("ID is expected to be unique.");
```
* Dependency Injection container `ServiceCollection` is used to inject the injectables.
Due to the fact that these tests are integration unit tests, Moq or usual Stub generating tools are not used.


# Future improvements
Due to the time restriction, the submission has ample room for the future improvements.
## More performance optimization
* Devise a local Web API to remove the network latency from the equasion
* [Use Benchmark library](https://github.com/dotnet/BenchmarkDotNet) for module level perf data collection.
* For the server farm, distributed cache is more desirable than the memory cache


## Resilient API fetching
* Prepare for the Rate Limiting from the server by having a dynamic throttling. (most do for the protection of DDos attack)
* Introduce transaction per page or per domain to roll back in case of a partial failure. 




 
