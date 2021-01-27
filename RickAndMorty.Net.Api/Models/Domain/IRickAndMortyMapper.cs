using AutoMapper;

namespace RickAndMorty.Net.Api.Models.Domain
{
    public interface IRickAndMortyMapper
    {
        IMapper Mapper { get; set; }
    }
}
