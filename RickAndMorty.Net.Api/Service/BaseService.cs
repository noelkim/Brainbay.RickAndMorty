using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Brainbay.Submission.DataAccess.Models.Dto;
using EnsureThat;
using Newtonsoft.Json;
using RickAndMorty.Net.Api.Helpers;

namespace RickAndMorty.Net.Api.Service
{
    internal abstract class BaseService
    {
        private HttpClient Client { get; }
        protected IMapper Mapper { get; }

        protected BaseService(HttpClient httpClient, IMapper mapper)
        {
            Ensure.Any.IsNotNull(httpClient);
            Client = httpClient;
            Mapper = mapper;
        }

        /// <summary>
        /// HTTP get async and json deserialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        protected async Task<T> Get<T>(string path)
        {
            var response = await Client.GetAsync(path);
            return response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync()) : default(T);
        }

        /// <summary>
        /// Gets all pages objects to single enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<T>> GetPages<T>(string url)
        {
            var result = new List<T>();
            var nextPage = -1;

            do
            {
                var dto = await Get<PageDto<T>>(nextPage == -1 ? url : $"{url}{(url.Contains("?") ? "&" : "?")}page={nextPage}");
                result.AddRange(dto.Results);

                nextPage = dto.Info.Next.GetNextPageNumber();
            }
            while (nextPage != -1);

            return result;
        }
    }
}
