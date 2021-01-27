using AutoMapper;
using RickAndMorty.Net.Api.Mapper;

namespace RickAndMorty.Net.Api.Models.Domain
{
    public class RickAndMortyMapper : IRickAndMortyMapper
    {
        public IMapper Mapper { get; set; }


        public static RickAndMortyMapper Create()
        {
            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new RickAndMortyMapperProfile()));
            return new RickAndMortyMapper { Mapper = mapperConfig.CreateMapper() };
        }
    }
}
