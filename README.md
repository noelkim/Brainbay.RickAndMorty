# Brainbay.RickAndMorty 
This solution is an assignment submission for the Brainbay technical assessment.

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

 
