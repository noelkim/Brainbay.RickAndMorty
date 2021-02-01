using Brainbay.Submission.DataAccess.Models.Enums;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RickAndMorty.Net.Api.Tests")]
namespace RickAndMorty.Net.Api.Helpers
{
    internal static class BuildFilterUrlHelpers
    {
        //TODO: change to generic param method Dictionary<string, string>? value and nameof?

        public static string BuildCharacterFilterUrl(this string baseUrl,
            string name = "",
            CharacterStatus? status = null,
            string species = "",
            string type = "",
            CharacterGender? gender = null) => baseUrl + "?" +
                                               (!string.IsNullOrEmpty(name) ? $"{nameof(name)}={name}&" : "") +
                                               (status != null ? $"{nameof(status)}={status}&" : "") +
                                               (!string.IsNullOrEmpty(species) ? $"{nameof(species)}={species}&" : "") +
                                               (!string.IsNullOrEmpty(type) ? $"{nameof(type)}={type}&" : "") +
                                               (gender != null ? $"{nameof(gender)}={gender}" : "");

        public static string BuildLocationFilterUrl(this string baseUrl,
            string name = "",
            string type = "",
            string dimension = "") => baseUrl + "?" +
                                      (!string.IsNullOrEmpty(name) ? $"{nameof(name)}={name}&" : "") +
                                      (!string.IsNullOrEmpty(type) ? $"{nameof(type)}={type}&" : "") +
                                      (!string.IsNullOrEmpty(dimension) ? $"{nameof(dimension)}={dimension}" : "");

        public static string BuildEpisodeFilterUrl(this string baseUrl,
            string name = "",
            string episode = "") => baseUrl + "?" +
                                      (!string.IsNullOrEmpty(name) ? $"{nameof(name)}={name}&" : "") +
                                      (!string.IsNullOrEmpty(episode) ? $"{nameof(episode)}={episode}&" : "");
    }
}
