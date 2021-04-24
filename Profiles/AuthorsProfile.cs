using AutoMapper;
using System;

namespace CourseLibrary.API.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Entities.Author, Models.AuthorDto>()
                .ForMember(dest => dest.Name,
                src => src.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Age,
            src => src.MapFrom(src => ((DateTimeOffset.UtcNow - src.DateOfBirth).TotalDays / 365)
            ));

            CreateMap<Entities.Author, Models.CreateAuthorDto>().ReverseMap();
        }
    }
}
