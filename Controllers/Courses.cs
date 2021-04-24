using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("Api/authors/{authorId}/courses")]
    public class Courses : ControllerBase
    {
        private readonly CourseLibraryContext courseLibrary;
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public Courses(CourseLibraryContext courseLibrary,
            ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            this.courseLibrary = courseLibrary;
            this.courseLibraryRepository = courseLibraryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CoursesDto>> GetCourses(Guid authorId)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var databaseResult = courseLibraryRepository.GetCourses(authorId);
            return Ok(mapper.Map<IEnumerable<CoursesDto>>(databaseResult));
        }

        [HttpGet("{courseId}", Name = "getCourse")]
        public ActionResult<CoursesDto> GetCourse(Guid authorId, Guid courseId)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
                return NotFound();
            var databaseResult = courseLibraryRepository.GetCourse(authorId, courseId);

            if (databaseResult == null)
                return NotFound();

            return Ok(mapper.Map<CoursesDto>(databaseResult));
        }

        [HttpPost]
        public ActionResult<CoursesDto> CreateCourse(Guid authorId, CreateCoursesDto course)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var newCourse = mapper.Map<Entities.Course>(course);
            courseLibraryRepository.AddCourse(authorId, newCourse);
            courseLibraryRepository.Save();
            return CreatedAtRoute("getCourse", 
                new { courseId = newCourse.Id, authorId = newCourse.AuthorId },
                mapper.Map<Models.CreateCoursesDto>(newCourse));
        }
    }
}
