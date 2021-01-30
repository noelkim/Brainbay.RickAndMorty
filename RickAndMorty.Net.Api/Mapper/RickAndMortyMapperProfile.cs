using System.Linq;
using AutoMapper;
using Brainbay.Submission.DataAccess.Models.Domain;
using Brainbay.Submission.DataAccess.Models.Dto;
using Brainbay.Submission.DataAccess.Models.Enums;

namespace RickAndMorty.Net.Api.Mapper
{
    public class RickAndMortyMapperProfile : Profile
    {
        public RickAndMortyMapperProfile()
        {
            CreateMap<CharacterLocationDto, CharacterLocation>()
                       .ConstructUsing(cls =>
                           new CharacterLocation(cls.Name, cls.Url.ToUri()));

            CreateMap<CharacterOriginDto, CharacterOrigin>()
                .ConstructUsing(cls =>
                    new CharacterOrigin(cls.Name, cls.Url.ToUri()));

            CreateMap<CharacterDto, Character>()
                .ConstructUsing(cls =>
                    new Character(cls.Id, cls.Name, cls.Status ,
                        cls.Species, cls.Type, cls.Gender,
                        cls.Location.Url.ToUri(),
                        cls.Origin.Url.ToUri(),
                        cls.Image.ToUri(), cls.Episode.ToList(),
                        cls.Url.ToUri(), cls.Created.ToDateTime()));


            CreateMap<LocationDto, Location>()
                .ConstructUsing(cls =>
                    new Location(cls.Id, cls.Name, cls.Type, cls.Dimension,
                            cls.Residents.ToList(), cls.Url,
                        cls.Created.ToDateTime()));

            CreateMap<EpisodeDto, Episode>()
                .ConstructUsing(cls =>
                    new Episode(cls.Id, cls.Name, cls.Air_date.ToDateTime(), cls.Episode,
                        cls.Characters.ToList(), cls.Url, cls.Created.ToDateTime()));

            AllowNullCollections = true;

        }

    }

}
