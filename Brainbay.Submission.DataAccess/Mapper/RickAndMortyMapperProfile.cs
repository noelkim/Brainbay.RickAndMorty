using System.Linq;
using AutoMapper;
using Brainbay.Submission.DataAccess.Helpers;
using Brainbay.Submission.DataAccess.Models.Domain;
using Brainbay.Submission.DataAccess.Models.Dto;
using Brainbay.Submission.DataAccess.Models.Enums;

namespace Brainbay.Submission.DataAccess.Mapper
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
                    new Character(cls.Id, cls.Name, cls.Status.ToEnum<CharacterStatus>(),
                        cls.Species, cls.Type, cls.Gender.ToEnum<CharacterGender>(),
                        new CharacterLocation(cls.Location.Name, cls.Location.Url.ToUri()),
                        new CharacterOrigin(cls.Origin.Name, cls.Origin.Url.ToUri()),
                        cls.Url.ToUri(), cls.Episode.Select(x => x.ToUri()).ToList(),
                        cls.Url.ToUri(), cls.Created.ToDateTime()));


            CreateMap<LocationDto, Location>()
                .ConstructUsing(cls =>
                    new Location(cls.Id, cls.Name, cls.Type, cls.Dimension, cls.Residents.Select(x => x.ToUri()).ToList(), cls.Url.ToUri(),
                        cls.Created.ToDateTime()));

            CreateMap<EpisodeDto, Episode>()
                .ConstructUsing(cls =>
                    new Episode(cls.Id, cls.Name, cls.Air_date.ToDateTime(), cls.Episode,
                        cls.Characters.Select(x => x.ToUri()).ToList(), cls.Url.ToUri(), cls.Created.ToDateTime()));

            AllowNullCollections = true;

        }

    }

}
