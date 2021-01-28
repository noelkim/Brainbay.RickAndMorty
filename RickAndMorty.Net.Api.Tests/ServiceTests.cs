using System;
using System.Linq;
using AutoMapper;
using Brainbay.Submission.DataAccess.Mapper;
using Brainbay.Submission.DataAccess.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Net.Api.Service;
using Xunit;

namespace RickAndMorty.Net.Api.Tests
{
    public class ServiceTests
    {
        private IRickAndMortyService RickAndMortyService { get; }

        public ServiceTests()
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

        [Theory]
        [InlineData(6)]
        public async void GetCharacterTest(int value)
        {
            var result = await RickAndMortyService.GetCharacter(value);

            Assert.NotNull(result);
            Assert.True(result.Id == value);
            Assert.True(!String.IsNullOrEmpty(result.Name));
            Assert.True(!String.IsNullOrEmpty(result.Species));
            Assert.True(result.Created != default(DateTime));
            Assert.NotEmpty(result.Episode);
        }

        [Fact]
        public async void GetAllCharactersTest()
        {
            var result = await RickAndMortyService.GetAllCharacters();

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(!String.IsNullOrEmpty(result.First().Name));
            Assert.True(!String.IsNullOrEmpty(result.First().Species));
            Assert.True(result.First().Created != default(DateTime));
            Assert.NotEmpty(result.First().Episode);
        }

        [Theory]
        [InlineData(new[] { 5, 10 })]
        public async void GetMultipleCharactersTest(int[] value)
        {
            var result = await RickAndMortyService.GetMultipleCharacters(value);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(!String.IsNullOrEmpty(result.First().Name));
            Assert.True(!String.IsNullOrEmpty(result.First().Species));
            Assert.True(result.First().Created != default(DateTime));
            Assert.NotEmpty(result.First().Episode);
        }

        [Theory]
        [InlineData(CharacterStatus.Alive)]
        public async void FilterCharactersTest(CharacterStatus value)
        {
            var result = await RickAndMortyService.FilterCharacters(characterStatus: value);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(!String.IsNullOrEmpty(result.First().Name));
            Assert.True(!String.IsNullOrEmpty(result.First().Species));
            Assert.True(result.First().Created != default(DateTime));
            Assert.NotEmpty(result.First().Episode);
        }

        [Fact]
        public async void GetAllLocationsTest()
        {
            var result = await RickAndMortyService.GetAllLocations();

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(!String.IsNullOrEmpty(result.First().Name));
            Assert.True(!String.IsNullOrEmpty(result.First().Dimension));
            Assert.True(result.First().Created != default(DateTime));
            Assert.NotEmpty(result.First().Residents);
        }

        [Theory]
        [InlineData(new[] { 5, 10 })]
        public async void GetMultipleLocationsTest(int[] value)
        {
            var result = await RickAndMortyService.GetMultipleLocations(value);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(!String.IsNullOrEmpty(result.First().Name));
            Assert.True(!String.IsNullOrEmpty(result.First().Dimension));
            Assert.True(result.First().Created != default(DateTime));
            Assert.NotEmpty(result.First().Residents);
        }

        [Theory]
        [InlineData(6)]
        public async void GetLocationTest(int value)
        {
            var result = await RickAndMortyService.GetLocation(value);

            Assert.NotNull(result);
            Assert.True(result.Id == value);
            Assert.True(!String.IsNullOrEmpty(result.Name));
            Assert.True(!String.IsNullOrEmpty(result.Dimension));
            Assert.True(result.Created != default(DateTime));
            Assert.NotEmpty(result.Residents);
        }

        [Theory]
        [InlineData("earth")]
        public async void FilterLocationsTest(string value)
        {
            var result = await RickAndMortyService.FilterLocations(value);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(!String.IsNullOrEmpty(result.First().Name));
            Assert.True(!String.IsNullOrEmpty(result.First().Dimension));
            Assert.True(result.First().Created != default(DateTime));
            Assert.NotEmpty(result.First().Residents);
        }

        [Fact]
        public async void GetAllEpisodesTest()
        {
            var result = await RickAndMortyService.GetAllEpisodes();

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(!String.IsNullOrEmpty(result.First().Name));
            Assert.True(!String.IsNullOrEmpty(result.First().EpisodeCode));
            Assert.True(result.First().Created != default(DateTime));
            Assert.NotEmpty(result.First().Characters);
        }

        [Theory]
        [InlineData(5)]
        public async void GetEpisodeTest(int value)
        {
            var result = await RickAndMortyService.GetEpisode(value);

            Assert.NotNull(result);
            Assert.True(result.Id == value);
            Assert.True(!String.IsNullOrEmpty(result.Name));
            Assert.True(!String.IsNullOrEmpty(result.EpisodeCode));
            Assert.True(result.Created != default(DateTime));
            Assert.NotEmpty(result.Characters);
        }

        [Theory]
        [InlineData(new[] { 5, 10 })]
        public async void GetMultipleEpisodesTest(int[] value)
        {
            var result = await RickAndMortyService.GetMultipleEpisodes(value);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(!String.IsNullOrEmpty(result.First().Name));
            Assert.True(!String.IsNullOrEmpty(result.First().EpisodeCode));
            Assert.True(result.First().Created != default(DateTime));
            Assert.NotEmpty(result.First().Characters);
        }

        [Theory]
        [InlineData("Rick")]
        public async void FilterEpisodesTest(string value)
        {
            var result = await RickAndMortyService.FilterEpisodes(name: value);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(!String.IsNullOrEmpty(result.First().Name));
            Assert.True(!String.IsNullOrEmpty(result.First().EpisodeCode));
            Assert.True(result.First().Created != default(DateTime));
            Assert.NotEmpty(result.First().Characters);
        }
    }
}
