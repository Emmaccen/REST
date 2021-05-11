using AutoMapper;

namespace CourseLibrary.API.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Entities.Course, Models.CoursesDto>();

            CreateMap<Entities.Course, Models.CreateCoursesDto>().ReverseMap();

            CreateMap<Entities.Course, Models.UpdateCourseDto>().ReverseMap();
        }
    }
}
