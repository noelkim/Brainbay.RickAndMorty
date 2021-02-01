using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Brainbay.Submission.DataAccess.Models.Domain;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Net.Api.Mapper;
using RickAndMorty.Net.Api.Service;
using Xunit;
using FluentAssertions;

namespace RickAndMorty.Net.Api.Tests
{
    public class CharacterTests
    {
        private IRickAndMortyService RickAndMortyService { get; }

        public CharacterTests()
        {
            // Use Dependency Injection and configure the services
            var services = new ServiceCollection();
            services.AddTransient<IRickAndMortyService, RickAndMortyService>()
                    .AddAutoMapper(c => c.AddProfile<RickAndMortyMapperProfile>());


            // Register internal DefaultHttpClientFactory
            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#how-to-use-typed-clients-with-ihttpclientfactory
            services.AddHttpClient<IRickAndMortyService, RickAndMortyService>(
                                      c => c.BaseAddress = new Uri("https://rickandmortyapi.com/"));
            var serviceProvider = services.BuildServiceProvider();
            RickAndMortyService = serviceProvider.GetRequiredService<IRickAndMortyService>();
        }


        [Fact]
        public async void GetAllCharactersTest()
        {
            // ARRANGE
            var pageNr = 1;
            var characterList = new List<Character>();

            // Get the first page
            var result = await RickAndMortyService.GetCharacterSinglePage(
                page: pageNr);

            var totalCount = result.PageInfo.Count;
            characterList.AddRange(result.Characters);

            // ACT
            // Fetch all remaining pages
            while (pageNr < result.PageInfo.Pages)
            {
                pageNr++;
                result = await RickAndMortyService.GetCharacterSinglePage(page: pageNr);
                characterList.AddRange(result.Characters);
            }

            // ASSERT
            characterList.Count.Should().BeGreaterThan(0, "The result cannot be empty.");
            characterList.Count.Should().Be(totalCount, "It is expected to fetch all characters.");
            characterList.Select(n => n.Id).Should().OnlyHaveUniqueItems("ID is expected to be unique.");
        }

    }
}
