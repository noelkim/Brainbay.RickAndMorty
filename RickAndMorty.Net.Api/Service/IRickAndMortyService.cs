﻿using System.Collections.Generic;
using System.Threading.Tasks;
using RickAndMorty.Net.Api.Models.Domain;
using RickAndMorty.Net.Api.Models.Enums;

namespace RickAndMorty.Net.Api.Service
{
    public interface IRickAndMortyService
    {
        Task<Character> GetCharacter(int id);
        Task<IEnumerable<Character>> GetAllCharacters();
        Task<IEnumerable<Character>> GetMultipleCharacters(int[] ids);
        Task<IEnumerable<Character>> FilterCharacters(string name = "",
            CharacterStatus? characterStatus = null,
            string species = "",
            string type = "",
            CharacterGender? gender = null);
        Task<IEnumerable<Location>> GetAllLocations();
        Task<IEnumerable<Location>> GetMultipleLocations(int[] ids);
        Task<Location> GetLocation(int id);
        Task<IEnumerable<Location>> FilterLocations(string name = "",
            string type = "",
            string dimension = "");
    }
}