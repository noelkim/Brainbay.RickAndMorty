﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Brainbay.Submission.DataAccess.Models.Domain;
using Brainbay.Submission.DataAccess.Models.Dto;
using Brainbay.Submission.DataAccess.Models.Enums;
using EnsureThat;
using RickAndMorty.Net.Api.Helpers;

namespace RickAndMorty.Net.Api.Service
{
    public class RickAndMortyService : BaseService, IRickAndMortyService
    {

        public RickAndMortyService(HttpClient httpClient, IMapper mapper)
            : base(httpClient, mapper)
        {
        }

        public async Task<Character> GetCharacter(int id)
        {
            Ensure.Bool.IsTrue(id > 0);

            var dto = await Get<CharacterDto>($"api/character/{id}");

            return Mapper.Map<Character>(dto);
        }

        public async Task<IEnumerable<Character>> GetAllCharacters()
        {
            var dto = await GetPages<CharacterDto>("api/character/");

            return Mapper.Map<IEnumerable<Character>>(dto);
        }

        public async Task<IEnumerable<Character>> GetMultipleCharacters(int[] ids)
        {
            Ensure.Bool.IsTrue(ids.Any());

            var dto = await Get<IEnumerable<CharacterDto>>($"api/character/{string.Join(",", ids)}");

            return Mapper.Map<IEnumerable<Character>>(dto);
        }

        public async Task<IEnumerable<Character>> FilterCharacters(string name = "",
            CharacterStatus? characterStatus = null,
            string species = "",
            string type = "",
            CharacterGender? gender = null)
        {
            Ensure.Bool.IsTrue(!string.IsNullOrEmpty(name) || characterStatus != null ||
                               !string.IsNullOrEmpty(species) || !string.IsNullOrEmpty(type) || gender != null);

            var url = "/api/character/".BuildCharacterFilterUrl(name,
                                                                characterStatus,
                                                                species,
                                                                type,
                                                                gender);

            var dto = await GetPages<CharacterDto>(url);

            return Mapper.Map<IEnumerable<Character>>(dto);
        }


        public async Task<PagedCharacters> GetCharacterSinglePage(
            int page = 1,
            string name = "",
            CharacterStatus? characterStatus = null,
            string species = "",
            string type = "",
            CharacterGender? gender = null)
        {           
            var url = "/api/character/".BuildCharacterFilterUrl(name,
                                                                characterStatus,
                                                                species,
                                                                type,
                                                                gender);
            var dto = await Get<PageDto<CharacterDto>>(url + "&page=" + page);

            return new PagedCharacters(page, dto.Info, Mapper.Map<IEnumerable<Character>>(dto.Results));
        }

        public async Task<IEnumerable<Location>> GetAllLocations()
        {
            var dto = await GetPages<LocationDto>("api/location/");

            return Mapper.Map<IEnumerable<Location>>(dto);
        }

        public async Task<IEnumerable<Location>> GetMultipleLocations(int[] ids)
        {
            Ensure.Bool.IsTrue(ids.Any());

            var dto = await Get<IEnumerable<LocationDto>>($"api/location/{string.Join(",", ids)}");

            return Mapper.Map<IEnumerable<Location>>(dto);
        }

        public async Task<Location> GetLocation(int id)
        {
            Ensure.Bool.IsTrue(id > 0);

            var dto = await Get<LocationDto>($"api/location/{id}");

            return Mapper.Map<Location>(dto);
        }

        public async Task<IEnumerable<Location>> FilterLocations(string name = "",
            string type = "",
            string dimension = "")
        {
            Ensure.Bool.IsTrue(!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(type) || !string.IsNullOrEmpty(dimension));

            var url = "/api/location/".BuildLocationFilterUrl(name, type, dimension);

            var dto = await GetPages<LocationDto>(url);

            return Mapper.Map<IEnumerable<Location>>(dto);
        }

        public async Task<IEnumerable<Episode>> GetAllEpisodes()
        {
            var dto = await GetPages<EpisodeDto>("api/episode/");

            return Mapper.Map<IEnumerable<Episode>>(dto);
        }

        public async Task<Episode> GetEpisode(int id)
        {
            Ensure.Bool.IsTrue(id > 0);

            var dto = await Get<EpisodeDto>($"api/episode/{id}");

            return Mapper.Map<Episode>(dto);
        }

        public async Task<IEnumerable<Episode>> GetMultipleEpisodes(int[] ids)
        {
            Ensure.Bool.IsTrue(ids.Any());

            var dto = await Get<IEnumerable<EpisodeDto>>($"api/episode/{string.Join(",", ids)}");

            return Mapper.Map<IEnumerable<Episode>>(dto);
        }

        public async Task<IEnumerable<Episode>> FilterEpisodes(string name = "",
            string episode = "")
        {
            Ensure.Bool.IsTrue(!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(episode));

            var url = "/api/episode/".BuildEpisodeFilterUrl(name, episode);

            var dto = await GetPages<EpisodeDto>(url);

            return Mapper.Map<IEnumerable<Episode>>(dto);
        }
    }
}
